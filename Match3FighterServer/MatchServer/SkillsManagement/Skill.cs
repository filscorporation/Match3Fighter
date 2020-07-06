using System;
using System.Collections.Generic;
using MatchServer.FieldManagement;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Player;

namespace MatchServer.SkillsManagement
{
    /// <summary>
    /// Players usable ingame ability
    /// </summary>
    public abstract class Skill
    {
        public abstract string Name { get; }

        /// <summary>
        /// Applies skill effect to the game match
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="skillsManager"></param>
        /// <param name="random"></param>
        /// <param name="match"></param>
        /// <param name="playerUserIndex"></param>
        public abstract List<EffectData> Apply(FieldManager manager, SkillsManager skillsManager, Random random, GameMatch match, int playerUserIndex);

        public SkillData ToData()
        {
            return new SkillData
            {
                Name = Name,
            };
        }
    }
}
