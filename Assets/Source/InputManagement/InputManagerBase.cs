﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Source.InputManagement
{
    /// <summary>
    /// Controls all input and notifies subscribers about clicks or touches
    /// </summary>
    public abstract class InputManagerBase : MonoBehaviour
    {
        private readonly List<IInputSubscriber> subs = new List<IInputSubscriber>();

        public bool IsNeedToCheckForInput = true;

        private const float minSwipeDistance = 0.5F;
        private GameObject swipeStartObject;
        private Vector2 swipeStartPoint;

        public void Update()
        {
            if (IsNeedToCheckForInput)
                CheckForInput();
        }

        protected abstract void CheckForInput();

        /// <summary>
        /// Begin input processing
        /// </summary>
        /// <param name="inputPoint"></param>
        protected bool ProcessInputBegin(Vector2 inputPoint)
        {
            if (IsPointerOverUIObject(inputPoint))
                // Ignore input when on UI
                return false;

            var wp = Camera.main.ScreenToWorldPoint(inputPoint);
            var position = new Vector2(wp.x, wp.y);

            if (EventSystem.current.currentSelectedGameObject != null)
                return false;
            Collider2D[] hits = Physics2D.OverlapPointAll(position);
            if (hits == null || !hits.Any())
                return false;
            // TODO: process multiple hits
            Collider2D hit = hits.First();

            swipeStartObject = hit.gameObject;
            swipeStartPoint = position;

            return true;
        }

        /// <summary>
        /// Sends input events to all subscribers
        /// </summary>
        /// <param name="inputPoint"></param>
        /// <returns></returns>
        protected bool ProcessInputEnd(Vector2 inputPoint)
        {
            if (swipeStartObject == null)
                return false;

            var wp = Camera.main.ScreenToWorldPoint(inputPoint);
            var position = new Vector2(wp.x, wp.y);

            if (Vector2.Distance(position, swipeStartPoint) > minSwipeDistance)
            {
                float dx = position.x - swipeStartPoint.x;
                float dy = position.y - swipeStartPoint.y;
                BlockSwipeDirection direction;
                if (Mathf.Abs(dx) > Mathf.Abs(dy))
                {
                    direction = dx > 0 ? BlockSwipeDirection.Right : BlockSwipeDirection.Left;
                }
                else
                {
                    direction = dy > 0 ? BlockSwipeDirection.Top : BlockSwipeDirection.Bottom;
                }

                InputEvent inputEvent = new BlockSwipeEvent() { InputObject = swipeStartObject,  Direction = direction };
                foreach (IInputSubscriber subscriber in subs)
                {
                    subscriber.Handle(inputEvent);
                }

                swipeStartObject = null;
                return true;
            }

            swipeStartObject = null;
            return false;
        }

        private bool IsPointerOverUIObject(Vector2 inputPoint)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = inputPoint;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        /// <summary>
        /// Subscribe to get input notifications
        /// </summary>
        /// <param name="sub"></param>
        public void Subscribe(IInputSubscriber sub)
        {
            subs.Add(sub);
        }
    }
}