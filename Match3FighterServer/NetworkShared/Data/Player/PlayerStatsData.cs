using System;

namespace NetworkShared.Data.Player
{
    [Serializable]
    public class PlayerStatsData
    {
        public string PlayerName;

        public UniqueBlockCollectionData UniqueBlockCollection;

        public string ActiveHero;

        public int Currency;

        public int Rating;
    }
}
