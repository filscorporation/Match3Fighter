using System;
using System.Collections.Generic;

namespace NetworkShared.Data.Effects
{
    public enum EffectType
    {
        HealthChanged = 1,
        ManaChanged = 2,
        BlockShot = 3,
        GlobalEffect = 10,
        UniqueEffect = 20,
    }

    [Serializable]
    public class EffectData
    {
        public EffectType EffectType;

        public Dictionary<string, object> Data;
    }
}
