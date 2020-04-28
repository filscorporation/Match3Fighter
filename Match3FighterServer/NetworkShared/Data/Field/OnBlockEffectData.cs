using System;

namespace NetworkShared.Data.Field
{
    public enum OnBlockEffectType
    {
        Frozen,
    }

    [Serializable]
    public class OnBlockEffectData
    {
        public OnBlockEffectType Type;

        public float Duration;
    }
}
