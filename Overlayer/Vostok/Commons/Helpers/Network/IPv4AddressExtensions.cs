using System;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Network
{
    [PublicAPI]
    internal static class IPv4AddressExtensions
    {
        public static uint ToUInt32(this IPAddress address)
        {
            if (address.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException("Address must be an IPv4 address.", nameof(address));

#pragma warning disable 618
            var rawAddress = (uint)address.Address;
#pragma warning restore 618

            return
                ((rawAddress & 0x000000FFU) << 24) |
                ((rawAddress & 0x0000FF00U) << 8) |
                ((rawAddress & 0x00FF0000U) >> 8) |
                ((rawAddress & 0xFF000000U) >> 24);
        }

        public static uint ToUInt32BigEndian(this IPAddress address)
        {
            if (address.AddressFamily != AddressFamily.InterNetwork)
                throw new ArgumentException("Address must be an IPv4 address.", nameof(address));

#pragma warning disable 618
            return (uint)address.Address;
#pragma warning restore 618
        }
    }
}