using System;
using System.Collections.Generic;
using System.Linq;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement.UniqueEffect
{
    public class InfinityBlock : UniqueBlock
    {
        public override string Name => nameof(InfinityBlock);
        public override BlockTypes BaseType => BlockTypes.Health;

        public float HealOverTimeAmount = 1;

        public override List<EffectData> OnCreate(FieldManager manager, Random random, GameMatch match, Player user, Block block)
        {
            Field playerField = user.InGameID == 1 ? match.Field1 : match.Field2;
            List<EffectData> data = base.OnCreate(manager, random, match, user, block);

            GlobalEffect globalEffect = new GlobalEffect(GlobalEffectType.HealOverTime, HealOverTimeAmount);
            AttachedGlobalEffect = globalEffect;
            playerField.GlobalEffects.Add(globalEffect);
            data.Add(GlobalEffectData(user, globalEffect));

            return data;
        }

        public override List<EffectData> OnDelete(FieldManager manager, Random random, GameMatch match, Player user, Block block)
        {
            Field playerField = user.InGameID == 1 ? match.Field1 : match.Field2;
            Player player = user.InGameID == 1 ? match.Player1 : match.Player2;
            List<EffectData> data = base.OnDelete(manager, random, match, user, block);

            playerField.GlobalEffects.Remove(AttachedGlobalEffect);
            data.Add(GlobalEffectRemovedData(player, AttachedGlobalEffect));

            return data;
        }

        public override List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block)
        {
            return new List<EffectData>();
        }
    }
}
