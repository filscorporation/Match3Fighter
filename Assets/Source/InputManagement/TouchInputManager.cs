using System.Linq;
using UnityEngine;

namespace Assets.Source.InputManagement
{
    /// <summary>
    /// Touch input
    /// </summary>
    public class TouchInputManager : InputManagerBase
    {
        private int touchID = -1;

        protected override void CheckForInput()
        {
            foreach (Touch touch in Input.touches.Where(t => t.phase == TouchPhase.Began))
            {
                touchID = touch.fingerId;

                if (ProcessInputBegin(touch.position))
                    return;
            }
            foreach (Touch touch in Input.touches.Where(t => t.phase == TouchPhase.Ended))
            {
                if (touch.fingerId == touchID)
                    ProcessInputBegin(touch.position);
                touchID = -1;
            }
        }
    }
}
