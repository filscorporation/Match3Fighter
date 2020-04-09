using System;
using System.Threading.Tasks;
using NetworkShared.Network;

namespace MatchServer
{
    public class Program
    {
        private const int maxPlayers = 100;
        private const int portNumber = 26910;

        public static void Main(string[] args)
        {
            Console.Title = "Match3Fighter Game Server";

            GameCore core = new GameCore();
            Task.Factory.StartNew(core.Start);

            Server server = new Server();
            server.Listener = core;
            server.Start(maxPlayers, portNumber);

            core.Server = server;
            
            Console.ReadKey();
        }
    }
}
