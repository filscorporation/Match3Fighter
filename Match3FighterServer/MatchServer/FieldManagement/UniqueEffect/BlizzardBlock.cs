using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class BlizzardBlock : UniqueBlock
    {
        public int BlocksToFreezeAmount = 9;
        public float FreezeDuration = 12F;

        public override string Name => nameof(BlizzardBlock);

        public override int Level => 3;

        public override BlockTypes BaseType => BlockTypes.Mana;

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            Field enemyField = playerUserIndex == 1 ? match.Field2 : match.Field1;

            List<EffectData> data = new List<EffectData>();

            foreach (Block toFreezeBlock in manager.GetRandomNonDestroyedBlocks(enemyField, BlocksToFreezeAmount))
            {
                toFreezeBlock.OnBlockEffects.Add(new OnBlockEffect(OnBlockEffectType.Frozen, FreezeDuration));
            }

            return data;
        }
    }
}
