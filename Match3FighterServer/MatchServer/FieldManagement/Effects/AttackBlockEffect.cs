using System;
using System.Collections.Generic;
using System.Linq;
using MatchServer.Players;
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

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, List<Block> combo)
        {
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;
            Player enemy = playerUserIndex == 1 ? match.Player2 : match.Player1;
            Field enemyField = playerUserIndex == 1 ? match.Field2 : match.Field1;

            int effectsCount = Math.Max(1, combo.Count - FieldManager.MinComboCount);

            List<EffectData> data = new List<EffectData>();
            data.Add(HealthData(enemy, -DamageToEnemyHealth * effectsCount));

            for (int i = 0; i < effectsCount; i++)
            {
                Action(manager, data, playerField, enemy, enemyField, combo.First());
            }

            if (combo.Count > 3)
            {
                manager.CreateBlockInRange(playerField, BlockTypes.Chameleon, combo);
            }

            return data;
        }

        private void Action(FieldManager manager, List<EffectData> data, Field playerField, Player enemy, Field enemyField, Block init)
        {
            enemy.TakeDamage(DamageToEnemyHealth);

            for (int i = 0; i < BlocksToAttackCount; i++)
            {
                Block block = manager.GetRandomNonDestroyedBlock(enemyField);
                // TODO: block damage
                if (block != null)
                {
                    manager.DestroyBlocks(enemyField, new List<Block> {block}, BlockState.DestroyedByDamage);
                    data.Add(ShotData(playerField, enemyField, init, block, -DamageToBlockHealth));
                }
            }
        }
    }
}
