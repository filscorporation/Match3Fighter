using System;
using System.Collections.Generic;
using System.Linq;
using MatchServer.Players;
using MatchServer.UpgradesManagement;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    public class AttackBlockEffect : Effect
    {
        public int BlocksToAttackCount = 1;
        public float DamageToBlockHealth = 1F;
        public float DamageToEnemyHealth = 10F;

        public override BlockTypes ComboEffectType => BlockTypes.Attack;

        public override List<EffectData> Apply(FieldManager manager, UpgradeManager upgradeManager, Random random, GameMatch match, int playerUserIndex, Combo combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;
            Player enemy = playerUserIndex == 1 ? match.Player2 : match.Player1;
            Field enemyField = playerUserIndex == 1 ? match.Field2 : match.Field1;
            UpgradesInfo playerUpgradesInfo = playerUserIndex == 1 ? match.Player1Upgrades : match.Player2Upgrades;

            int effectsCount = Math.Max(1, combo.Blocks.Count - FieldManager.MinComboCount);

            List<EffectData> data = new List<EffectData>();

            if (enemyField.TryBlock(out var effect))
            {
                data.Add(GlobalEffectRemovedData(enemy, effect.Type));
            }
            else
            {
                float damage = DamageToEnemyHealth * combo.EffectScale
                               * upgradeManager.GetAttackBlockUpgradeBonus(playerUpgradesInfo);
                data.Add(HealthData(enemy, -damage * effectsCount));
                for (int i = 0; i < effectsCount; i++)
                {
                    enemy.TakeDamage(damage);

                    for (int j = 0; j < BlocksToAttackCount; j++)
                    {
                        Block block = manager.GetRandomNonDestroyedBlocks(enemyField).FirstOrDefault();
                        if (block != null)
                        {
                            manager.DestroyBlocks(enemyField, new List<Block> { block }, BlockState.DestroyedByDamage);
                            data.Add(ShotData(playerField, enemyField, combo.Blocks.First(), block, -DamageToBlockHealth));
                        }
                    }
                }
            }

            if (combo.Blocks.Count > 3)
            {
                data.AddRange(BlockEffectsHelper.CreateUniqueBlock(manager, random, playerField, player, combo, ComboEffectType));
            }

            return data;
        }
    }
}
