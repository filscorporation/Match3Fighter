using System;

namespace MatchServer.Players
{
    /// <summary>
    /// AI controled instance of the player
    /// </summary>
    public class Bot : Player
    {
        public const float BotActDelay = 3F;
        private DateTime botActDelayTimer;

        public Bot() : base(0)
        {
            botActDelayTimer = DateTime.UtcNow;
        }

        /// <summary>
        /// Returns if bot can act
        /// </summary>
        public bool CanAct => (DateTime.UtcNow - botActDelayTimer).TotalSeconds > BotActDelay;

        /// <summary>
        /// Refresh can act variables
        /// </summary>
        public void UpdateAct()
        {
            botActDelayTimer = DateTime.UtcNow;
        }
    }
}
