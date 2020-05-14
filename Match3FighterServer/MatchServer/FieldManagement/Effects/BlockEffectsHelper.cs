using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    /// <summary>
    /// Common methods for block effects
    /// </summary>
    public static class BlockEffectsHelper
    {
        /// <summary>
        /// Creates unique block at random position of the combo
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="random"></param>
        /// <param name="field"></param>
        /// <param name="player"></param>
        /// <param name="combo"></param>
        /// <param name="type"></param>
        public static List<EffectData> CreateUniqueBlock(FieldManager manager, Random random, Field field, Player player, Combo combo, BlockTypes type)
        {
            //if (combo.Blocks.Any(b => b.IsUnique && b.Type != BlockTypes.Chameleon))
            //{
            //    // Unique block is not created from combo with already unique
            //    return;
            //}

            UniqueBlock block;
            switch (combo.Blocks.Count)
            {
                case 4:
                    block = player.UniqueBlockCollection.Level1Blocks[type];
                    break;
                case 5:
                    block = player.UniqueBlockCollection.Level2Blocks[type];
                    break;
                case 6:
                    block = player.UniqueBlockCollection.Level3Blocks[type];
                    break;
                default:
                    return new List<EffectData>();
            }

            Block newBlock = manager.CreateBlockInRange(field, block, combo.Blocks);
            return block.OnCreate(manager, random, player.CurrentMatch, player, newBlock);
        }

        /// <summary>
        /// Creates random unique block of passed level at random position of the combo
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="random"></param>
        /// <param name="field"></param>
        /// <param name="player"></param>
        /// <param name="combo"></param>
        /// <param name="level">Unique block level (4, 5, 6)</param>
        public static List<EffectData> CreateRandomUniqueBlock(FieldManager manager, Random random, Field field, Player player, Combo combo, int level)
        {
            UniqueBlock block;
            int index;
            switch (level)
            {
                case 4:
                    index = random.Next(player.UniqueBlockCollection.Level1Blocks.Count);
                    block = player.UniqueBlockCollection.Level1Blocks.ElementAt(index).Value;
                    break;
                case 5:
                    index = random.Next(player.UniqueBlockCollection.Level2Blocks.Count);
                    block = player.UniqueBlockCollection.Level2Blocks.ElementAt(index).Value;
                    break;
                case 6:
                    index = random.Next(player.UniqueBlockCollection.Level3Blocks.Count);
                    block = player.UniqueBlockCollection.Level3Blocks.ElementAt(index).Value;
                    break;
                default:
                    return new List<EffectData>();
            }

            Block newBlock = manager.CreateBlockInRange(field, block, combo.Blocks);
            return block.OnCreate(manager, random, player.CurrentMatch, player, newBlock);
        }
    }
}
