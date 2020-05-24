using System;
using NetworkShared.Data.Player;

namespace NetworkShared.Data
{
    [Serializable]
    public class GameEndResponse
    {
        public int PlayerWon;

        public PlayerStatsData PlayerStats;
    }
}
