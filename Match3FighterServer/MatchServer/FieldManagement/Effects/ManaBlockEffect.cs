using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    public class ManaBlockEffect : Effect
    {
        public float ManaToRestore = 40F;

        public override BlockTypes ComboEffectType => BlockTypes.Mana;

        public override EffectData Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, List<Block> combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;

            player.GainMana(ManaToRestore);

            // TODO: fill data what happend
            EffectData data = new EffectData();

            return data;
        }
    }
}
