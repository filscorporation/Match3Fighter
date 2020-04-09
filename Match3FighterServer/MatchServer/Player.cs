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

        public string Name;

        public int MaxHealth;

        public int MaxMana;

        public Player(int clientID)
        {
            ClientID = clientID;

            Name = "Player" + clientID;
            MaxHealth = 35;
            MaxMana = 50;
        }

        public PlayerData ToData()
        {
            return new PlayerData
            {
                Name = Name,
                MaxHealth = MaxHealth,
                MaxMana = MaxMana,
            };
        }
    }
}
