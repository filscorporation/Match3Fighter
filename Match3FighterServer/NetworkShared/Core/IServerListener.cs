namespace NetworkShared.Core
{
    /// <summary>
    /// Interface for types that can handle server events
    /// </summary>
    public interface IServerListener
    {
        /// <summary>
        /// Client was connected
        /// </summary>
        /// <param name="clientID"></param>
        void ClientConnected(int clientID);

        /// <summary>
        /// Client was disconnected
        /// </summary>
        /// <param name="clientID"></param>
        void ClientDisconnected(int clientID);

        /// <summary>
        /// Client sent some data
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="dataType"></param>
        /// <param name="data"></param>
        void ClientSentData(int clientID, int dataType, object data);
    }
}
