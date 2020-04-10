using System;
using System.Linq.Expressions;
using MatchServer.Players;
using NetworkShared.Data.Player;

namespace MatchServer
{
    /// <summary>
    /// Info about player
    /// </summary>
    public class Player
    {
        /// <summary>
        /// ID of client connection
        /// </summary>
        public int ClientID;

        /// <summary>
        /// Players device unique identifier
        /// </summary>
        public string PlayerID;

        /// <summary>
        /// Current match this player is in
        /// </summary>
        public GameMatch CurrentMatch;

        /// <summary>
        /// Shows to both players
        /// </summary>
        public string Name;

        /// <summary>
        /// Player starts fight with that much hp
        /// </summary>
        public float MaxHealth;

        /// <summary>
        /// When goes to zero - player lost
        /// </summary>
        public float Health;

        /// <summary>
        /// Player starts fight with that much mp
        /// </summary>
        public float MaxMana;

        /// <summary>
        /// Used to make turns and use skills
        /// </summary>
        public float Mana;

        /// <summary>
        /// Mana regeneration speed
        /// </summary>
        public float ManaPerSecond;

        private DateTime lastUpdate;

        public Player(int clientID)
        {
            ClientID = clientID;

            Name = "Player" + clientID;
            MaxHealth = 35F;
            Health = MaxHealth;
            MaxMana = 50F;
            Mana = MaxMana;
            ManaPerSecond = 2.5F;

            lastUpdate = DateTime.UtcNow;
        }

        public PlayerData ToData()
        {
            return new PlayerData
            {
                Name = Name,
                MaxHealth = MaxHealth,
                Health = Health,
                MaxMana = MaxMana,
                Mana = Mana,
                ManaPerSecond = ManaPerSecond,
            };
        }
    }
}
