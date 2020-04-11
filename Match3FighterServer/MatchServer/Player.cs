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

        public float BlockSwapCost = 20F;

        private DateTime lastUpdate;

        public Player(int clientID)
        {
            ClientID = clientID;

            Name = "Player" + clientID;
            MaxHealth = 100F;
            Health = MaxHealth;
            MaxMana = 200F;
            Mana = MaxMana;
            ManaPerSecond = 2.5F;

            lastUpdate = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates stats that has regeneration per second 
        /// </summary>
        public void Update()
        {
            float mana = (float)(DateTime.UtcNow - lastUpdate).TotalSeconds * ManaPerSecond;
            Mana = Math.Min(Mana + mana, MaxMana);

            lastUpdate = DateTime.UtcNow;
        }

        /// <summary>
        /// Sets stats to max
        /// </summary>
        public void Refresh()
        {
            Mana = MaxMana;
            Health = MaxHealth;
            
            lastUpdate = DateTime.UtcNow;
        }

        /// <summary>
        /// Tries to spend mana
        /// </summary>
        /// <param name="mana"></param>
        /// <returns></returns>
        public bool TrySpendMana(float mana)
        {
            Update();

            if (Mana < mana)
                return false;

            Mana -= mana;

            return true;
        }

        /// <summary>
        /// Adds mana
        /// </summary>
        /// <param name="mana"></param>
        public void GainMana(float mana)
        {
            Mana = Math.Min(Mana + mana, MaxMana);
        }

        /// <summary>
        /// Adds health
        /// </summary>
        /// <param name="health"></param>
        public void GainHealth(float health)
        {
            Health = Math.Min(Health + health, MaxHealth);
        }

        /// <summary>
        /// Makes player take damage
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage)
        {
            Health -= damage;
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
