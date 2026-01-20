using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PolyQA.Agent.Resolver
{
    public class SavedAgentResolver : IAgentResolveStrategy
    {
        private readonly ILogger _logger = Logging.CreateLogger<SavedAgentResolver>();
        
        public ValueTask<Agent> Resolve()
        {
            var address = Save.AgentIpAddress;
            
            if (string.IsNullOrEmpty(address))
            {
                return new ValueTask<Agent>();
            }

            _logger.LogInformation("Use IP {address} obtained from last connection", address);
            return new ValueTask<Agent>(new Agent(address));
        }
    }
}