using System;
using System.Collections.Generic;
using System.Linq;
using MatchServer.Players;
using MatchServer.UpgradesManagement;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    public class HealthBlockEffect : Effect
    {
        public int BlocksToGainArmourCount = 9;
        public float BlockArmourToGain = 0.2F;
        public float HealthToRestore = 10F;

        public override BlockTypes ComboEffectType => BlockTypes.Health;

        public override List<EffectData> Apply(FieldManager manager, UpgradeManager upgradeManager, Random random, GameMatch match, int playerUserIndex, Combo combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;
            UpgradesInfo playerUpgradesInfo = playerUserIndex == 1 ? match.Player1Upgrades : match.Player2Upgrades;

            int effectsCount = Math.Max(1, combo.Blocks.Count - FieldManager.MinComboCount);

            List<EffectData> data = new List<EffectData>();
            float health = HealthToRestore * combo.EffectScale
                           * upgradeManager.GetHealthBlockUpgradeBonus(playerUpgradesInfo);
            data.Add(HealthData(player, health * effectsCount));

            for (int i = 0; i < effectsCount; i++)
            {
                player.GainHealth(health);
            }

            if (combo.Blocks.Count > 3)
            {
                data.AddRange(BlockEffectsHelper.CreateUniqueBlock(manager, random, playerField, player, combo, ComboEffectType));
            }

            return data;
        }
    }
}
