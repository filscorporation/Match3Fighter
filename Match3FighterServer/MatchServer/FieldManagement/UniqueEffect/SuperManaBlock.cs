using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class SuperManaBlock : UniqueBlock
    {
        public override string Name => nameof(SuperManaBlock);

        public override BlockTypes BaseType => BlockTypes.Mana;

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;

            List<EffectData> data = new List<EffectData>();

            player.GainMana(player.MaxMana);
            data.Add(ManaData(player, player.MaxMana));

            return data;
        }
    }
}
