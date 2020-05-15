using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class Arcane5Block : UniqueBlock
    {
        public float EffectScale = 1.2F;

        public override string Name => nameof(Arcane5Block);

        public override int Level => 2;

        public override BlockTypes BaseType => BlockTypes.Arcane;

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            List<EffectData> data = new List<EffectData>();

            combo.EffectScale *= EffectScale;

            return data;
        }
    }
}
