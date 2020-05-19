using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MatchServer.FieldManagement;
using NetworkShared.Data.Field;
using NetworkShared.Data.Player;

namespace MatchServer.Players
{
    public class PlayersManager
    {
        private readonly MatchManager matchManager;

        public ConcurrentDictionary<string, Player> Players = new ConcurrentDictionary<string, Player>();

        private readonly ConcurrentDictionary<int, string> Sessions = new ConcurrentDictionary<int, string>();

        private readonly ConcurrentDictionary<int, Player> queue = new ConcurrentDictionary<int, Player>();

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
            queue.TryRemove(clientID, out _);
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
            if (player.IsInDebugMode)
            {
                queue.TryRemove(player.ClientID, out _);
                matchManager.MakeMatch(player, player);
            }
            else
            {
                if (queue.ContainsKey(player.ClientID))
                    return;

                if (!queue.TryAdd(player.ClientID, player))
                    throw new Exception("Error adding player into queue");
            }
        }

        /// <summary>
        /// Tries to find pair of players to fight
        /// </summary>
        public void TryMakeMatch()
        {
            if (queue.Count >= 2)
            {
                queue.TryRemove(queue.First().Key, out Player player1);
                queue.TryRemove(queue.First().Key, out Player player2);
                matchManager.MakeMatch(player1, player2);
            }
        }

        /// <summary>
        /// Validate and set player stats
        /// </summary>
        /// <param name="player"></param>
        /// <param name="data"></param>
        public void TrySetPlayerStats(Player player, PlayerStatsData data)
        {
            Dictionary<string, UniqueBlock> uBlocks = GameCore.Instance.BlockEffectsManager.UniqueBlocks;

            foreach (KeyValuePair<BlockTypes, UniqueBlockData> pair in data.UniqueBlockCollection.Level1Blocks)
            {
                if (pair.Value.Level != 1)
                    throw new NotSupportedException();

                if (player.UniqueBlockCollection.Collection.All(b => b.Name != pair.Value.Name))
                    throw new NotSupportedException();

                player.UniqueBlockCollection.Level1Blocks[pair.Key] = uBlocks[pair.Value.Name];
            }

            foreach (KeyValuePair<BlockTypes, UniqueBlockData> pair in data.UniqueBlockCollection.Level2Blocks)
            {
                if (pair.Value.Level != 2)
                    throw new NotSupportedException();

                if (player.UniqueBlockCollection.Collection.All(b => b.Name != pair.Value.Name))
                    throw new NotSupportedException();

                player.UniqueBlockCollection.Level2Blocks[pair.Key] = uBlocks[pair.Value.Name];
            }

            foreach (KeyValuePair<BlockTypes, UniqueBlockData> pair in data.UniqueBlockCollection.Level3Blocks)
            {
                if (pair.Value.Level != 3)
                    throw new NotSupportedException();

                if (player.UniqueBlockCollection.Collection.All(b => b.Name != pair.Value.Name))
                    throw new NotSupportedException();

                player.UniqueBlockCollection.Level3Blocks[pair.Key] = uBlocks[pair.Value.Name];
            }
        }
    }
}
