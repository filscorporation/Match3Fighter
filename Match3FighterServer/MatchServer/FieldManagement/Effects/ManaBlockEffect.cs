using System;
using System.Collections.Generic;
using MatchServer.Players;
using MatchServer.UpgradesManagement;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    public class ManaBlockEffect : Effect
    {
        public float ManaToRestore = 40F;

        public override BlockTypes ComboEffectType => BlockTypes.Mana;

        public override List<EffectData> Apply(FieldManager manager, UpgradeManager upgradeManager, Random random, GameMatch match, int playerUserIndex, Combo combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;
            UpgradesInfo playerUpgradesInfo = playerUserIndex == 1 ? match.Player1Upgrades : match.Player2Upgrades;

            int effectsCount = Math.Max(1, combo.Blocks.Count - FieldManager.MinComboCount);

            List<EffectData> data = new List<EffectData>();
            float mana = ManaToRestore * combo.EffectScale
                         * upgradeManager.GetManaBlockUpgradeBonus(playerUpgradesInfo);
            data.Add(ManaData(player, mana * effectsCount));

            for (int i = 0; i < effectsCount; i++)
            {
                player.GainMana(mana);
            }

            if (combo.Blocks.Count > 3)
            {
                data.AddRange(BlockEffectsHelper.CreateUniqueBlock(manager, random, playerField, player, combo, ComboEffectType));
            }

            return data;
        }
    }
}
