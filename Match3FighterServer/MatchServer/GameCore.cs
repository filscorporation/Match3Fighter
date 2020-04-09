using System;
using System.Threading;
using NetworkShared.Core;
using NetworkShared.Data;
using NetworkShared.Network;

namespace MatchServer
{
    /// <summary>
    /// Controls main game loop and all ingame components connections
    /// </summary>
    public class GameCore : IServerListener
    {
        public bool IsRunning = false;
        public const int TicksPerSec = 30;
        public const int MsPerTick = 1000 / TicksPerSec;

        public Server Server;

        #region Core

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
        }

        #endregion

        #region Clients

        /// <summary>
        /// Client was connected
        /// </summary>
        /// <param name="clientID"></param>
        public void ClientConnected(int clientID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Client was disconnected
        /// </summary>
        /// <param name="clientID"></param>
        public void ClientDisconnected(int clientID)
        {
            throw new NotImplementedException();
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
                    case DataTypes.ConnectResponse:
                        break;
                    case DataTypes.LogInRequest:
                        break;
                    case DataTypes.LogInResponse:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing message from clint {clientID}:{e}");
            }
        }

        #endregion
    }
}
