using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    public class ChameleonBlockEffect : Effect
    {
        public override BlockTypes ComboEffectType => BlockTypes.Chameleon;

        public override EffectData Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, List<Block> combo)
        {
            Player enemy = playerUserIndex == 1 ? match.Player2 : match.Player1;

            // TODO: fill data what happend
            EffectData data = new EffectData();

            return data;
        }
    }
}
