#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Polyscape.RPCLite
{
    public class ServerService : NetworkService
    {
        private readonly TcpListener _listener;
        private readonly List<ClientInfo> _clients = new List<ClientInfo>();

        public delegate void AcceptClient(ClientInfo client, int id);
        public event AcceptClient? OnAcceptClient;
        
        public delegate void DisconnectedClient(ClientInfo client, int id);
        public event DisconnectedClient? OnDisconnectedClient;
        
        public ServerService(int port, ILogger<ServerService> logger) : base(logger)
        {
            Logger.LogInformation("Listen on {port}", port);
            _listener = new TcpListener(IPAddress.Any, port);
            AcceptClientsAsync().Forget(Logger);
        }

        public async ValueTask BroadcastAsync(Packet packet)
        {
            Logger.LogDebug("Broadcast started");
            var clients = new List<ClientInfo>();
            lock (_clients)
            {
                clients.AddRange(_clients);
            }

            await Task.WhenAll(clients.Select(client => client.SendAsync(packet, token: Cts.Token).AsTask()));

            Logger.LogDebug("Broadcast finished");
        }

        public ValueTask CallRpcAsync<T>(int clientId, string name, T args, CancellationToken token)
        {
            Logger.LogDebug("CallRpcAsync {clientId} {name} {args}", clientId, name, args);
            lock (_clients)
            {
                var client = _clients.FirstOrDefault(c => ClientInfoToId(c) == clientId);
                return client?.CallRpcAsync(name, args, token) ?? default;
            }
        }

        public ValueTask<T2?> CallRpcAsync<T1, T2>(int clientId, string name, T1 args, CancellationToken token)
        {
            Logger.LogDebug("CallRpcAsync {clientId} {name} {args}", clientId, name, args);
            lock (_clients)
            {
                var client = _clients.FirstOrDefault(c => ClientInfoToId(c) == clientId);
                return client?.CallRpcAsync<T1, T2>(name, args, token) ?? default;
            }
        }

        public override ValueTask FlushAsync(CancellationToken token)
        {
            Logger.LogDebug("Flush started");
            lock (_clients)
            {
                return new ValueTask(Task.WhenAll(_clients.Select(client => client.FlushAsync(token).AsTask())));
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _listener.Stop();

            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    client.Dispose();
                }

                _clients.Clear();
            }

            GC.SuppressFinalize(this);
        }

        private async Task AcceptClientsAsync()
        {
            Logger.LogDebug("Accept loop started");
            _listener.Start();
            while (!Cts.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync();
                CheckClientAndAddAsync(client).Forget(Logger);
            }
        }

        private async Task CheckClientAndAddAsync(TcpClient client)
        {
            Logger.LogDebug("Client accepted");
            if (client.Connected)
            {
                Logger.LogDebug("Client Check started");
                var clientInfo = new ClientInfo(client, Logger);

                // Check magic
                var magic = new byte[Magic.Length];
                await clientInfo.Stream.ReceiveAsync(magic, Cts.Token);
                if (!magic.SequenceEqual(Magic))
                {
                    clientInfo.Dispose();
                    Logger.LogWarning("Client Check failed");
                    return;
                }

                Logger.LogDebug("Client Check passed");
                lock (_clients)
                {
                    _clients.Add(clientInfo);
                }
                
                var clientId = ClientInfoToId(clientInfo);
                OnAcceptClient?.Invoke(clientInfo, clientId);

                clientInfo.StartReceiveLoop(Cts.Token);
                clientInfo.PacketReceived += packet => OnPacketReceived(clientInfo, packet);
                clientInfo.Disconnected += () =>
                {
                    Logger.LogDebug("Client disconnected");
                    lock (_clients)
                    {
                        _clients.Remove(clientInfo);
                    }
                    OnDisconnectedClient?.Invoke(clientInfo, clientId);
                };
            }
        }
    }
}