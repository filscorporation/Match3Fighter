using System;

namespace NetworkShared.Data.Field
{
    public enum GlobalEffectType
    {
        Shield,
    }

    [Serializable]
    public class GlobalEffectData
    {
        public GlobalEffectType Type;
    }
}
