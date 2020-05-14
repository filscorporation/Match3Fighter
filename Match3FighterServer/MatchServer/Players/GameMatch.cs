using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchServer.FieldManagement;
using MatchServer.UpgradesManagement;

namespace MatchServer.Players
{
    /// <summary>
    /// Info about players match
    /// </summary>
    public class GameMatch
    {
        public Player Player1;

        public Player Player2;

        public Field Field1;

        public Field Field2;

        public UpgradesInfo Player1Upgrades;

        public UpgradesInfo Player2Upgrades;
    }
}
