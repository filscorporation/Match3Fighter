using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Source.GameManagement;
using Assets.Source.UIManagement;
using NetworkShared.Data.Field;
using NetworkShared.Data.Upgrades;
using UnityEngine;

namespace Assets.Source.UpgradeManagement
{
    /// <summary>
    /// Controls battle block upgrades interactions
    /// </summary>
    public class UpgradeManager : MonoBehaviour
    {
        private static UpgradeManager instance;

        public static UpgradeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UpgradeManager>();
                }
                return instance;
            }
        }

        private int upgradeBaseCost = 10;
        private int upgradeCostStep = 10;

        /// <summary>
        /// Sets base constants used to calculate upgrade parameters
        /// </summary>
        /// <param name="baseCost"></param>
        /// <param name="step"></param>
        public void SetUpgradeBaseConstants(int baseCost, int step)
        {
            upgradeBaseCost = baseCost;
            upgradeCostStep = step;
        }

        private int GetCost(int level) => upgradeBaseCost + level * upgradeCostStep;

        /// <summary>
        /// Applies received upgrade state data to show on UI
        /// </summary>
        /// <param name="info"></param>
        public void ApplyUpgradeInfo(UpgradesInfoData info)
        {
            UIManager.Instance.SetAttackUpgradeCost(GetCost(info.UpgradesCount[BlockTypes.Attack]));
            UIManager.Instance.SetHealthUpgradeCost(GetCost(info.UpgradesCount[BlockTypes.Health]));
            UIManager.Instance.SetManaUpgradeCost(GetCost(info.UpgradesCount[BlockTypes.Mana]));
            UIManager.Instance.SetArcaneUpgradeCost(GetCost(info.UpgradesCount[BlockTypes.Arcane]));
        }

        public bool AttackBlockUpgrade()
        {
            GameManager.Instance.OnPlayerUpgrade(BlockTypes.Attack);

            return true;
        }

        public bool HealthBlockUpgrade()
        {
            GameManager.Instance.OnPlayerUpgrade(BlockTypes.Health);

            return true;
        }

        public bool ManaBlockUpgrade()
        {
            GameManager.Instance.OnPlayerUpgrade(BlockTypes.Mana);

            return true;
        }

        public bool ArcaneBlockUpgrade()
        {
            GameManager.Instance.OnPlayerUpgrade(BlockTypes.Arcane);

            return true;
        }
    }
}
