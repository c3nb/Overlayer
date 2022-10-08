using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Network
{
    [PublicAPI]
    internal static class NetworkHelper
    {
        public static volatile List<IPAddress> GatewayAddresses;

        public static volatile List<IPv4Network> LocalNetworks;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromDays(1);

        static NetworkHelper()
        {
            GatewayAddresses = new List<IPAddress>();
            LocalNetworks = new List<IPv4Network>();

            UpdateAndSchedule();
        }

        private static void UpdateAndSchedule()
        {
            Update();

            Task.Delay(CacheTtl).ContinueWith(_ => UpdateAndSchedule());
        }

        private static void Update()
        {
            try
            {
                var interfaces = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(iface => iface.OperationalStatus == OperationalStatus.Up)
                    .Where(iface => iface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .Where(iface => iface.Supports(NetworkInterfaceComponent.IPv4));

                var newGatewayAddresses = new List<IPAddress>();
                var newLocalNetworks = new List<IPv4Network>();

                foreach (var iface in interfaces)
                {
                    var ipProperties = iface.GetIPProperties();

                    var gatewayAddresses = ipProperties.GatewayAddresses
                        .Select(gw => gw.Address)
                        .ToArray();

                    var unicastAddresses = ipProperties.UnicastAddresses
                        .Where(uni => uni.Address.AddressFamily == AddressFamily.InterNetwork)
                        .ToArray();

                    if (gatewayAddresses.Length == 0 || unicastAddresses.Length == 0)
                        continue;

                    newGatewayAddresses.AddRange(gatewayAddresses);
                    newLocalNetworks.AddRange(unicastAddresses.Select(uni => new IPv4Network(uni.Address, (byte)uni.PrefixLength)));
                }

                GatewayAddresses = newGatewayAddresses;
                LocalNetworks = newLocalNetworks;
            }
            catch
            {
                GatewayAddresses = new List<IPAddress>();
                LocalNetworks = new List<IPv4Network>();
            }
        }
    }
}