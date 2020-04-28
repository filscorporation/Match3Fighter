using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class ShieldBlock : UniqueBlock
    {
        public override string Name => nameof(ShieldBlock);

        public override BlockTypes BaseType => BlockTypes.Health;

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;
            List<EffectData> data = new List<EffectData>();

            GlobalEffect globalEffect = new GlobalEffect(GlobalEffectType.Shield);
            playerField.GlobalEffects.Add(globalEffect);
            data.Add(GlobalEffectData(player, globalEffect));

            return data;
        }
    }
}
