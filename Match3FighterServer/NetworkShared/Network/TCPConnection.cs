using System;
using System.Net.Sockets;
using NetworkShared.Core;

namespace NetworkShared.Network
{
    /// <summary>
    /// TCP connection
    /// </summary>
    public class TCPConnection
    {
        protected IConnectionHandler Handler;
        public TcpClient Socket;

        private NetworkStream stream;
        private byte[] receivedBuffer;
        private Packet receivedPacket;

        private const int dataBufferSize = 4096;

        public TCPConnection(IConnectionHandler handler)
        {
            Handler = handler;
        }

        /// <summary>
        /// Create TCP connection from a client and begin read
        /// </summary>
        /// <param name="socket"></param>
        public void Connect(TcpClient socket)
        {
            Socket = socket;
            Socket.ReceiveBufferSize = dataBufferSize;
            Socket.SendBufferSize = dataBufferSize;

            Console.WriteLine($"Incoming connection from {Socket.Client.RemoteEndPoint}");

            stream = Socket.GetStream();

            receivedPacket = new Packet();
            receivedBuffer = new byte[dataBufferSize];

            stream.BeginRead(receivedBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        /// <summary>
        /// Create TCP connection to the server
        /// </summary>
        public void Connect(string ip, int port)
        {
            Socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receivedBuffer = new byte[dataBufferSize];
            Socket.BeginConnect(ip, port, ConnectCallback, Socket);
        }

        /// <summary>
        /// Callback to connection attempt
        /// </summary>
        /// <param name="result"></param>
        private void ConnectCallback(IAsyncResult result)
        {
            Socket.EndConnect(result);

            if (!Socket.Connected)
            {
                return;
            }

            stream = Socket.GetStream();

            receivedPacket = new Packet();

            stream.BeginRead(receivedBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        /// <summary>
        /// Drop TCP connection
        /// </summary>
        public void Disconnect()
        {
            Console.WriteLine($"Disconnecting {Socket?.Client?.RemoteEndPoint}");

            Socket?.Close();
            stream = null;
            receivedPacket = null;
            receivedBuffer = null;
            Socket = null;

            Handler.OnDisconnect();
        }

        /// <summary>
        /// Sends packet through TCP connection
        /// </summary>
        /// <param name="data"></param>
        public void SendPacket(Packet data)
        {
            try
            {
                if (Socket != null)
                {
                    stream.BeginWrite(data.ToArray(), 0, data.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data via TCP: {ex}");
            }
        }

        /// <summary>
        /// Process callback from network stream
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receivedBuffer, data, byteLength);

                receivedPacket.Reset(HandlePacket(data));
                stream.BeginRead(receivedBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving TCP data: {ex}");
                Disconnect();
            }
        }

        private bool HandlePacket(byte[] bytes)
        {
            int index = 0;

            receivedPacket.SetBytes(bytes);

            if (receivedPacket.UnreadLength() >= 4)
            {
                index = receivedPacket.ReadInt();
                if (index <= 0)
                {
                    return true;
                }
            }

            while (index > 0 && index <= receivedPacket.UnreadLength())
            {
                byte[] dataBytes = receivedPacket.ReadBytes(index);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(dataBytes))
                    {
                        int packetId = packet.ReadInt();
                        Handler.ReadPacket(packetId, packet);
                    }
                });

                index = 0;
                if (receivedPacket.UnreadLength() >= 4)
                {
                    index = receivedPacket.ReadInt();
                    if (index <= 0)
                    {
                        return true;
                    }
                }
            }

            if (index <= 1)
            {
                return true;
            }

            return false;
        }
    }
}
