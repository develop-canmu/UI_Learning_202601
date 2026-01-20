using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Agent.Resolver
{
    public class CommandLineArgumentsResolver : IAgentResolveStrategy
    {
        private readonly ILogger _logger = Logging.CreateLogger<CommandLineArgumentsResolver>();
        
        public ValueTask<Agent> Resolve()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            
            var address = intent.Call<string>("getStringExtra", "polyqa_address");
            if (!string.IsNullOrEmpty(address))
            {
                Save.AgentIpAddress = address;
                _logger.LogInformation("Use IP {address} obtained from command line", address);
                return new ValueTask<Agent>(new Agent(address));
            }
#elif UNITY_STANDALONE
            var args = System.Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-polyqa_address" && i + 1 < args.Length)
                {
                    var address = args[i + 1];
                    Save.AgentIpAddress = address;
                    _logger.LogInformation("Use IP {address} obtained from command line", address);
                    return new ValueTask<Agent>(new Agent(address));
                }
            }
#endif
            return new ValueTask<Agent>();
        }
    }
}