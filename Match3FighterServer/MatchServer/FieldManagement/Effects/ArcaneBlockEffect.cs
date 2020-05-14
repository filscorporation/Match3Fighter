using System;
using System.Collections.Generic;
using MatchServer.Players;
using MatchServer.UpgradesManagement;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    public class ArcaneBlockEffect : Effect
    {
        public float DamageToEnemyHealth = 10F;
        public float HealthToRestore = 10F;
        public float ManaToRestore = 15F;

        public override BlockTypes ComboEffectType => BlockTypes.Arcane;

        public override List<EffectData> Apply(FieldManager manager, UpgradeManager upgradeManager, Random random, GameMatch match, int playerUserIndex, Combo combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Player enemy = playerUserIndex == 1 ? match.Player2 : match.Player1;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;
            Field enemyField = playerUserIndex == 1 ? match.Field2 : match.Field1;
            UpgradesInfo playerUpgradesInfo = playerUserIndex == 1 ? match.Player1Upgrades : match.Player2Upgrades;

            List<EffectData> data = new List<EffectData>();

            int effectsCount = Math.Max(1, combo.Blocks.Count - FieldManager.MinComboCount);
            for (int i = 0; i < effectsCount; i++)
            {
                Action(manager, upgradeManager, data, random, player, playerField, enemy, enemyField, playerUpgradesInfo, combo);
            }

            if (combo.Blocks.Count > 3)
            {
                BlockEffectsHelper.CreateUniqueBlock(manager, playerField, player, combo, ComboEffectType);
            }

            return data;
        }

        private void Action(
            FieldManager manager,
            UpgradeManager upgradeManager,
            List<EffectData> data, Random random,
            Player player,
            Field playerField,
            Player enemy,
            Field enemyField,
            UpgradesInfo playerUpgradesInfo,
            Combo combo)
        {
            int r = random.Next(0, 3);

            float damage = DamageToEnemyHealth * combo.EffectScale
                        * upgradeManager.GetArcaneBlockUpgradeDamageBonus(playerUpgradesInfo);
            float health = HealthToRestore * combo.EffectScale
                        * upgradeManager.GetArcaneBlockUpgradeHealBonus(playerUpgradesInfo);
            float mana = ManaToRestore * combo.EffectScale
                        * upgradeManager.GetArcaneBlockUpgradeManaBonus(playerUpgradesInfo);

            switch (r)
            {
                // Attack and small heal
                case 0:
                    enemy.TakeDamage(damage);
                    data.Add(HealthData(enemy, -damage));
                    player.GainHealth(health);
                    data.Add(HealthData(player, health));
                    break;
                // Attack and mana
                case 1:
                    enemy.TakeDamage(damage);
                    data.Add(HealthData(enemy, -damage));
                    player.GainMana(mana);
                    data.Add(ManaData(player, mana));
                    break;
                // Heal and mana
                case 2:
                    player.GainHealth(health);
                    data.Add(HealthData(player, health));
                    player.GainMana(mana);
                    data.Add(ManaData(player, mana));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
