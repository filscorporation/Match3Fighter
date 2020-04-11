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
            Field playerField = playerUserIndex == 1 ? match.Field1 : match.Field2;

            // TODO: fill data what happend
            EffectData data = new EffectData();

            int effectsCount = Math.Max(1, combo.Count - FieldManager.MinComboCount);
            for (int i = 0; i < effectsCount; i++)
            {
                Action(manager, data, player);
            }

            if (combo.Count > 3)
            {
                manager.CreateBlockInRange(playerField, BlockTypes.Chameleon, combo);
            }

            return data;
        }

        private void Action(FieldManager manager, EffectData data, Player player)
        {
            player.GainMana(ManaToRestore);
        }
    }
}
