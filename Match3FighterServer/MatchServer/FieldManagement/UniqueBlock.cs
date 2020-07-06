using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Unique block that drops from 4-5-6 combos
    /// </summary>
    public abstract class UniqueBlock
    {
        public abstract string Name { get; }

        public abstract int Level { get; }

        public abstract BlockTypes BaseType { get; }

        public GlobalEffect AttachedGlobalEffect;

        /// <summary>
        /// Applies unique block effect on its creation
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="random"></param>
        /// <param name="match"></param>
        /// <param name="user"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public virtual List<EffectData> OnCreate(FieldManager manager, Random random, GameMatch match, Player user, Block block)
        {
            return new List<EffectData>();
        }

        /// <summary>
        /// Applies unique block effect on its destruction
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="random"></param>
        /// <param name="match"></param>
        /// <param name="user"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public virtual List<EffectData> OnDelete(FieldManager manager, Random random, GameMatch match, Player user, Block block)
        {
            return new List<EffectData>();
        }

        /// <summary>
        /// Applies effect from a combo to the game match
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="random"></param>
        /// <param name="match"></param>
        /// <param name="playerUserIndex"></param>
        /// <param name="combo"></param>
        /// <param name="block"></param>
        public abstract List<EffectData> Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, Combo combo, Block block);

        public UniqueBlockData ToData()
        {
            return new UniqueBlockData
            {
                Name = Name,
                Level = Level,
                Type = BaseType,
            };
        }
    }
}
