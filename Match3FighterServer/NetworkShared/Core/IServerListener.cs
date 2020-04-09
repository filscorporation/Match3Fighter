namespace NetworkShared.Core
{
    public interface IServerListener
    {
        void ClientConnected(int clientID);

        void ClientDisconnected(int clientID);

        void ClientSentData(int clientID, int dataType, object data);
    }
}
