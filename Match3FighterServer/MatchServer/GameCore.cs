using System;
using System.Threading;
using MatchServer.FieldManagement;
using MatchServer.Players;
using NetworkShared.Core;
using NetworkShared.Data;
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
        public MatchManager MatchManager;
        public PlayersManager PlayersManager;

        #region Core

        public void Initialize()
        {
            if (Instance != null)
                throw new Exception("GameCore instance already exists");
            Instance = this;

            FieldManager = new FieldManager();
            MatchManager = new MatchManager(FieldManager);
            PlayersManager = new PlayersManager(MatchManager);
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
                    case DataTypes.LogInResponse:
                    case DataTypes.ConnectResponse:
                    case DataTypes.StartGameResponse:
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
            Console.WriteLine($"Putting player {clientID} into queue");
            PlayersManager.PutPlayerIntoQueue(player);
        }

        /// <summary>
        /// Starts match between two players and sends them game info
        /// </summary>
        /// <param name="match"></param>
        public void StartMatch(GameMatch match)
        {
            Console.WriteLine($"Starting match");

            StartGameResponse response1 = new StartGameResponse();
            response1.MainPlayer = match.Player1.ToData();
            response1.EnemyPlayer = match.Player2.ToData();
            response1.MainField = match.Field1.ToData();
            response1.EnemyField = match.Field2.ToData();

            Server.SendDataToClient(match.Player1.ClientID, (int)DataTypes.StartGameResponse, response1);

            // TODO: temp
            if (match.Player1 == match.Player2)
                return;

            StartGameResponse response2 = new StartGameResponse();
            response1.MainPlayer = response1.EnemyPlayer;
            response1.EnemyPlayer = response1.MainPlayer;
            response1.MainField = response1.EnemyField;
            response1.EnemyField = response1.MainField;

            Server.SendDataToClient(match.Player2.ClientID, (int)DataTypes.StartGameResponse, response2);
        }

        #endregion
    }
}
