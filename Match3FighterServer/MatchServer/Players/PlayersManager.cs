using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchServer.Players
{
    public class PlayersManager
    {
        private readonly MatchManager matchManager;

        public Dictionary<string, Player> Players = new Dictionary<string, Player>();

        private readonly Dictionary<int, string> Sessions = new Dictionary<int, string>();

        private readonly List<Player> queue = new List<Player>();

        public PlayersManager(MatchManager matchManager)
        {
            this.matchManager = matchManager;
        }

        /// <summary>
        /// Logs in player
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="playerID"></param>
        public void LogIn(int clientID, string playerID)
        {
            Sessions.Add(clientID, playerID);

            // TODO: temp, players will be stored
            Player player = new Player(clientID);
            player.PlayerID = playerID;
            Players.Add(playerID, player);
        }

        /// <summary>
        /// Logs out player
        /// </summary>
        /// <param name="clientID"></param>
        public void LogOut(int clientID)
        {
            Sessions.Remove(clientID);
        }

        /// <summary>
        /// Returns logged in player by its session
        /// </summary>
        /// <param name="index"></param>
        public Player GetPlayer(int index)
        {
            return Players[Sessions[index]];
        }

        /// <summary>
        /// Adds player to the queue
        /// </summary>
        /// <param name="player"></param>
        public void PutPlayerIntoQueue(Player player)
        {
            if (queue.Contains(player))
                return;
            queue.Add(player);
        }

        /// <summary>
        /// Tries to find pair of players to fight
        /// </summary>
        public void TryMakeMatch()
        {
            // TODO: temp
            if (queue.Count >= 1)
            {
                Player player = queue[0];
                queue.RemoveAt(0);
                matchManager.MakeMatch(player, player);
            }

            if (queue.Count >= 2)
            {
                Player player1 = queue[0];
                Player player2 = queue[1];
                queue.RemoveAt(1);
                queue.RemoveAt(0);
                matchManager.MakeMatch(player1, player2);
            }
        }
    }
}
