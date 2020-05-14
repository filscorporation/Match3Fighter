using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchServer.FieldManagement;
using MatchServer.UpgradesManagement;

namespace MatchServer.Players
{
    public class MatchManager
    {
        private FieldManager fieldManager;

        public List<GameMatch> Matches = new List<GameMatch>();

        public MatchManager(FieldManager fieldManager)
        {
            this.fieldManager = fieldManager;
        }

        /// <summary>
        /// Begins fight between two players
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public void MakeMatch(Player player1, Player player2)
        {
            GameMatch match = new GameMatch();
            match.Player1 = player1;
            player1.CurrentMatch = match;
            player1.InGameID = 1;
            player1.Refresh();
            match.Player2 = player2;
            player2.CurrentMatch = match;
            player2.InGameID = 2;
            player2.Refresh();
            match.Player1Upgrades = new UpgradesInfo();
            match.Player2Upgrades = new UpgradesInfo();

            match.Field1 = fieldManager.GenerateFieldForPlayer(player1);
            match.Field1.InGameID = 1;
            match.Field2 = fieldManager.GenerateFieldForPlayer(player2);
            match.Field1.InGameID = 2;

            Matches.Add(match);

            GameCore.Instance.StartMatch(match);
        }

        /// <summary>
        /// Ends match
        /// </summary>
        /// <param name="match"></param>
        public void DropMatch(GameMatch match)
        {
            Console.WriteLine($"Dropping match {match}");
            if (match.Player1 != null) match.Player1.CurrentMatch = null;
            if (match.Player2 != null) match.Player2.CurrentMatch = null;

            Matches.Remove(match);
        }
    }
}
