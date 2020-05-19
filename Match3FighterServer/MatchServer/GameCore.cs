using System;
using System.Collections.Generic;
using System.Threading;
using MatchServer.FieldManagement;
using MatchServer.Players;
using MatchServer.UpgradesManagement;
using NetworkShared.Core;
using NetworkShared.Data;
using NetworkShared.Data.Effects;
using NetworkShared.Data.Field;
using NetworkShared.Data.Player;
using NetworkShared.Network;

namespace MatchServer
{
    /// <summary>
    /// Controls main game loop and all ingame components connections
    /// </summary>
    public class GameCore : IServerListener
    {
        public static GameCore Instance;

        public bool IsRunning = false;
        public const int TicksPerSec = 30;
        public const int MsPerTick = 1000 / TicksPerSec;

        public Server Server;
        public FieldManager FieldManager;
        public UpgradeManager UpgradeManager;
        public MatchManager MatchManager;
        public PlayersManager PlayersManager;
        public BlockEffectsManager BlockEffectsManager;

        #region Core

        public void Initialize()
        {
            if (Instance != null)
                throw new Exception("GameCore instance already exists");
            Instance = this;

            FieldManager = new FieldManager();
            UpgradeManager = new UpgradeManager();
            MatchManager = new MatchManager(FieldManager);
            PlayersManager = new PlayersManager(MatchManager);
            BlockEffectsManager = new BlockEffectsManager(FieldManager, UpgradeManager);
        }

