#nullable enable
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Polyscape.RPCLite
{
    public class ClientService : NetworkService
    {
        private readonly ClientInfo _client;
        
        public delegate void DisconnectedAction();
        public event DisconnectedAction? OnDisconnected;

        public static Task<ClientService> CreateAsync(string address, int port, ILogger<ClientService> logger)
        {
            return CreateAsync(address, port, Timeout.InfiniteTimeSpan, logger);
        }
        
        public static async Task<ClientService> CreateAsync(
            string address, int port, TimeSpan connectionTimeout, ILogger<ClientService> logger)
        {
            logger.LogDebug("Connect to {address}:{port}", address, port);
            var client = new TcpClient();
            var connectTask = client.ConnectAsync(address, port);
            var timeoutTask = Task.Delay(connectionTimeout);
            var completedTask = await Task.WhenAny(connectTask, timeoutTask);
            if (completedTask == timeoutTask)
            {
                client.Dispose();
                throw new TimeoutException("Connection timeout");
            }
            logger.LogInformation("Connected to {address}:{port}", address, port);
            var service = new ClientService(new ClientInfo(client, logger), logger);
            return service;
        }
        
        private ClientService(ClientInfo client, ILogger<ClientService> logger) : base(logger)
        {
            _client = client;
            Init();
        }

        public ClientService(string address, int port, ILogger<ClientService> logger) : base(logger)
        {
            Logger.LogDebug("Connect to {address}:{port}", address, port);
            var client = new TcpClient(address, port);
            Logger.LogInformation("Connected to {address}:{port}", address, port);
        
            _client = new ClientInfo(client, logger);
            Init();
        }
        
        private void Init()
        {
            SendMagicAsync().Forget(Logger);
            _client.StartReceiveLoop(Cts.Token);
            _client.PacketReceived += packet => OnPacketReceived(_client, packet);
            _client.Disconnected += Dispose;
            
            _client.Disconnected += () => OnDisconnected?.Invoke();
        }
    
        public async ValueTask SendAsync(Packet packet)
        {
            Logger.LogTrace("Send started {length}", packet.Data.Length);
            await _client.SendAsync(packet, token: Cts.Token);
            Logger.LogTrace("Send finished {length}", packet.Data.Length);
        }

        public ValueTask CallRpcAsync<T>(string name, T args, CancellationToken token)
        {
            Logger.LogTrace("CallRpcAsync {name} {args}", name, args);
            return _client.CallRpcAsync(name, args, token);
        }
    
        public ValueTask<T2?> CallRpcAsync<T1, T2>(string name, T1 args, CancellationToken token)
        {
            Logger.LogTrace("CallRpcAsync {name} {args}", name, args);
            return _client.CallRpcAsync<T1, T2>(name, args, token);
        }

        public override async ValueTask FlushAsync(CancellationToken token) => await _client.FlushAsync(token);

        public override void Dispose()
        {
            base.Dispose();

            _client.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task SendMagicAsync()
        {
            Logger.LogTrace("Send magic");
            await _client.Stream.WriteAsync(Magic, Cts.Token);
            Logger.LogTrace("Magic sent");
        }
    }
}