#nullable enable
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Polyscape.RPCLite
{
    public static class NetworkStreamExtensions
    {
        /// <summary>
        /// 絶対に指定したバッファサイズ分読み込む
        /// </summary>
        public static ValueTask ReceiveAsync(this NetworkStream stream, byte[] buffer, CancellationToken token) => ReceiveAsync(stream, buffer.AsMemory(), token);

        public static async ValueTask ReceiveAsync(this NetworkStream stream, Memory<byte> buffer, CancellationToken token)
        {
            var read = 0;
            while (read < buffer.Length)
            {
                var async = await stream.ReadAsync(buffer[read..], token);
                read += async;
                
                if (async == 0)
                {
                    throw new CloseConnectionException("Connection closed by the peer");
                }
            }
        }
    }
}