        public void Start()
        {
            Console.WriteLine($"Game thread started. Running at {TicksPerSec} ticks per second.");
            DateTime nextLoop = DateTime.Now;

            IsRunning = true;

            while (IsRunning)
            {
                while (nextLoop < DateTime.Now)
                {
                    Update();

                    nextLoop = nextLoop.AddMilliseconds(MsPerTick);

                    if (nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }

        private void Update()
        {
            ThreadManager.UpdateMain();

            PlayersManager.TryMakeMatch();
        }

        #endregion

        #region Clients

        /// <summary>
        /// Client was connected
        /// </summary>
        /// <param name="clientID"></param>
        public void ClientConnected(int clientID)
        {
            // TODO: device ID from client
            string playerID = Guid.NewGuid().ToString();

            PlayersManager.LogIn(clientID, playerID);

            ConnectResponse response = new ConnectResponse();
            Server.SendDataToClient(clientID, (int)DataTypes.ConnectResponse, response);
        }

        /// <summary>
        /// Client was disconnected
        /// </summary>
        /// <param name="clientID"></param>
        public void ClientDisconnected(int clientID)
        {
            Player player = PlayersManager.GetPlayer(clientID);
            if (player.CurrentMatch != null)
                MatchManager.DropMatch(player.CurrentMatch);

            PlayersManager.LogOut(clientID);
        }

        #endregion

        #region Data

        /// <summary>
        /// Client sent some data
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="dataType"></param>
        /// <param name="data"></param>
        public void ClientSentData(int clientID, int dataType, object data)
        {
            try
            {
                switch ((DataTypes)dataType)
                {
                    case DataTypes.LogInRequest:
                        break;
                    case DataTypes.PutPlayerIntoQueueRequest:
                        PutPlayerIntoQueue(clientID, (PutPlayerIntoQueueRequest) data);
                        break;
                    case DataTypes.BlockSwapRequest:
                        ProcessBlockSwap(clientID, (BlockSwapRequest) data);
                        break;
                    case DataTypes.UpgradeRequest:
                        ProcessUpgradeRequest(clientID, (UpgradeRequest)data);
                        break;
                    case DataTypes.GetPlayerStatsRequest:
                        ProcessGetPlayerStatsRequest(clientID,(GetPlayerStatsRequest)data);
                        break;
                    case DataTypes.SetPlayerStatsRequest:
                        ProcessSetPlayerStatsRequest(clientID, (SetPlayerStatsRequest)data);
                        break;
                    case DataTypes.PlayerStatsResponse:
                    case DataTypes.GameEndResponse:
                    case DataTypes.LogInResponse:
                    case DataTypes.ConnectResponse:
                    case DataTypes.StartGameResponse:
                    case DataTypes.GameStateResponse:
                    case DataTypes.ErrorResponse:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing message from client {clientID}:{e}");
            }
        }

        /// <summary>
        /// Puts player into the game queue
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="request"></param>
        public void PutPlayerIntoQueue(int clientID, PutPlayerIntoQueueRequest request)
        {
            Player player = PlayersManager.GetPlayer(clientID);
            if (player == null)
            {
                Console.WriteLine($"Can't find player {clientID}");
                return;
            }
            if (request.DebugMode)
            {
                player.IsInDebugMode = true;
                Console.WriteLine($"Player {clientID} is in debug mode");
            }
            else
            {
                player.IsInDebugMode = false;
            }

            Console.WriteLine($"Putting player {clientID} into queue");
            PlayersManager.PutPlayerIntoQueue(player);
        }

        /// <summary>
        /// Players turn processing
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="request"></param>
        public void ProcessBlockSwap(int clientID, BlockSwapRequest request)
        {
            Player player = PlayersManager.GetPlayer(clientID);
            if (player == null)
            {
                Console.WriteLine($"Can't find player {clientID}");
                return;
            }
            if (player.CurrentMatch == null)
            {
                Console.WriteLine($"Player {player.ClientID} is not in the game");
                return;
            }

            GameMatch match = player.CurrentMatch;
            Field playerField = match.Player1 == player ? match.Field1 : match.Field2;
            Field enemyField = match.Player1 == player ? match.Field2 : match.Field1;
            Player enemy = match.Player1 == player ? match.Player2 : match.Player1;
            List<EffectData> effectsData = new List<EffectData>();

            FieldManager.RefreshGlobalEffects(playerField, player);
            FieldManager.RefreshGlobalEffects(enemyField, enemy);

            if (!player.TrySpendMana(player.BlockSwapCost))
            {
                SendError(player.ClientID, ErrorType.NotEnoughMana);
                return;
            }
            else
            {
                EffectData hData = new EffectData();
                hData.EffectType = EffectType.ManaChanged;
                hData.Data = new Dictionary<string, object>();
                hData.Data["Target"] = player.InGameID;
                hData.Data["Value"] = -player.BlockSwapCost;
                effectsData.Add(hData);
            }

            FieldManager.RefreshDurationEffects(playerField);
            FieldManager.RefreshDurationEffects(enemyField);

            if (!FieldManager.TryRebuildFieldFromSwap(playerField, new Swap(request.X, request.Y, request.Direction), out List<Block> blocks))
            {
                SendError(player.ClientID, ErrorType.ImpossibleTurn);
            }
            
            List<Combo> combos = FieldManager.CheckForCombos(playerField, blocks);
            foreach (Combo combo in combos)
            {
                FieldManager.DestroyBlocks(enemyField, combo.Blocks, BlockState.DestroyedAsCombo);
                effectsData.AddRange(BlockEffectsManager.ApplyEffectsFromCombo(match, match.Player1 == player ? 1 : 2, combo));
            }

            effectsData.AddRange(FieldManager.ClearDestroyedBlocks(playerField, match, player));
            effectsData.AddRange(FieldManager.ClearDestroyedBlocks(enemyField, match, enemy));

            FieldManager.FillHoles(playerField);
            FieldManager.FillHoles(enemyField);

            GameStateResponse response = new GameStateResponse {
                GameState = GetPlayer1MatchStateData(match),
                Effects = effectsData.ToArray()
            };
            Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameStateResponse, response);

            if (match.Player1 == match.Player2)
            {
                FieldManager.SetDefaultState(playerField);
                FieldManager.SetDefaultState(enemyField);
                return;
            }

            response = new GameStateResponse
            {
                GameState = GetPlayer2MatchStateData(match),
                Effects = effectsData.ToArray()
            };
            Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.GameStateResponse, response);
            
            FieldManager.SetDefaultState(playerField);
            effectsData.AddRange(FieldManager.ClearDestroyedBlocks(playerField, match, player));
            FieldManager.SetDefaultState(enemyField);
            effectsData.AddRange(FieldManager.ClearDestroyedBlocks(enemyField, match, enemy));

            if (CheckForGameEnd(match, out var gameEndResponse))
            {
                MatchManager.DropMatch(player.CurrentMatch);

                Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameEndResponse, gameEndResponse);
                if (match.Player1 == match.Player2)
                {
                    return;
                }
                Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.GameEndResponse, gameEndResponse);

                return;
            }
        }

        /// <summary>
        /// Players upgrade request processing
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="request"></param>
        public void ProcessUpgradeRequest(int clientID, UpgradeRequest request)
        {
            Player player = PlayersManager.GetPlayer(clientID);

            if (player == null)
            {
                Console.WriteLine($"Can't find player {clientID}");
                return;
            }
            if (player.CurrentMatch == null)
            {
                Console.WriteLine($"Player {player.ClientID} is not in the game");
                return;
            }

            GameMatch match = player.CurrentMatch;
            Field playerField = match.Player1 == player ? match.Field1 : match.Field2;
            Field enemyField = match.Player1 == player ? match.Field2 : match.Field1;
            Player enemy = match.Player1 == player ? match.Player2 : match.Player1;

            FieldManager.RefreshDurationEffects(playerField);
            FieldManager.RefreshDurationEffects(enemyField);
            FieldManager.RefreshGlobalEffects(playerField, player);
            FieldManager.RefreshGlobalEffects(enemyField, enemy);

            UpgradeRequestResponse upgradeResponse = UpgradeManager.ProcessUpgradeRequest(match, player, request.UpgradeBlockType);

            switch (upgradeResponse)
            {
                case UpgradeRequestResponse.Ok:
                    break;
                case UpgradeRequestResponse.NotEnoughMana:
                    SendError(player.ClientID, ErrorType.NotEnoughMana);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameStateResponse response = new GameStateResponse
            {
                GameState = GetPlayer1MatchStateData(match),
                Effects = new EffectData[0],
            };
            Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameStateResponse, response);

            if (match.Player1 == match.Player2)
            {
                return;
            }

            response = new GameStateResponse
            {
                GameState = GetPlayer2MatchStateData(match),
                Effects = new EffectData[0]
            };
            Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.GameStateResponse, response);
        }

