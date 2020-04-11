using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.Effects
{
    public class HealthBlockEffect : Effect
    {
        public int BlocksToGainArmourCount = 9;
        public float BlockArmourToGain = 0.2F;
        public float HealthToRestore = 10F;

        public override BlockTypes ComboEffectType => BlockTypes.Health;

        public override EffectData Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, List<Block> combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;

            player.GainHealth(HealthToRestore);

            for (int i = 0; i < BlocksToGainArmourCount; i++)
            {
                Block block = manager.GetRandomNonDestroyedBlock(playerField);
                // TODO: block armour

            }

            // TODO: fill data what happend
            EffectData data = new EffectData();

            return data;
        }
    }
}
