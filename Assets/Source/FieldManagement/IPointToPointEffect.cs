using System.Collections;
using UnityEngine;

namespace Assets.Source.FieldManagement
{
    public interface IPointToPointEffect
    {
        IEnumerator Initialize(Vector2 from, Vector2 to);
    }
}
