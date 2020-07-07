namespace Assets.Source.PlayerManagement
{
    public enum PlayerState
    {
        InGame,
        Dead,
    }

    /// <summary>
    /// Players stats in game
    /// </summary>
    public class Player
    {
        public PlayerState State = PlayerState.InGame;

        public int InGameID;

        public string Name;

        public float MaxHealth;

        public float Health;

        public float HealthPerSecondFromEffect;

        public float MaxMana;

        public float Mana;

        public float ManaPerSecond;

        public float ManaPerSecondFromEffects;

        public int MaxEnergy;

        public int Energy;
    }
}
