namespace NetworkShared.Network
{
    /// <summary>
    /// Interface for type that can handle clients events
    /// </summary>
    public interface IConnectionHandler
    {
        void ReadPacket(int type, Packet packet);

        void OnDisconnect();
    }
}
