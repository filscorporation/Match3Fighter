using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class LifeBlock : UniqueBlock
    {
        public float EffectScale = 1.5F;

        public override string Name => nameof(LifeBlock);

        public override BlockTypes BaseType => BlockTypes.Health;

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            List<EffectData> data = new List<EffectData>();

            combo.EffectScale *= EffectScale;

            return data;
        }
    }
}
