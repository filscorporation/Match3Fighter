using System;
using System.Collections.Generic;
using MatchServer.FieldManagement;
using MatchServer.Players;
using NetworkShared.Data.Effects;

namespace MatchServer.SkillsManagement.Skills
{
    public class LightningSkill : Skill
    {
        public float RelativeDamageToEnemyHealth = 0.15F;

        public override string Name => nameof(LightningSkill);

        public override List<EffectData> Apply(FieldManager manager, SkillsManager skillsManager, Random random, GameMatch match, int playerUserIndex)
        {
            Player enemy = playerUserIndex == 1 ? match.Player2 : match.Player1;

            List<EffectData> data = new List<EffectData>();

            float damage = RelativeDamageToEnemyHealth * enemy.Health;
            enemy.TakeDamage(damage);
            data.Add(EffectDataHelper.HealthData(enemy, -damage));

            return data;
        }
    }
}
