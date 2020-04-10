using System;
using NetworkShared.Data.Field;
using NetworkShared.Data.Player;

namespace NetworkShared.Data
{
    [Serializable]
    public class GameStateResponse
    {
        public GameStateData GameState;
    }
}
