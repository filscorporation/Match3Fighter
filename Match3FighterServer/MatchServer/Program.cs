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

            // Starts game core loop
            GameCore core = new GameCore();
            Task.Factory.StartNew(core.Start);

            // Starts server that holds client connections and receiving/sending data
            Server server = new Server();
            server.Listener = core;
            server.Start(maxPlayers, portNumber);

            core.Server = server;
            
            Console.ReadKey();
        }
    }
}
