#nullable enable
using System;

namespace Polyscape.RPCLite
{
    public class CloseConnectionException : Exception
    {
        public CloseConnectionException(string? message = null) : base(message) { }
    }

    public class RpcException : Exception
    {
        public RpcException(string? message = null) : base(message) { }
    }
}