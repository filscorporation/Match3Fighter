namespace NetworkShared.Network
{
    /// <summary>
    /// Client representation on a server with its own connection
    /// </summary>
    public class Client : IConnectionHandler
    {
        /// <summary>
        /// Client unique ID
        /// </summary>
        public int ID;

        /// <summary>
        /// Client connection
        /// </summary>
        public TCPConnection ClientTCP;

        /// <summary>
        /// Link to a server
        /// </summary>
        public Server Server;

        public Client(int clientId, Server server)
        {
            ID = clientId;
            ClientTCP = new TCPConnection(this);
            Server = server;
        }

        /// <summary>
        /// Disconnect the client
        /// </summary>
        public void Disconnect()
        {
            ClientTCP.Disconnect();
        }

        /// <summary>
        /// Redirect packet to the server
        /// </summary>
        /// <param name="type"></param>
        /// <param name="packet"></param>
        public void ReadPacket(int type, Packet packet)
        {
            Server.GetPacketFromClient(ID, type, packet);
        }

        /// <summary>
        /// Redirect disconnect to the server
        /// </summary>
        public void OnDisconnect()
        {
            Server.OnClientDisconnect(ID);
        }
    }
}
