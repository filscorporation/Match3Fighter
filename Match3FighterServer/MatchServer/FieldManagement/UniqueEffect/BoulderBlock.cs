using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class BoulderBlock : UniqueBlock
    {
        public override string Name => nameof(BoulderBlock);

        public override BlockTypes BaseType => BlockTypes.Attack;

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;
            Player enemy = playerUserIndex == 1 ? match.Player2 : match.Player1;
            Field enemyField = playerUserIndex == 1 ? match.Field2 : match.Field1;

            List<EffectData> data = new List<EffectData>();

            if (enemyField.TryBlock(out var effect))
            {
                data.Add(GlobalEffectRemovedData(enemy, effect));
            }
            else
            {
                Block toBlock = manager.GetRandomNonDestroyedBlockExceptBorders(enemyField);
                manager.DestroyBlocks(enemyField, manager.GetNeighbours(enemyField, toBlock), BlockState.DestroyedByDamage);
                data.Add(UniqueShotData(playerField, enemyField, block, toBlock, "BoulderEffect"));
            }

            return data;
        }
    }
}
