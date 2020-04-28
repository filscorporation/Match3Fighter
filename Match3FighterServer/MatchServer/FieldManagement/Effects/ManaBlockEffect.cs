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

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;

            int effectsCount = Math.Max(1, combo.Blocks.Count - FieldManager.MinComboCount);

            List<EffectData> data = new List<EffectData>();
            data.Add(ManaData(player, ManaToRestore * effectsCount * combo.EffectScale));

            for (int i = 0; i < effectsCount; i++)
            {
                Action(manager, data, player, combo);
            }

            if (combo.Blocks.Count > 3)
            {
                CreateUniqueBlock(manager, playerField, player, combo, ComboEffectType);
            }

            return data;
        }

        private void Action(FieldManager manager, List<EffectData> data, Player player, Combo combo)
        {
            player.GainMana(ManaToRestore * combo.EffectScale);
        }
    }
}
