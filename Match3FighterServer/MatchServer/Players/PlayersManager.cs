using System.Collections.Concurrent;
using System.Linq;

namespace MatchServer.Players
{
    public class PlayersManager
    {
        private readonly MatchManager matchManager;

        public ConcurrentDictionary<string, Player> Players = new ConcurrentDictionary<string, Player>();

        private readonly ConcurrentDictionary<int, string> Sessions = new ConcurrentDictionary<int, string>();

        private readonly ConcurrentBag<Player> queue = new ConcurrentBag<Player>();

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
            Sessions.TryAdd(clientID, playerID);

            // TODO: temp, players will be stored
            Player player = new Player(clientID);
            player.PlayerID = playerID;
            Players.TryAdd(playerID, player);
        }

        /// <summary>
        /// Logs out player
        /// </summary>
        /// <param name="clientID"></param>
        public void LogOut(int clientID)
        {
            Sessions.TryRemove(clientID, out _);
        }

        /// <summary>
        /// Returns logged in player by its session
        /// </summary>
        /// <param name="index"></param>
        public Player GetPlayer(int index)
        {
            if (Sessions.TryGetValue(index, out string key)
                && Players.TryGetValue(key, out Player player))
                return player;
                
            return null;
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
            if (GameCore.AllowOnePlayerMode && queue.Count >= 1)
            {
                queue.TryTake(out Player player);
                matchManager.MakeMatch(player, player);
            }

            if (queue.Count >= 2)
            {
                queue.TryTake(out Player player1);
                queue.TryTake(out Player player2);
                matchManager.MakeMatch(player1, player2);
            }
        }
    }
}
