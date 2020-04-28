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

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;

            int effectsCount = Math.Max(1, combo.Blocks.Count - FieldManager.MinComboCount);

            List<EffectData> data = new List<EffectData>();
            data.Add(HealthData(player, HealthToRestore * effectsCount * combo.EffectScale));

            for (int i = 0; i < effectsCount; i++)
            {
                Action(manager, data, player, playerField, combo);
            }

            if (combo.Blocks.Count > 3)
            {
                CreateUniqueBlock(manager, playerField, player, combo, ComboEffectType);
            }

            return data;
        }

        private void Action(FieldManager manager, List<EffectData> data, Player player, Field playerField, Combo combo)
        {
            player.GainHealth(HealthToRestore * combo.EffectScale);

            for (int i = 0; i < BlocksToGainArmourCount; i++)
            {
                Block block = manager.GetRandomNonDestroyedBlock(playerField);
                // TODO: block armour

            }
        }
    }
}
