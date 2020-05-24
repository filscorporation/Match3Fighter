using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class ArcanePlusBlock : UniqueBlock
    {
        public float EffectScale = 1.5F;

        public override string Name => nameof(ArcanePlusBlock);

        public override int Level => 3;

        public override BlockTypes BaseType => BlockTypes.Arcane;

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            List<EffectData> data = new List<EffectData>();

            combo.EffectScale *= EffectScale;

            return data;
        }
    }
}
