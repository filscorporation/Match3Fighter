using System;
using System.Collections.Generic;
using MatchServer.FieldManagement;
using MatchServer.Players;
using NetworkShared.Data.Effects;

namespace MatchServer.SkillsManagement.Skills
{
    public class HealingSkill : Skill
    {
        public float RelativeHealing = 0.15F;

        public override string Name => nameof(HealingSkill);

        public override List<EffectData> Apply(FieldManager manager, SkillsManager skillsManager, Random random, GameMatch match, int playerUserIndex)
        {
            Player player = playerUserIndex == 1 ? match.Player1 : match.Player2;

            List<EffectData> data = new List<EffectData>();

            float healing = RelativeHealing * player.MaxHealth;
            player.GainHealth(healing);
            data.Add(EffectDataHelper.HealthData(player, healing));

            return data;
        }
    }
}
