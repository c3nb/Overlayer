using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Network
{
    /// <summary>
    /// Helper class to discover free TCP ports.
    /// </summary>
    [PublicAPI]
    internal static class FreeTcpPortFinder
    {
        /// <summary>
        /// Returns a currently available TCP port to bind on.
        /// </summary>
        public static int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}