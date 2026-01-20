using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PolyQA.Agent.Resolver
{
    public class UdpBroadcastResolver : IAgentResolveStrategy
    {
        private static readonly TimeSpan TimeoutDuration = TimeSpan.FromSeconds(3);
        private readonly ILogger _logger = Logging.CreateLogger<UdpBroadcastResolver>();
        
        public async ValueTask<Agent> Resolve()
        {
            var client = new UdpClient();
            var request = Encoding.ASCII.GetBytes("Looking for PolyQA");
            client.EnableBroadcast = true;
            await client.SendAsync(request, request.Length, new IPEndPoint(IPAddress.Broadcast, Const.SearchPort));
            
            try
            {
                var result = await client.ReceiveAsync().WaitAsync(TimeoutDuration, TimeProvider.System);
                var address = result.RemoteEndPoint.Address.ToString();
                _logger.LogInformation("[PolyQA] Use IP {address} obtained from UDP broadcast", address);
                return new Agent(address);
            }
            catch (TimeoutException)
            {
                // ignored
            }
            
            return null;
        }
    }
}