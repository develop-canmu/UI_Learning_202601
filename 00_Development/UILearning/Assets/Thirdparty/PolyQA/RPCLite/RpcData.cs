#nullable enable
using MemoryPack;

namespace Polyscape.RPCLite
{
    [MemoryPackable]
    public partial struct RpcData
    {
        public string Name;
        public byte[] Args;
    }
}