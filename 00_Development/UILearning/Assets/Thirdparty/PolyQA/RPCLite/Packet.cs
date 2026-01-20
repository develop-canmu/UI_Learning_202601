#nullable enable
using System;
using MemoryPack;
using MemoryPack.Formatters;

namespace Polyscape.RPCLite
{
    public enum PacketType : byte
    {
        RpcRequest,
        RpcResponse,
        RpcError,
        RawBytes,
    }

    [MemoryPackable]
    // 他のプラットフォームなどを考えるとMemoryPackに任せるべきじゃないが、今は他のSerializeも依存しているので
    public readonly partial struct Packet
    {
        public readonly PacketType Type;
        public readonly byte[] Data;

        public Packet(PacketType type, byte[] data)
        {
            Type = type;
            Data = data;
        }

        public int Size => 1 + sizeof(int) + Data.Length;

        // public async ValueTask SendAsync(ClientInfo client, CancellationToken token)
        // {
        //     await client.SendAsync(this, token);
        //     var size = 1 + sizeof(int) + Data.Length;
        //     var workBytes = ArrayPool<byte>.Shared.Rent(size);
        //     try
        //     {
        //         workBytes[0] = (byte)Type;
        //
        //         BitConverter.TryWriteBytes(workBytes.AsSpan(1), Data.Length);
        //         Array.Copy(Data, 0, workBytes, 1 + sizeof(int), Data.Length);
        //
        //         await client.SendAsync(workBytes.AsMemory(0, size), token);
        //
        //     }
        //     finally
        //     {
        //         ArrayPool<byte>.Shared.Return(workBytes);
        //     }
        // }

        // public static async ValueTask<Packet> ReceiveAsync(ClientInfo client, CancellationToken token)
        // {
        //     var workBytes = ArrayPool<byte>.Shared.Rent(1 + sizeof(int));
        //
        //     PacketType type;
        //     int length;
        //     try
        //     {
        //         await client.Stream.ReceiveAsync(workBytes.AsMemory(0, 1 + sizeof(int)), token);
        //         type = (PacketType)workBytes[0];
        //         length = BitConverter.ToInt32(workBytes, 1);
        //     }
        //     finally
        //     {
        //         ArrayPool<byte>.Shared.Return(workBytes);
        //     }
        //     var data = new byte[length];
        //     await client.Stream.ReceiveAsync(data, token);
        //
        //     return new Packet(type, data);
        // }

#if UNITY_2021
        /// <summary>
        /// Unity 2021のIL2CPPでAOTコード生成を行うためのメソッド
        /// https://docs.unity3d.com/2021.3/Documentation/Manual/ScriptingRestrictions.html
        /// </summary>
        private void UsedOnlyForAOTCodeGeneration()
        {
            var _ = new ListFormatter<Packet>();

            throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
        }
#endif
    }
}