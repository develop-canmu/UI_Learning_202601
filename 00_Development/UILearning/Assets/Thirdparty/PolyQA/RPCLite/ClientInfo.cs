#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MemoryPack;
using Microsoft.Extensions.Logging;

namespace Polyscape.RPCLite
{
    public sealed class ClientInfo : IDisposable
    {
        private readonly List<Packet> _sendQueue = new List<Packet>();
        private readonly List<Packet> _receiveQueue = new List<Packet>();
        private bool _isFlushing;
        private ILogger _logger;

        public readonly TcpClient Client;
        public readonly NetworkStream Stream;

        public delegate void PacketHandler(Packet packet);

        public delegate void DisconnectedHandler();

        public event PacketHandler? PacketReceived;
        public event DisconnectedHandler? Disconnected;

        public ClientInfo(TcpClient client, ILogger logger)
        {
            Client = client;
            Stream = client.GetStream();
            _logger = logger;
        }

        public void StartReceiveLoop(CancellationToken token) => _ = Task.Run(() => ReceiveLoopAsync(token));

        public void Dispose()
        {
            Stream.Dispose();
            Client.Dispose();
        }

        public async ValueTask SendAsync(Packet packet, bool isForceFlush = false, CancellationToken token = default)
        {
            int size;
            lock (_sendQueue)
            {
                _sendQueue.Add(packet);
                size = _sendQueue.Sum(p => p.Size);
            }

            if (size > 1024 * 1024 || isForceFlush)
            {
                await FlushAsync(token);
            }
        }

        public async ValueTask FlushAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _isFlushing)
            {
                await Task.Delay(100, token);
            }

            _isFlushing = true;
            try
            {
                byte[] bytes;
                lock (_sendQueue)
                {
                    if (_sendQueue.Count == 0)
                    {
                        return;
                    }

                    bytes = MemoryPackSerializer.Serialize(_sendQueue);
                    _sendQueue.Clear();
                }

                var size = BitConverter.GetBytes(bytes.Length);
                await Stream.WriteAsync(size, token);
                await Stream.WriteAsync(bytes, token);
            }
            finally
            {
                _isFlushing = false;
            }
        }

        public async Task AliveCheckTaskAsync(CancellationTokenSource source, CancellationToken token)
        {
            try
            {
                var failureCount = 0;

                while (!token.IsCancellationRequested)
                {
                    // クライアントが接続されているか確認
                    if (Client.Connected)
                    {
                        var isDisconnected = Client.Client.Poll(1000, SelectMode.SelectRead) && Client.Client.Available == 0;

                        if (isDisconnected)
                        {
                            failureCount++;

                            if (failureCount > 10)
                            {
                                _logger.LogInformation("Client disconnected.");
                                break; // Whileループを抜ける
                            }
                        }
                        else
                        {
                            failureCount = 0;
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Client not connected.");
                        break; // Whileループを抜ける
                    }

                    await Task.Delay(1000, token); // 次のチェックまで待機
                }
            }
            catch (OperationCanceledException) { } // Ignore
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking client alive status.");
            }
            finally
            {
                source.Cancel(); // CancellationTokenSourceをキャンセルして、他の待機中のタスクに終了を通知
            }
        }

        public async ValueTask CallRpcAsync<T>(string name, T args, CancellationToken token)
        {
            var data = new RpcData
            {
                Name = name,
                Args = MemoryPackSerializer.Serialize(args)
            };
            var packet = new Packet(PacketType.RpcRequest, MemoryPackSerializer.Serialize(data));
            await SendAsync(packet, token: token);
        }

        public async ValueTask<T2?> CallRpcAsync<T1, T2>(string name, T1 args, CancellationToken token)
        {
            var data = new RpcData
            {
                Name = name,
                Args = MemoryPackSerializer.Serialize(args)
            };
            var packet = new Packet(PacketType.RpcRequest, MemoryPackSerializer.Serialize(data));

            var tcs = new TaskCompletionSource<T2?>();
            PacketReceived += RpcReceived;

            await SendAsync(packet, true, token);

            return await tcs.Task;

            void RpcReceived(Packet p)
            {
                if (p.Type != PacketType.RpcResponse && p.Type != PacketType.RpcError)
                {
                    return;
                }

                var res = MemoryPackSerializer.Deserialize<RpcData>(p.Data);
                if (res.Name != name)
                {
                    return;
                }

                if(p.Type == PacketType.RpcError)
                {
                    tcs.SetException(new RpcException($"RPC {res.Name} error"));
                }
                else
                {
                    tcs.SetResult(MemoryPackSerializer.Deserialize<T2>(res.Args));
                }

                PacketReceived -= RpcReceived;
            }
        }

        private async Task ReceiveLoopAsync(CancellationToken token)
        {
            _logger.LogDebug("Receive loop started");
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            AliveCheckTaskAsync(cts, cts.Token).Forget(_logger);

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var sizeBytes = new byte[sizeof(int)];
                    await Stream.ReceiveAsync(sizeBytes, cts.Token);
                    var size = BitConverter.ToInt32(sizeBytes);

                    var buffer = new byte[size];
                    await Stream.ReceiveAsync(buffer, cts.Token);

                    var packets = MemoryPackSerializer.Deserialize<List<Packet>>(buffer);
                    if (packets != null)
                    {
                        foreach (var packet in packets)
                        {
                            OnPacketReceived(packet);
                        }
                    }
                }
            }
            catch (CloseConnectionException)
            {
                _logger.LogDebug("Connection closed by the peer");
                throw;
            }
            catch (IOException e)
            {
                _logger.LogDebug(e, "Error in receive loop. Includes some that are not a problem.");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in receive loop");
                throw;
            }
            finally
            {
                _logger.LogDebug("Receive loop stopped");
                OnDisconnected();
                cts.Cancel();
            }
        }

        private void OnPacketReceived(Packet packet)
        {
            PacketReceived?.Invoke(packet);
        }

        private void OnDisconnected()
        {
            Disconnected?.Invoke();
        }
    }
}