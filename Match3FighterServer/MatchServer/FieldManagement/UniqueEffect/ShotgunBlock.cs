using System;
using System.Collections.Generic;
using System.Linq;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class ShotgunBlock : UniqueBlock
    {
        public override string Name => nameof(ShotgunBlock);
        public override BlockTypes BaseType => BlockTypes.Attack;

        public int ShotsAmount = 8;
        public int DamageToEnemyHealth = 8;

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
                enemy.TakeDamage(DamageToEnemyHealth);
                data.Add(HealthData(enemy, DamageToEnemyHealth));

                List<Block> blocks = manager.GetRandomNonDestroyedBlocks(enemyField, ShotsAmount).ToList();
                foreach (Block blockToDestroy in blocks)
                {
                    manager.DestroyBlocks(enemyField, blocks, BlockState.DestroyedByDamage);
                    data.Add(UniqueShotData(playerField, enemyField, block, blockToDestroy, "ShotgunEffect"));
                }
            }

            return data;
        }
    }
}
