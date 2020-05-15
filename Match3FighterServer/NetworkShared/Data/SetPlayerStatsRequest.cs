using System;
using NetworkShared.Data.Player;

namespace NetworkShared.Data
{
    [Serializable]
    public class SetPlayerStatsRequest
    {
        public PlayerStatsData PlayerStats;
    }
}
