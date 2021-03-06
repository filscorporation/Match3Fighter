﻿using System.Collections.Generic;
using NetworkShared.Data.Field;
using NetworkShared.Data.Player;

namespace Assets.Source.PlayerManagement
{
    /// <summary>
    /// Players collection and active hero info
    /// </summary>
    public class PlayerStats
    {
        public string PlayerName;

        public string ActiveHero;

        public int Currency;

        public int Rating;

        public List<UniqueBlockData> Collection;

        public Dictionary<BlockTypes, UniqueBlockData> Level1Blocks;

        public Dictionary<BlockTypes, UniqueBlockData> Level2Blocks;

        public Dictionary<BlockTypes, UniqueBlockData> Level3Blocks;

        public PlayerStatsData ToData()
        {
            return new PlayerStatsData
            {
                PlayerName = PlayerName,
                ActiveHero = ActiveHero,
                UniqueBlockCollection = new UniqueBlockCollectionData
                {
                    Collection = Collection.ToArray(),
                    Level1Blocks = Level1Blocks,
                    Level2Blocks = Level2Blocks,
                    Level3Blocks = Level3Blocks,
                }
            };
        }
    }
}
