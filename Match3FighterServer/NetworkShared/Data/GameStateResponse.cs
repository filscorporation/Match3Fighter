using System;
using NetworkShared.Data.Effects;

namespace NetworkShared.Data
{
    [Serializable]
    public class GameStateResponse
    {
        public EffectData[] Effects;

        public GameStateData GameState;
    }
}
