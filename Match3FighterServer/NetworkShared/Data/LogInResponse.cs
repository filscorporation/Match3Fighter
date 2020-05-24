using System;
using NetworkShared.Data.Player;

namespace NetworkShared.Data
{
    public enum LogInType
    {
        Registered,
        SignedIn,
    }

    /// <summary>
    /// Response to clients attempt to login
    /// </summary>
    [Serializable]
    public class LogInResponse
    {
        public LogInType Type;

        public PlayerStatsData PlayerStats;
    }
}
