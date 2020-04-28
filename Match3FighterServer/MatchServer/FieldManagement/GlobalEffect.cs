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
        }

        public GlobalEffectType Type;

        public GlobalEffectData ToData()
        {
            return new GlobalEffectData
            {
                Type = Type,
            };
        }
    }
}
