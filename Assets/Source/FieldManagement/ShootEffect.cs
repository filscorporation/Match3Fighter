using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.FieldManagement
{
    /// <summary>
    /// Effect when block shoots something
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class ShootEffect : MonoBehaviour, IPointToPointEffect
    {
        private LineRenderer lineRenderer;
        private const float shootDelay = 0.1F;

        public IEnumerator Initialize(Vector2 from, Vector2 to)
        {
            yield return new WaitForSeconds(shootDelay);

            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, new Vector3(from.x, from.y, -5));
            lineRenderer.SetPosition(1, new Vector3(to.x, to.y, -5));

            Destroy(gameObject, 0.5F);
        }
    }
}
