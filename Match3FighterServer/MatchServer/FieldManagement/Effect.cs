using System;
using System.Collections.Generic;
using MatchServer.Players;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Some ingame effect after event - deal damage, add health, add mana..
    /// </summary>
    public abstract class Effect
    {
        public abstract BlockTypes ComboEffectType { get; }

        /// <summary>
        /// Applies effect from a combo to the game match
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="random"></param>
        /// <param name="match"></param>
        /// <param name="playerUserIndex"></param>
        /// <param name="combo"></param>
        public abstract EffectData Apply(FieldManager manager, Random random, GameMatch match, int playerUserIndex, List<Block> combo);
    }
}
