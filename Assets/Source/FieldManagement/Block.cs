using System.Collections;
using System.Collections.Generic;
using NetworkShared.Data.Field;
using UnityEngine;

namespace Assets.Source.FieldManagement
{
    /// <summary>
    /// Field element
    /// </summary>
    public class Block : MonoBehaviour
    {
        public int X;

        public int Y;

        public BlockTypes Type;

        public bool IsLocked = false;

        public List<OnBlockEffect> OnBlockEffects = new List<OnBlockEffect>();

        public Field Field;

        public Animation Animation;

        public GameObject DestructionEffect;

        private const string destroyedAnimationName = "BlockDestroyed";
        private const string appearedAnimationName = "BlockAppeared";

        private const string PlayerLayerName = "PlayerBlocks";
        private const string EnemyLayerName = "EnemyBlocks";

        public static bool Freeze = false;
        private bool needToDrop = false;
        private Vector2 dropTargetPosition;
        private float dv, da = 15F;

        private bool needToMove = false;
        private Vector2 moveTargetPosition;
        private float mv = 30F;

        private const float destroyDelay = 0.1F;
        private const float flipDelay = 0.1F;
        private const float dropDelay = 0.4F;
        private const float appearDelay = 0.35F;

        public void Update()
        {
            if (!Freeze)
            {
                MoveToTarget();
                DropToTarget();
            }
        }

        /// <summary>
        /// Sets players sorting layer to the sprite
        /// </summary>
        public void SetPlayerLayer()
        {
            GetComponent<SpriteRenderer>().sortingLayerName = PlayerLayerName;
        }

        /// <summary>
        /// Sets enemy sorting layer to the sprite
        /// </summary>
        public void SetEnemyLayer()
        {
            GetComponent<SpriteRenderer>().sortingLayerName = EnemyLayerName;
        }

        private void MoveToTarget()
        {
            if (!needToMove)
                return;

            transform.position = Vector2.Lerp(transform.position, moveTargetPosition, mv * Time.deltaTime);
            if (Vector2.Distance(transform.position, moveTargetPosition) < Mathf.Epsilon)
            {
                needToMove = false;
            }
        }

        private void DropToTarget()
        {
            if (!needToDrop)
                return;
            
            transform.position = Vector2.MoveTowards(transform.position, dropTargetPosition, dv * Time.deltaTime);
            dv += da * Time.deltaTime;
            if (Vector2.Distance(transform.position, dropTargetPosition) < Mathf.Epsilon)
            {
                needToDrop = false;
            }
        }

        public IEnumerator AnimateDestroyed()
        {
            gameObject.name += " Destroy";

            yield return new WaitForSeconds(destroyDelay);

            Animation.PlayQueued(destroyedAnimationName);
            Destroy(gameObject, 2.5F);

            GameObject eff = Instantiate(DestructionEffect, transform.position + new Vector3(0, 0, 5), Quaternion.identity);
            eff.transform.SetAsFirstSibling();
            Destroy(eff, 2F);
        }

        public IEnumerator AnimateFlipped()
        {
            gameObject.name += " Flipped";

            yield return new WaitForSeconds(flipDelay);

            Animation.PlayQueued(destroyedAnimationName);
            Destroy(gameObject, 2.5F);
        }

        public IEnumerator AnimateAppeared()
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            gameObject.name += " Appear";

            yield return new WaitForSeconds(appearDelay);

            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            Animation.PlayQueued(appearedAnimationName);
        }

        public void AnimateSwap(Vector2 startingPosition)
        {
            moveTargetPosition = transform.position;
            transform.position = startingPosition;
            needToMove = true;

            gameObject.name += " Swap";
        }

        public IEnumerator AnimateDropped(Vector2 startingPosition, bool isNew = false)
        {
            dropTargetPosition = transform.position;
            transform.position = startingPosition;

            gameObject.name += " Drop";

            yield return new WaitForSeconds(dropDelay);
            dv = 0;
            needToDrop = true;
            needToMove = false;
        }
    }
}
