using System.Threading.Tasks;
using PolyQA.Agent.Resolver;

namespace PolyQA.Agent
{
    public class AgentResolver
    {
        private const string LocalhostIPAddress = "127.0.0.1";

        private readonly IAgentResolveStrategy[] _strategies =
        {
            new DeepLinkResolver(),
            new CommandLineArgumentsResolver(),
            new SavedAgentResolver(),
            new UdpBroadcastResolver(),
        };
        
        public async ValueTask<Agent> Resolve()
        {
            foreach (var strategy in _strategies)
            {
                var agent = await strategy.Resolve();
                if (agent != null)
                {
                    return agent;
                }
            }

            return new Agent(LocalhostIPAddress);
        }
    }
}