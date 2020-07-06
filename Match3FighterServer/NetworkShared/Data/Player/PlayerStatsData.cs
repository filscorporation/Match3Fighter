using System;
using System.Collections.Generic;

namespace NetworkShared.Data.Player
{
    [Serializable]
    public class PlayerStatsData
    {
        public string PlayerName;

        public UniqueBlockCollectionData UniqueBlockCollection;

        public SkillData[] Skills;

        public SkillData[] ActiveSkills;

        public string ActiveHero;

        public int Currency;

        public int Rating;
    }
}
