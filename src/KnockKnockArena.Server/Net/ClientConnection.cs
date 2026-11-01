using System.Net.Sockets;
using KnockKnockArena.Server.Auth;
using KnockKnockArena.Shared.Protocol;

namespace KnockKnockArena.Server.Net
{


    public sealed class ClientConnection
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly object _sendGate = new();

        public PlayerSession? Session;

        public string RemoteEndPoint { get; }

        public ClientConnection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
            RemoteEndPoint = tcpClient.Client.RemoteEndPoint?.ToString() ?? "unknown";
        }
        public string DisplayName =>
        Session == null ? RemoteEndPoint : $"{Session.Username}#{Session.PlayerId} ({RemoteEndPoint})";

        public bool TryReadMessage(out MessageType type, out byte[] payload) =>
        TcpFraming.TryReadMessage(_stream, out type, out payload);

        public void Send(MessageType type, byte[] payload)
        {
            lock (_sendGate)
                TcpFraming.WriteMessage(_stream, type, payload);
        }
        public void Close()
        {
            try { _tcpClient.Close(); }
            catch { /* already closed */ }
        }
    }
}