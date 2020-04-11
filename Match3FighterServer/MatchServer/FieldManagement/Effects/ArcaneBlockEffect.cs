using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    public class ArcaneBlockEffect : Effect
    {
        public int BlocksToAttackCount = 1;
        public float DamageToBlockHealth = 0.7F;
        public float DamageToEnemyHealth = 10F;
        public float HealthToRestore = 5F;
        public float ManaToRestore = 15F;

        public override BlockTypes ComboEffectType => BlockTypes.Arcane;

        public override EffectData Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, List<Block> combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Player enemy = playerUserIndex == 1 ? match.Player2 : match.Player1;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;
            Field enemyField = playerUserIndex == 1 ? match.Field2 : match.Field1;

            int r = random.Next(0, 3);

            switch (r)
            {
                // Attack and small heal
                case 0:
                    enemy.TakeDamage(DamageToEnemyHealth);
                    for (int i = 0; i < BlocksToAttackCount; i++)
                    {
                        Block block = manager.GetRandomNonDestroyedBlock(enemyField);
                        // TODO: block damage
                        if (block != null)
                            manager.DestroyBlocks(enemyField, new List<Block> { block }, BlockState.DestroyedByDamage);
                    }
                    player.GainHealth(HealthToRestore);
                    break;
                // Attack and mana
                case 1:
                    enemy.TakeDamage(DamageToEnemyHealth);
                    for (int i = 0; i < BlocksToAttackCount; i++)
                    {
                        Block block = manager.GetRandomNonDestroyedBlock(enemyField);
                        // TODO: block damage
                        if (block != null)
                            manager.DestroyBlocks(enemyField, new List<Block> { block }, BlockState.DestroyedByDamage);
                    }
                    player.GainMana(ManaToRestore);
                    break;
                // Heal and mana
                case 2:
                    player.GainHealth(HealthToRestore);
                    player.GainMana(ManaToRestore);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            // TODO: fill data what happend
            EffectData data = new EffectData();

            return data;
        }
    }
}
