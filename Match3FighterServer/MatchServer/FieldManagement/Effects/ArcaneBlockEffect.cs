using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    public class ArcaneBlockEffect : Effect
    {
        public int BlocksToAttackCount = 1;
        public float DamageToBlockHealth = 0.7F;
        public float DamageToEnemyHealth = 5F;
        public float HealthToRestore = 5F;
        public float ManaToRestore = 15F;

        public override BlockTypes ComboEffectType => BlockTypes.Arcane;

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, List<Block> combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Player enemy = playerUserIndex == 1 ? match.Player2 : match.Player1;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;
            Field enemyField = playerUserIndex == 1 ? match.Field2 : match.Field1;

            // TODO: fill data what happend
            List<EffectData> data = new List<EffectData>();

            int effectsCount = Math.Max(1, combo.Count - FieldManager.MinComboCount);
            for (int i = 0; i < effectsCount; i++)
            {
                Action(manager, data, random, player, playerField, enemy, enemyField);
            }

            if (combo.Count > 3)
            {
                manager.CreateBlockInRange(playerField, BlockTypes.Chameleon, combo);
            }

            return data;
        }

        private void Action(FieldManager manager, List<EffectData> data, Random random, Player player, Field playerField, Player enemy, Field enemyField)
        {
            int r = random.Next(0, 3);

            switch (r)
            {
                // Attack and small heal
                case 0:
                    enemy.TakeDamage(DamageToEnemyHealth);
                    data.Add(HealthData(enemy, -DamageToEnemyHealth));
                    player.GainHealth(HealthToRestore);
                    data.Add(HealthData(player, HealthToRestore));
                    break;
                // Attack and mana
                case 1:
                    enemy.TakeDamage(DamageToEnemyHealth);
                    data.Add(HealthData(enemy, -DamageToEnemyHealth));
                    player.GainMana(ManaToRestore);
                    data.Add(ManaData(player, ManaToRestore));
                    break;
                // Heal and mana
                case 2:
                    player.GainHealth(HealthToRestore);
                    data.Add(HealthData(player, HealthToRestore));
                    player.GainMana(ManaToRestore);
                    data.Add(ManaData(player, ManaToRestore));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
