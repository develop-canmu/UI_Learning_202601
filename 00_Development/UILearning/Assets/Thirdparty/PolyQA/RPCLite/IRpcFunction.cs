#nullable enable
using System;
using System.Threading.Tasks;
using MemoryPack;

namespace Polyscape.RPCLite
{
    public interface IRpcFunction
    {
        string Name { get; }
        Type ResultType { get; }
    }

    public interface ISyncRpcFunction : IRpcFunction
    {
        object? Invoke(int clientId, byte[] args);
    }

    public interface IAsyncRpcFunction : IRpcFunction
    {
        ValueTask<object?> InvokeAsync(int clientId, byte[] args);
    }

    public class RpcFunction<TResult, TArgs> : ISyncRpcFunction
    {
        public RpcFunction(string name, Func<TArgs, int, TResult> function)
        {
            Name = name;
            Function = function;
        }

        public string Name { get; }
        public Type ResultType => typeof(TResult);
        public object? Invoke(int clientId, byte[] args)
        {
            return Function(MemoryPackSerializer.Deserialize<TArgs>(args)!, clientId);
        }

        private Func<TArgs, int, TResult> Function { get; }
    }

    public class AsyncRpcFunction<TResult, TArgs> : IAsyncRpcFunction
    {
        public AsyncRpcFunction(string name, Func<TArgs, int, ValueTask<TResult>> function)
        {
            Name = name;
            Function = function;
        }

        public string Name { get; }
        public Type ResultType => typeof(TResult);
        public async ValueTask<object?> InvokeAsync(int clientId, byte[] args)
        {
            var result = await Function(MemoryPackSerializer.Deserialize<TArgs>(args)!, clientId);
            return result;
        }

        private Func<TArgs, int, ValueTask<TResult>> Function { get; }
    }

    public class RpcAction<TArgs> : ISyncRpcFunction
    {
        public RpcAction(string name, Action<TArgs, int> action)
        {
            Name = name;
            Action = action;
        }

        public string Name { get; }
        public Type ResultType => typeof(void);
        public object? Invoke(int clientId, byte[] args)
        {
            Action(MemoryPackSerializer.Deserialize<TArgs>(args)!, clientId);
            return null;
        }

        private Action<TArgs, int> Action { get; }
    }
}