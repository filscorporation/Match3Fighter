using System;
using System.Collections.Generic;
using System.Linq;
using MatchServer.Players;
using MatchServer.UpgradesManagement;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;

namespace MatchServer.FieldManagement
{
    /// <summary>
    /// Some in game effect after event - deal damage, add health, add mana..
    /// </summary>
    public abstract class Effect
    {
        public abstract BlockTypes ComboEffectType { get; }

        /// <summary>
        /// Applies effect from a combo to the game match
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="upgradeManager"></param>
        /// <param name="random"></param>
        /// <param name="match"></param>
        /// <param name="playerUserIndex"></param>
        /// <param name="combo"></param>
        public abstract List<EffectData> Apply(FieldManager manager, UpgradeManager upgradeManager, Random random, GameMatch match, int playerUserIndex, Combo combo);
    }
}
