using System;
using MatchServer.FieldManagement;
using MatchServer.UpgradesManagement;
using NetworkShared.Data;

namespace MatchServer.Players
{
    /// <summary>
    /// Info about players match
    /// </summary>
    public class GameMatch
    {
        public GameMatch(GameMode gameMode)
        {
            ID = Guid.NewGuid();
            GameMode = gameMode;
        }

        public Guid ID;

        public GameMode GameMode;

        public Player Player1;

        public Player Player2;

        public Field Field1;

        public Field Field2;

        public UpgradesInfo Player1Upgrades;

        public UpgradesInfo Player2Upgrades;
    }
}
