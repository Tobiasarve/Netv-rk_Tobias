using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using KnockKnockArena.Shared.Protocol;
using KnockKnockArena.Shared.Protocol.Messages;

namespace KnockKnockArena.Networking
{
    public sealed class TcpConnectedEvent { }

    public sealed class TcpDisconnectedEvent
    {
        public string Reason = "";
    }

    public sealed class NetworkClient: IDisposable
    {
        public readonly ConcurrentQueue<object> TcpMessages = new ConcurrentQueue<object>();

        public byte PlayerId { get; private set; }

        public long TcpMessagesSkipped;

        private TcpClient? _tcp;
        private NetworkStream? _tcpStream;
        private IPAddress _serverAddress = IPAddress.None;
        private volatile bool _running = true;

        private void ReceiveTcpLoop()
        {
            try
            {
                while (_running && TcpFraming.TryReadMessage(
                    _tcpStream, out MessageType type, out byte[] payload))
                {
                    switch (type)
                    {
                        case MessageType.LoginResponse:
                            LoginResponse login = LoginResponse.FromPayload(payload);

                            if (login.Success)
                                PlayerId = login.PlayerId;

                            TcpMessages.Enqueue(login);
                            break;

                        case MessageType.ServerInfo:
                            TcpMessages.Enqueue(ServerInfoMessage.FromPayload(payload));
                            break;

                        case MessageType.ChatBroadcast:
                            TcpMessages.Enqueue(ChatBroadcast.FromPayload(payload));
                            break;

                        case MessageType.PlayerJoined:
                            TcpMessages.Enqueue(PlayerJoined.FromPayload(payload));
                            break;

                        case MessageType.PlayerLeft:
                            TcpMessages.Enqueue(PlayerLeft.FromPayload(payload));
                            break;

                        default:
                            Interlocked.Increment(ref TcpMessagesSkipped);
                            break;
                    }
                }

                TcpMessages.Enqueue(new TcpDisconnectedEvent
                {
                    Reason = "server closed the connection"
                });
            }
            catch (Exception ex)
            {
                if (_running)
                {
                    TcpMessages.Enqueue(new TcpDisconnectedEvent
                    {
                        Reason = ex.Message
                    });
                }
            }
        }
        public void ConnectAndLogin(string host, int port, string username, string password)
        {
            Thread tcpThread = new Thread(() =>
            {
                try
                {
                    _serverAddress = AddressResolver.Resolve(host);
                    _tcp = new TcpClient(AddressFamily.InterNetwork);
                    _tcp.Connect(_serverAddress, port);
                    _tcpStream = _tcp.GetStream();
                    TcpFraming.WriteMessage(_tcpStream, MessageType.LoginRequest, new LoginRequest
                    {
                        Username = username,
                        Password = password,
                    }.ToPayload());
                    TcpMessages.Enqueue(new TcpConnectedEvent());

                    ReceiveTcpLoop();
                }
                catch (Exception ex)
                {
                    TcpMessages.Enqueue(new TcpDisconnectedEvent { Reason = ex.Message });
                }
            })
            {
                IsBackground = true,
                Name = "kka-tcp",
            };
            tcpThread.Start();
        }

        public void SendChat(string text)
        {
            NetworkStream? stream = _tcpStream;
            if (stream == null)
                return;
            TcpFraming.WriteMessage(stream, MessageType.ChatSend, new ChatSend { Text = text }.ToPayload());
        }
        public void Dispose()
        {
            _running = false;
            try { _tcp?.Close(); } catch { }
        }
    }
}