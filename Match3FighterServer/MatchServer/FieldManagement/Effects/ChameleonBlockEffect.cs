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

            if (combo.Blocks.Count > 3)
            {
                BlockEffectsHelper.CreateRandomUniqueBlock(manager, random, playerField, player, combo, 6);
            }

            return new List<EffectData>();
        }
    }
}
