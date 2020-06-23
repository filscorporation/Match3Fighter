using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class MassFlipBlock : UniqueBlock
    {
        public int FlipAmount = 7;

        public override string Name => nameof(MassFlipBlock);

        public override int Level => 2;

        public override BlockTypes BaseType => BlockTypes.Arcane;

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            Field enemyField = playerUserIndex == 1 ? match.Field2 : match.Field1;

            List<EffectData> data = new List<EffectData>();

            manager.FlipBlocks(enemyField, manager.GetRandomNonDestroyedBlocks(enemyField, FlipAmount));

            return data;
        }
    }
}
