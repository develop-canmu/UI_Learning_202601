using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using PolyQA.Extensions;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Agent.Resolver
{
    public class DeepLinkResolver : IAgentResolveStrategy
    {
        private readonly ILogger _logger = Logging.CreateLogger<DeepLinkResolver>();
        
        public ValueTask<Agent> Resolve()
        {
            var url = Application.absoluteURL;
            if (string.IsNullOrEmpty(url))
            {
                return new ValueTask<Agent>();
            }
            
            var uri = new Uri(Application.absoluteURL);
            var query = HttpUtility.ParseQueryString(uri.Query);
            var address = query["polyqa_address"];

            if (string.IsNullOrEmpty(address))
            {
                return new ValueTask<Agent>();
            }

            if (!address.Contains(","))
            {
                Save.AgentIpAddress = address;
                _logger.LogInformation("Use IP {address} obtained from deep link", address);
                return new ValueTask<Agent>(new Agent(address));
            }

            var agentAddresses = address.Split(',');
            var sameNetworkAddress = FindSameNetworkAddress(agentAddresses);
            if (string.IsNullOrEmpty(sameNetworkAddress))
            {
                return new ValueTask<Agent>();
            }

            Save.AgentIpAddress = sameNetworkAddress;
            _logger.LogInformation("Use IP {sameNetworkAddress} obtained from deep link", sameNetworkAddress);
            return new ValueTask<Agent>(new Agent(sameNetworkAddress));
        }

        private string FindSameNetworkAddress(string[] agentAddresses)
        {
            var agentIpAddresses = ToIPAddress(agentAddresses);
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                var unicastAddresses = networkInterface.GetIPProperties().UnicastAddresses;
                foreach (var unicastAddress in unicastAddresses)
                {
                    if (unicastAddress.Address.AddressFamily != AddressFamily.InterNetwork)
                    {
                        continue;
                    }

                    foreach (var agentIpAddress in agentIpAddresses)
                    {
                        if (IsInSameNetwork(unicastAddress, agentIpAddress))
                        {
                            return agentIpAddress.ToString();
                        }
                    }
                }
            }

            return null;
        }

        private IPAddress[] ToIPAddress(string[] addresses)
        {
            return addresses
                .Select(x =>
                {
                    IPAddress.TryParse(x, out var address);
                    return address;
                })
                .Where(x => x != null)
                .ToArray();
        }

        private bool IsInSameNetwork(UnicastIPAddressInformation unicastAddress, IPAddress agentAddress)
        {
            return agentAddress.IsInSameSubnet(unicastAddress.Address, unicastAddress.IPv4Mask);
        }
    }
}