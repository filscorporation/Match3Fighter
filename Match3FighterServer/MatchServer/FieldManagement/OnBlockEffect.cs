using System;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Effects applied to one block
    /// </summary>
    public class OnBlockEffect
    {
        public OnBlockEffectType Type;

        public float Duration;

        public DateTime StartTime;

        public OnBlockEffect(OnBlockEffectType type, float duration)
        {
            Type = type;
            Duration = duration;
            StartTime = DateTime.UtcNow;
        }

        public OnBlockEffectData ToData()
        {
            return new OnBlockEffectData
            {
                Type = Type,
                Duration = Duration - (DateTime.UtcNow - StartTime).Milliseconds/1000F,
            };
        }
    }
}
