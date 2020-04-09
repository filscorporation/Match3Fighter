using System;
using System.Threading;
using NetworkShared.Core;
using NetworkShared.Data;
using NetworkShared.Network;

namespace MatchServer
{
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

        public void ClientConnected(int clientID)
        {
            throw new NotImplementedException();
        }

        public void ClientDisconnected(int clientID)
        {
            throw new NotImplementedException();
        }

        #endregion
        
        #region Data

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
