using System;

namespace NetworkShared.Data.Field
{
    public enum GlobalEffectType
    {
        Shield,
        HealOverTime,
        ManaOverTime,
    }

    [Serializable]
    public class GlobalEffectData
    {
        public GlobalEffectType Type;

        public float Value;
    }
}
