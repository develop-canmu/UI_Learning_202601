using System.Threading.Tasks;

namespace PolyQA.Agent
{
    public interface IAgentResolveStrategy
    {
        public ValueTask<Agent> Resolve();
    }
}