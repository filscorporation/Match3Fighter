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
        public int EnergyFromOneBlock = 1;

        public override BlockTypes ComboEffectType => BlockTypes.Arcane;

        public override List<EffectData> Apply(FieldManager manager, UpgradeManager upgradeManager, Random random, GameMatch match, int playerUserIndex, Combo combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;

            List<EffectData> data = new List<EffectData>();

            int enegryToGain = combo.Blocks.Count * EnergyFromOneBlock;
            player.GainEnergy(enegryToGain);

            data.Add(EffectDataHelper.EnergyData(player, enegryToGain));

            return data;
        }
    }
}
