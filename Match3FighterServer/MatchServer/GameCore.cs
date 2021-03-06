﻿using System;
using System.Collections.Generic;
using System.Threading;
using MatchServer.DatabaseManagement;
using MatchServer.FieldManagement;
using MatchServer.Players;
using MatchServer.SkillsManagement;
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
        public DatabaseManager DatabaseManager;
        public FieldManager FieldManager;
        public SkillsManager SkillsManager;
        public UpgradeManager UpgradeManager;
        public MatchManager MatchManager;
        public PlayersManager PlayersManager;
        public BlockEffectsManager BlockEffectsManager;
        public AIManager AIManager;

        #region Core

        public void Initialize()
        {
            if (Instance != null)
                throw new Exception("GameCore instance already exists");
            Instance = this;

            DatabaseManager = new DatabaseManager();
            DatabaseManager.Connect();

            FieldManager = new FieldManager();
            UpgradeManager = new UpgradeManager();
            SkillsManager = new SkillsManager(FieldManager);
            AIManager = new AIManager(FieldManager);
            MatchManager = new MatchManager(FieldManager, AIManager);
            PlayersManager = new PlayersManager(MatchManager, DatabaseManager);
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
                        Thread.Sleep(Math.Max(0, (nextLoop - DateTime.Now).Milliseconds));
                    }
                }
            }
        }

        private void Update()
        {
            ThreadManager.UpdateMain();

            PlayersManager.TryMakeMatch();
            AIManager.UpdateBots();
        }

        #endregion

        #region Clients

        /// <summary>
        /// Client was connected
        /// </summary>
        /// <param name="clientID"></param>
        public void ClientConnected(int clientID)
        {
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
            if (player?.CurrentMatch != null)
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
                        LogIn(clientID, (LogInRequest)data);
                        break;
                    case DataTypes.PutPlayerIntoQueueRequest:
                        PutPlayerIntoQueue(clientID, (PutPlayerIntoQueueRequest)data);
                        break;
                    case DataTypes.BlockSwapRequest:
                        ProcessBlockSwap(clientID, (BlockSwapRequest)data);
                        break;
                    case DataTypes.BlockTapRequest:
                        ProcessBlockTap(clientID, (BlockTapRequest)data);
                        break;
                    case DataTypes.UseSkillRequest:
                        ProcessUseSkillRequest(clientID, (UseSkillRequest)data);
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
        /// Logs player in
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="request"></param>
        public void LogIn(int clientID, LogInRequest request)
        {
            Player player;
            LogInType logInType;
            try
            {
                logInType = PlayersManager.LogIn(clientID, request.PlayerID, out player);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error logging in client {clientID}: {e}");
                ErrorResponse error = new ErrorResponse();
                error.Type = ErrorType.LogInFailed;
                Server.SendDataToClient(clientID, (int)DataTypes.ErrorResponse, error);
                return;
            }

            LogInResponse response = new LogInResponse();
            response.Type = logInType;
            response.PlayerStats = player.GetStatsData();
            Server.SendDataToClient(clientID, (int)DataTypes.LogInResponse, response);
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

            player.GameMode = request.GameMode;
            player.GameParameters = request.GameParameters;

            Console.WriteLine($"Putting player {clientID} into queue in {player.GameMode} mode");
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

            ProcessBlockSwap(player, request);
        }

        /// <summary>
        /// Players turn processing
        /// </summary>
        /// <param name="player"></param>
        /// <param name="request"></param>
        public void ProcessBlockSwap(Player player, BlockSwapRequest request)
        {
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
                SendError(player, ErrorType.NotEnoughMana);
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
                SendError(player, ErrorType.ImpossibleTurn);
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

            if (match.GameMode == GameMode.Practice)
            {
                FieldManager.SetDefaultState(playerField);
                FieldManager.SetDefaultState(enemyField);

                if (CheckForGameEnd(match, out GameEndResponse gameEndResponseDebug))
                {
                    GiveMatchReward(match, gameEndResponseDebug.PlayerWon);
                    RecalculateRating(match, gameEndResponseDebug.PlayerWon);
                    PlayersManager.UpdatePlayer(match.Player1);

                    MatchManager.DropMatch(player.CurrentMatch);

                    gameEndResponseDebug.PlayerStats = match.Player1.GetStatsData();
                    Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameEndResponse, gameEndResponseDebug);
                }
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

            if (CheckForGameEnd(match, out GameEndResponse gameEndResponse))
            {
                GiveMatchReward(match, gameEndResponse.PlayerWon);
                RecalculateRating(match, gameEndResponse.PlayerWon);
                PlayersManager.UpdatePlayer(match.Player1);
                PlayersManager.UpdatePlayer(match.Player2);

                MatchManager.DropMatch(player.CurrentMatch);

                gameEndResponse.PlayerStats = match.Player1.GetStatsData();
                Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameEndResponse, gameEndResponse);

                gameEndResponse.PlayerStats = match.Player2.GetStatsData();
                Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.GameEndResponse, gameEndResponse);

                return;
            }
        }

        /// <summary>
        /// Players tap turn processing
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="request"></param>
        public void ProcessBlockTap(int clientID, BlockTapRequest request)
        {
            Player player = PlayersManager.GetPlayer(clientID);
            if (player == null)
            {
                Console.WriteLine($"Can't find player {clientID}");
                return;
            }

            ProcessBlockTap(player, request);
        }

        /// <summary>
        /// Players tap turn processing
        /// </summary>
        /// <param name="player"></param>
        /// <param name="request"></param>
        public void ProcessBlockTap(Player player, BlockTapRequest request)
        {
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
            
            FieldManager.RefreshDurationEffects(playerField);
            FieldManager.RefreshDurationEffects(enemyField);

            if (!FieldManager.TryLockBlock(playerField, request.X, request.Y, out Combo combo))
            {
                // Lock removed, try to pop combo
                if (combo != null)
                {
                    // TODO: merge combo processing code
                    FieldManager.DestroyBlocks(enemyField, combo.Blocks, BlockState.DestroyedAsCombo);
                    effectsData.AddRange(BlockEffectsManager.ApplyEffectsFromCombo(match, match.Player1 == player ? 1 : 2, combo));

                    effectsData.AddRange(FieldManager.ClearDestroyedBlocks(playerField, match, player));
                    effectsData.AddRange(FieldManager.ClearDestroyedBlocks(enemyField, match, enemy));

                    FieldManager.FillHoles(playerField);
                    FieldManager.FillHoles(enemyField);
                }
            }
            
            GameStateResponse response = new GameStateResponse
            {
                GameState = GetPlayer1MatchStateData(match),
                Effects = effectsData.ToArray(),
            };
            Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameStateResponse, response);

            if (match.GameMode == GameMode.Practice)
            {
                FieldManager.SetDefaultState(playerField);
                FieldManager.SetDefaultState(enemyField);

                if (CheckForGameEnd(match, out GameEndResponse gameEndResponseDebug))
                {
                    GiveMatchReward(match, gameEndResponseDebug.PlayerWon);
                    RecalculateRating(match, gameEndResponseDebug.PlayerWon);
                    PlayersManager.UpdatePlayer(match.Player1);

                    MatchManager.DropMatch(player.CurrentMatch);

                    gameEndResponseDebug.PlayerStats = match.Player1.GetStatsData();
                    Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameEndResponse, gameEndResponseDebug);
                }
                return;
            }

            response = new GameStateResponse
            {
                GameState = GetPlayer2MatchStateData(match),
                Effects = effectsData.ToArray(),
            };
            Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.GameStateResponse, response);

            FieldManager.SetDefaultState(playerField);
            effectsData.AddRange(FieldManager.ClearDestroyedBlocks(playerField, match, player));
            FieldManager.SetDefaultState(enemyField);
            effectsData.AddRange(FieldManager.ClearDestroyedBlocks(enemyField, match, enemy));

            if (CheckForGameEnd(match, out GameEndResponse gameEndResponse))
            {
                GiveMatchReward(match, gameEndResponse.PlayerWon);
                RecalculateRating(match, gameEndResponse.PlayerWon);
                PlayersManager.UpdatePlayer(match.Player1);
                PlayersManager.UpdatePlayer(match.Player2);

                MatchManager.DropMatch(player.CurrentMatch);

                gameEndResponse.PlayerStats = match.Player1.GetStatsData();
                Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameEndResponse, gameEndResponse);

                gameEndResponse.PlayerStats = match.Player2.GetStatsData();
                Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.GameEndResponse, gameEndResponse);

                return;
            }
        }

        /// <summary>
        /// Players skill using request processing
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="request"></param>
        public void ProcessUseSkillRequest(int clientID, UseSkillRequest request)
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

            FieldManager.RefreshDurationEffects(playerField);
            FieldManager.RefreshDurationEffects(enemyField);

            if (request.SkillID != 0 && request.SkillID != 1)
            {
                SendError(player, ErrorType.ImpossibleTurn);
                return;
            }

            int playerUserIndex = match.Player1 == player ? 1 : 2;
            Skill skill = player.ActiveSkills[request.SkillID];

            if (!player.TrySpendEnergy(skill.Cost))
            {
                SendError(player, ErrorType.NotEnoughEnergy);
                return;
            }
            else
            {
                EffectData hData = new EffectData();
                hData.EffectType = EffectType.EnergyChanged;
                hData.Data = new Dictionary<string, object>();
                hData.Data["Target"] = player.InGameID;
                hData.Data["Value"] = -skill.Cost;
                effectsData.Add(hData);
            }

            effectsData.AddRange(SkillsManager.ApplySkillEffect(match, playerUserIndex, skill.Name));

            GameStateResponse response = new GameStateResponse
            {
                GameState = GetPlayer1MatchStateData(match),
                Effects = effectsData.ToArray(),
            };
            Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameStateResponse, response);

            if (match.GameMode == GameMode.Practice)
            {
                FieldManager.SetDefaultState(playerField);
                FieldManager.SetDefaultState(enemyField);

                if (CheckForGameEnd(match, out GameEndResponse gameEndResponseDebug))
                {
                    GiveMatchReward(match, gameEndResponseDebug.PlayerWon);
                    RecalculateRating(match, gameEndResponseDebug.PlayerWon);
                    PlayersManager.UpdatePlayer(match.Player1);

                    MatchManager.DropMatch(player.CurrentMatch);

                    gameEndResponseDebug.PlayerStats = match.Player1.GetStatsData();
                    Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameEndResponse, gameEndResponseDebug);
                }
                return;
            }

            response = new GameStateResponse
            {
                GameState = GetPlayer2MatchStateData(match),
                Effects = effectsData.ToArray(),
            };
            Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.GameStateResponse, response);

            FieldManager.SetDefaultState(playerField);
            effectsData.AddRange(FieldManager.ClearDestroyedBlocks(playerField, match, player));
            FieldManager.SetDefaultState(enemyField);
            effectsData.AddRange(FieldManager.ClearDestroyedBlocks(enemyField, match, enemy));

            if (CheckForGameEnd(match, out GameEndResponse gameEndResponse))
            {
                GiveMatchReward(match, gameEndResponse.PlayerWon);
                RecalculateRating(match, gameEndResponse.PlayerWon);
                PlayersManager.UpdatePlayer(match.Player1);
                PlayersManager.UpdatePlayer(match.Player2);

                MatchManager.DropMatch(player.CurrentMatch);

                gameEndResponse.PlayerStats = match.Player1.GetStatsData();
                Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.GameEndResponse, gameEndResponse);

                gameEndResponse.PlayerStats = match.Player2.GetStatsData();
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
                    SendError(player, ErrorType.NotEnoughMana);
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

            if (match.GameMode == GameMode.Practice)
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

            PlayerStatsData data = player.GetStatsData();

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

            PlayerStatsData data = player.GetStatsData();

            PlayerStatsResponse response = new PlayerStatsResponse
            {
                PlayerStats = data,
            };

            Server.SendDataToClient(player.ClientID, (int)DataTypes.PlayerStatsResponse, response);
        }

        private void SendError(Player player, ErrorType type)
        {
            if (player is Bot)
            {
                Console.WriteLine($"Error: {type}");
                return;
            }
            ErrorResponse error = new ErrorResponse();
            error.Type = type;
            Server.SendDataToClient(player.ClientID, (int)DataTypes.ErrorResponse, error);
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

            if (match.GameMode == GameMode.Practice)
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

        /// <summary>
        /// Recalculates players rating from match results
        /// </summary>
        /// <param name="match"></param>
        /// <param name="playerWonID"></param>
        private void RecalculateRating(GameMatch match, int playerWonID)
        {
            // TODO: elo
            if (playerWonID == match.Player1.InGameID)
            {
                match.Player1.Rating += 2;
                match.Player2.Rating -= 1;
            }
            else
            {
                match.Player1.Rating -= 1;
                match.Player2.Rating += 2;
            }
        }

        /// <summary>
        /// Gives both players reward from match results
        /// </summary>
        /// <param name="match"></param>
        /// <param name="playerWonID"></param>
        private void GiveMatchReward(GameMatch match, int playerWonID)
        {
            const int winnerReward = 10;
            const int loserReward = 5;
            if (playerWonID == match.Player1.InGameID)
            {
                match.Player1.Currency += winnerReward;
                match.Player2.Currency += loserReward;
            }
            else
            {
                match.Player1.Currency += loserReward;
                match.Player2.Currency += winnerReward;
            }
        }

        private GameStateData GetPlayer1MatchStateData(GameMatch match)
        {
            GameStateData data = new GameStateData();
            data.GameID = match.ID.ToString();
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
            data.GameID = match.ID.ToString();
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
