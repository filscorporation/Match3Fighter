using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class FreezeBlock : UniqueBlock
    {
        public float FreezeDuration = 15F;

        public override string Name => nameof(FreezeBlock);
        public override BlockTypes BaseType => BlockTypes.Mana;
        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            Field enemyField = playerUserIndex == 1 ? match.Field2 : match.Field1;

            List<EffectData> data = new List<EffectData>();

            Block center = manager.GetRandomNonDestroyedBlockExceptBorders(enemyField);
            foreach (Block toFreezeBlock in manager.GetNeighbours(enemyField, center))
            {
                toFreezeBlock.OnBlockEffects.Add(new OnBlockEffect(OnBlockEffectType.Frozen, FreezeDuration));
            }

            return data;
        }
    }
}
