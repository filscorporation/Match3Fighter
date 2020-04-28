using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class ChameleonBlock : UniqueBlock
    {
        public override string Name => nameof(ChameleonBlock);

        public override BlockTypes BaseType => BlockTypes.Chameleon;

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            return new List<EffectData>();
        }
    }
}