        /// <summary>
        /// Player requests to get his info - collection, active hero..
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="request"></param>
        public void ProcessGetPlayerStatsRequest(int clientID, GetPlayerStatsRequest request)
        {
            Player player = PlayersManager.GetPlayer(clientID);

            if (player == null)
            {
                Console.WriteLine($"Can't find player {clientID}");
                return;
            }

            PlayerStatsData data = new PlayerStatsData
            {
                UniqueBlockCollection = player.UniqueBlockCollection.ToData(),
            };

            PlayerStatsResponse response = new PlayerStatsResponse
            {
                PlayerStats = data,
            };

            Server.SendDataToClient(player.ClientID, (int)DataTypes.PlayerStatsResponse, response);
        }

        /// <summary>
        /// Player requests to set his info - collection, active hero..
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="request"></param>
        public void ProcessSetPlayerStatsRequest(int clientID, SetPlayerStatsRequest request)
        {
            Player player = PlayersManager.GetPlayer(clientID);

            if (player == null)
            {
                Console.WriteLine($"Can't find player {clientID}");
                return;
            }

            PlayersManager.TrySetPlayerStats(player, request.PlayerStats);

            PlayerStatsData data = new PlayerStatsData
            {
                UniqueBlockCollection = player.UniqueBlockCollection.ToData(),
            };

            PlayerStatsResponse response = new PlayerStatsResponse
            {
                PlayerStats = data,
            };

            Server.SendDataToClient(player.ClientID, (int)DataTypes.PlayerStatsResponse, response);
        }

        private void SendError(int clientID, ErrorType type)
        {
            ErrorResponse error = new ErrorResponse();
            error.Type = type;
            Server.SendDataToClient(clientID, (int)DataTypes.ErrorResponse, error);
        }

        #endregion

        #region Game Logic
        
        /// <summary>
        /// Starts match between two players and sends them game info
        /// </summary>
        /// <param name="match"></param>
        public void StartMatch(GameMatch match)
        {
            Console.WriteLine($"Starting match");

            StartGameResponse response = new StartGameResponse { GameState = GetPlayer1MatchStateData(match) };
            Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.StartGameResponse, response);

            if (match.Player1 == match.Player2)
                return;

            response = new StartGameResponse { GameState = GetPlayer2MatchStateData(match) };
            Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.StartGameResponse, response);
        }

        /// <summary>
        /// Check if any player dead
        /// </summary>
        /// <param name="match"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool CheckForGameEnd(GameMatch match, out GameEndResponse response)
        {
            if (match.Player1.Health <= 0)
            {
                response = new GameEndResponse
                {
                    PlayerWon = match.Player2.InGameID,
                };

                return true;
            }
            if (match.Player2.Health <= 0)
            {
                response = new GameEndResponse
                {
                    PlayerWon = match.Player1.InGameID,
                };

                return true;
            }

            response = null;
            return false;
        }

        private GameStateData GetPlayer1MatchStateData(GameMatch match)
        {
            GameStateData data = new GameStateData();
            data.MainPlayer = match.Player1.ToData();
            data.EnemyPlayer = match.Player2.ToData();
            data.MainField = match.Field1.ToData();
            data.EnemyField = match.Field2.ToData();
            data.MainUpgradesInfo = match.Player1Upgrades.ToData();
            data.EnemyUpgradesInfo= match.Player2Upgrades.ToData();
            return data;
        }

        private GameStateData GetPlayer2MatchStateData(GameMatch match)
        {
            GameStateData data = new GameStateData();
            data.MainPlayer = match.Player2.ToData();
            data.EnemyPlayer = match.Player1.ToData();
            data.MainField = match.Field2.ToData();
            data.EnemyField = match.Field1.ToData();
            data.MainUpgradesInfo = match.Player2Upgrades.ToData();
            data.EnemyUpgradesInfo = match.Player1Upgrades.ToData();
            return data;
        }

        #endregion
    }
}
