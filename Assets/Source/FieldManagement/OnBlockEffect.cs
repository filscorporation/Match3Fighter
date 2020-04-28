using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkShared.Data.Field;
using UnityEngine;

namespace Assets.Source.FieldManagement
{
    public class OnBlockEffect
    {
        public OnBlockEffect(OnBlockEffectType type, float duration)
        {
            Type = type;
            Duration = duration;
        }

        public OnBlockEffectType Type;

        public float Duration;

        public GameObject Prefab;
    }
}
