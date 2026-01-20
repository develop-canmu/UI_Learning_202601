#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MemoryPack;
using Microsoft.Extensions.Logging;

namespace Polyscape.RPCLite
{
    public abstract class NetworkService : IDisposable
    {
        protected static readonly byte[] Magic = new byte[8] { 0x52, 0x50, 0x43, 0x4c, 0x69, 0x74, 0x65, 0x00 };

        protected readonly CancellationTokenSource Cts = new ();
        protected readonly ILogger Logger;
        private readonly Dictionary<ClientInfo, int> _clientInfoToId = new();

        public delegate void PacketHandler(ClientInfo client, Packet packet);
        public event PacketHandler? PacketReceived;


        private readonly Dictionary<string, IRpcFunction> Registered = new Dictionary<string, IRpcFunction>();

        protected NetworkService(ILogger logger)
        {
            Logger = logger;
        }

        public void Register<TResult, TArgs>(string name, Func<TArgs, int, TResult> function)
        {
            Registered[name] = new RpcFunction<TResult, TArgs>(name, function);
        }

        public void Register<TResult, TArgs>(string name, Func<TArgs, int, ValueTask<TResult>> function)
        {
            Registered[name] = new AsyncRpcFunction<TResult, TArgs>(name, function);
        }

        public void Register<TArgs>(string name, Action<TArgs, int> function)
        {
            Registered[name] = new RpcAction<TArgs>(name, function);
        }

        // TODO Interface Registration 実装には呼び出し側のInterface実装が存在しなければならないので、SourceGeneratorが必要。MemoryPackのGeneratorと実行順序の依存関係が出来ないようにする必要ある
        // public void Register<TInterface, TImplementation>()
        //     where TImplementation : TInterface, new()
        // {
        //     var methods = typeof(TInterface).GetMethods();
        //     foreach (var method in methods)
        //     {
        //         var name = $"{typeof(TInterface).Name}.{method.Name}";
        //         _registered[name] = new RpcFunction<TInterface, TImplementation>(name, method);
        //     }
        // }

        public virtual ValueTask FlushAsync(CancellationToken token)
        {
            return default;
        }

        public virtual void Dispose()
        {
            Cts.Cancel();

            GC.SuppressFinalize(this);
        }

        protected virtual void OnPacketReceived(ClientInfo client, Packet packet)
        {
            if (packet.Type == PacketType.RpcRequest)
            {
                Task.Run(async () =>
                {
                    var data = MemoryPackSerializer.Deserialize<RpcData>(packet.Data);
                    var method = Registered.GetValueOrDefault(data.Name);

                    if (method != null)
                    {
                        Logger.LogTrace("Method {name} called", data.Name);
                        object? result;
                        try
                        {
                            if (method is ISyncRpcFunction syncMethod)
                            {
                                result = syncMethod.Invoke(ClientInfoToId(client), data.Args);
                            }
                            else if (method is IAsyncRpcFunction asyncMethod)
                            {
                                result = await asyncMethod.InvokeAsync(ClientInfoToId(client), data.Args);
                            }
                            else
                            {
                                result = null;
                                Logger.LogError("Method {name} is not a valid RPC function", data.Name);
                            }
                        }
                        catch (Exception e)
                        {
                            result = null;
                            Logger.LogError(e, "Method {name} failed", data.Name);
                        }
                        if (method.ResultType != typeof(void))
                        {
                            if (result == null)
                            {
                                var resData = data;
                                resData.Args = Array.Empty<byte>();
                                var resPacket = new Packet(PacketType.RpcError, MemoryPackSerializer.Serialize(resData));
                                await client.SendAsync(resPacket, true);
                            }
                            else
                            {
                                var resData = data;
                                resData.Args = MemoryPackSerializer.Serialize(method.ResultType, result);
                                var resPacket = new Packet(PacketType.RpcResponse, MemoryPackSerializer.Serialize(resData));
                                await client.SendAsync(resPacket, true);
                                Logger.LogTrace("Method {name} responded", data.Name);
                            }
                        }
                    }
                    else
                    {
                        Logger.LogError("Method {name} not found", data.Name);
                    }
                }).Forget(Logger);
            }

            PacketReceived?.Invoke(client, packet);
        }

        protected int ClientInfoToId(ClientInfo client)
        {
            if(_clientInfoToId.TryGetValue(client, out var id))
            {
                return id;
            }

            var newId = _clientInfoToId.Count;
            _clientInfoToId[client] = newId;
            return newId;
        }
    }
}