using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using NetworkShared.Core;

namespace NetworkShared.Network
{
    /// <summary>
    /// Server holds client connections and data sharing
    /// </summary>
    public class Server
    {
        public IServerListener Listener;
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();

        private static TcpListener tcpListener;

        /// <summary>
        /// Initializes clients and starts to listen
        /// </summary>
        /// <param name="maxPlayers">Max connections</param>
        /// <param name="port">Port to start server on</param>
        public void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            Console.WriteLine($"Server started on port {Port}.");
        }

        /// <summary>
        /// Callback when client is connected
        /// </summary>
        /// <param name="result"></param>
        private void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (Clients[i].ClientTCP.Socket == null)
                {
                    Clients[i].ClientTCP.Connect(client);
                    Listener.ClientConnected(i);
                    return;
                }
            }

            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server is full!");
        }

        /// <summary>
        /// Initialize clients
        /// </summary>
        private void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i, this));
            }
        }

        /// <summary>
        /// Sends data to the client
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void SendDataToClient(int clientID, int type, object data)
        {
            byte[] bytes = null;
            if (data != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    binaryFormatter.Serialize(stream, data);
                    bytes = stream.ToArray();
                }
            }

            using (Packet packet = new Packet(type))
            {
                if (bytes != null)
                    packet.SetBytes(bytes);
                packet.WriteLength();
                Clients[clientID].ClientTCP.SendPacket(packet);
            }
        }

        /// <summary>
        /// Handles packet from a client
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="type"></param>
        /// <param name="packet"></param>
        public void GetPacketFromClient(int clientID, int type, Packet packet)
        {
            byte[] bytes = packet.ReadAllBytes();
            object result = null;

            if (bytes != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    result = binaryFormatter.Deserialize(stream);
                }
            }

            Listener.ClientSentData(clientID, type, result);
        }

        /// <summary>
        /// Handles disconnect of a client
        /// </summary>
        /// <param name="clientID"></param>
        public void OnClientDisconnect(int clientID)
        {
            Listener.ClientDisconnected(clientID);
        }
    }
}
