using System.Collections;
using UnityEngine;

namespace Assets.Source.FieldManagement
{
    public class ProjectileEffect : MonoBehaviour, IPointToPointEffect
    {
        private Vector2 targetPosition = Vector2.zero;
        private float moveSpeed = 40F;
        private const float shootLength = 0.1F;

        public void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        }

        public IEnumerator Initialize(Vector2 from, Vector2 to)
        {
            //yield return new WaitForSeconds(shootDelay);

            transform.position = from;
            targetPosition = to;
            moveSpeed = Vector2.Distance(from, to) / shootLength;

            Destroy(gameObject, shootLength * 3);

            yield break;
        }
    }
}
