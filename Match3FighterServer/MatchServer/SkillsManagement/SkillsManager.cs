using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MatchServer.FieldManagement;
using MatchServer.Players;
using NetworkShared.Data.Effects;

namespace MatchServer.SkillsManagement
{
    /// <summary>
    /// Manages all available players skills and their effects
    /// </summary>
    public class SkillsManager
    {
        private readonly FieldManager fieldManager;

        private readonly Random random;

        public static readonly Dictionary<string, Skill> Skills = new Dictionary<string, Skill>();

        public SkillsManager(FieldManager fieldManager)
        {
            this.fieldManager = fieldManager;

            random = new Random();

            InitializeSkills();
        }

        private void InitializeSkills()
        {
            foreach (Type type in Assembly.GetAssembly(typeof(Skill)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Skill))))
            {
                Skill skill = (Skill)Activator.CreateInstance(type);
                Skills[skill.Name] = skill;
            }
        }

        /// <summary>
        /// Returns effect the skill
        /// </summary>
        /// <param name="match"></param>
        /// <param name="playerUserIndex"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        public List<EffectData> ApplySkillEffect(GameMatch match, int playerUserIndex, string skill)
        {
            List<EffectData> data = new List<EffectData>();

            data.AddRange(Skills[skill].Apply(fieldManager, this, random, match, playerUserIndex));

            return data;
        }
    }
}
