using System;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Global effect applied to players field (himself)
    /// </summary>
    public class GlobalEffect
    {
        public GlobalEffect(GlobalEffectType type)
        {
            Type = type;

            lastUpdate = DateTime.UtcNow;
        }

        public GlobalEffect(GlobalEffectType type, float value)
        {
            Type = type;
            Value = value;

            lastUpdate = DateTime.UtcNow;
        }

        public GlobalEffectType Type;

        public float Value;

        private DateTime lastUpdate;

        /// <summary>
        /// Returns updated over time value
        /// </summary>
        public float UpdateValue()
        {
            float newValue = (float)(DateTime.UtcNow - lastUpdate).TotalSeconds * Value;
            lastUpdate = DateTime.UtcNow;

            return newValue;
        }

        public GlobalEffectData ToData()
        {
            return new GlobalEffectData
            {
                Type = Type,
                Value = Value,
            };
        }
    }
}
