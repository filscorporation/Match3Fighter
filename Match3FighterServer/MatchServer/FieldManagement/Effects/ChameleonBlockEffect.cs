using System;
using System.Collections.Generic;
using MatchServer.Players;
using MatchServer.UpgradesManagement;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    public class ChameleonBlockEffect : Effect
    {
        public override BlockTypes ComboEffectType => BlockTypes.Chameleon;

        public override List<EffectData> Apply(FieldManager manager, UpgradeManager upgradeManager, Random random, GameMatch match, int playerUserIndex, Combo combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;

            List<EffectData> data = new List<EffectData>();

            if (combo.Blocks.Count > 3)
            {
                data.AddRange(BlockEffectsHelper.CreateRandomUniqueBlock(manager, random, playerField, player, combo, 6));
            }

            return data;
        }
    }
}
