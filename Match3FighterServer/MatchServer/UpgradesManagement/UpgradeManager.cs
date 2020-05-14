using MatchServer.Players;
using NetworkShared.Data.Field;

namespace MatchServer.UpgradesManagement
{
    public enum UpgradeRequestResponse
    {
        Ok,
        NotEnoughMana,
    }

    /// <summary>
    /// Controls upgrades in game match
    /// </summary>
    public class UpgradeManager
    {
        private const int baseUpgradeCost = 10;
        private const int upgradeCostStep = 10;

        public float AttackBlockUpgradeBonus = 0.1F;
        public float HealthBlockUpgradeBonus = 0.2F;
        public float ManaBlockUpgradeBonus = 0.1F;
        public float ArcaneBlockUpgradeDamageBonus = 0.05F;
        public float ArcaneBlockUpgradeHealBonus = 0.1F;
        public float ArcaneBlockUpgradeManaBonus = 0.2F;

        /// <summary>
        /// Tries to process upgrade
        /// </summary>
        /// <returns></returns>
        public UpgradeRequestResponse ProcessUpgradeRequest(GameMatch match, Player player, BlockTypes upgradeBlockType)
        {
            UpgradesInfo info = player == match.Player1 ? match.Player1Upgrades : match.Player2Upgrades;
            int upgradeCost = baseUpgradeCost + info.UpgradesCount[upgradeBlockType] * upgradeCostStep;
            if (!player.TrySpendMana(upgradeCost))
            {
                return UpgradeRequestResponse.NotEnoughMana;
            }

            info.UpgradesCount[upgradeBlockType]++;

            return UpgradeRequestResponse.Ok;
        }

        public float GetAttackBlockUpgradeBonus(UpgradesInfo info)
        {
            return 1 + info.UpgradesCount[BlockTypes.Attack] * AttackBlockUpgradeBonus;
        }

        public float GetHealthBlockUpgradeBonus(UpgradesInfo info)
        {
            return 1 + info.UpgradesCount[BlockTypes.Health] * HealthBlockUpgradeBonus;
        }

        public float GetManaBlockUpgradeBonus(UpgradesInfo info)
        {
            return 1 + info.UpgradesCount[BlockTypes.Mana] * ManaBlockUpgradeBonus;
        }

        public float GetArcaneBlockUpgradeDamageBonus(UpgradesInfo info)
        {
            return 1 + info.UpgradesCount[BlockTypes.Arcane] * ArcaneBlockUpgradeDamageBonus;
        }

        public float GetArcaneBlockUpgradeHealBonus(UpgradesInfo info)
        {
            return 1 + info.UpgradesCount[BlockTypes.Arcane] * ArcaneBlockUpgradeHealBonus;
        }

        public float GetArcaneBlockUpgradeManaBonus(UpgradesInfo info)
        {
            return 1 + info.UpgradesCount[BlockTypes.Arcane] * ArcaneBlockUpgradeManaBonus;
        }
    }
}
