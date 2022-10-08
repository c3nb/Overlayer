using System;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Network
{
    [PublicAPI]
    internal class IPv4Network
    {
        private readonly uint networkBegin;
        private readonly uint networkBroadcast;

        public IPv4Network(IPAddress networkAddress, byte networkCidr)
        {
            if (networkAddress == null)
                throw new ArgumentNullException(nameof(networkAddress));

            if (networkAddress.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException("Network address must be an IPv4 address.", nameof(networkAddress));

            if (networkCidr > 32)
                throw new ArgumentOutOfRangeException(nameof(networkCidr), "Network CIDR must be in [0; 32] range.");

            NetworkAddress = networkAddress;
            NetworkCidr = networkCidr;

            var networkAddressNumber = networkAddress.ToUInt32();

            networkBegin = networkAddressNumber & (uint.MaxValue << (32 - networkCidr));

            if (networkCidr == 32)
            {
                networkBroadcast = networkBegin;
            }
            else
            {
                networkBroadcast = networkAddressNumber | (uint.MaxValue >> networkCidr);
            }
        }

        public IPAddress NetworkAddress { get; }

        public byte NetworkCidr { get; }

        public static bool TryParse(string input, out IPv4Network network)
        {
            network = null;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            var slashIndex = input.IndexOf('/');
            if (slashIndex < 0)
                return false;

            if (!IPAddress.TryParse(input.Substring(0, slashIndex), out var networkAddress))
                return false;

            if (networkAddress.AddressFamily != AddressFamily.InterNetwork)
                return false;

            if (!byte.TryParse(input.Substring(slashIndex + 1), out var networkCidr))
                return false;

            if (networkCidr > 32)
                return false;

            network = new IPv4Network(networkAddress, networkCidr);

            return true;
        }

        public static IPv4Network Parse(string input)
        {
            if (!TryParse(input, out var network))
                throw new FormatException($"Failed to parse IPv4 network from string '{input}'.");

            return network;
        }

        public bool Contains(IPAddress address)
        {
            if (address.IsIPv4MappedToIPv6)
                address = address.MapToIPv4();

            if (address.AddressFamily != AddressFamily.InterNetwork)
                return false;

            var rawAddress = address.ToUInt32();

            return rawAddress >= networkBegin && rawAddress <= networkBroadcast;
        }

        public override string ToString() => NetworkAddress + "/" + NetworkCidr;
    }
}