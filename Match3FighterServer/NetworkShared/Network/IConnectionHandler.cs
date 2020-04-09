namespace NetworkShared.Network
{
    public interface IConnectionHandler
    {
        void ReadPacket(int type, Packet packet);

        void OnDisconnect();
    }
}
