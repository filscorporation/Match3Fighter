using System;

namespace NetworkShared.Data.Player
{
    [Serializable]
    public class PlayerData
    {
        public int InGameID;

        public string Name;

        public float MaxHealth;

        public float Health;

        public float MaxMana;

        public float Mana;

        public float ManaPerSecond;

        public int MaxEnergy;

        public int Energy;
    }
}
