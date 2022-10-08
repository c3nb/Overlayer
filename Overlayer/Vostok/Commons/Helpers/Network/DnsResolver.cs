using System;
using System.Collections.Concurrent;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Network
{
    [PublicAPI]
    internal class DnsResolver
    {
        private static readonly IPAddress[] EmptyAddresses = {};

        private readonly TimeSpan cacheTtl;
        private readonly TimeSpan resolveTimeout;

        private readonly ConcurrentDictionary<string, (IPAddress[] addresses, DateTime validTo)> cache;
        private readonly ConcurrentDictionary<string, Lazy<Task<IPAddress[]>>> initialUpdateTasks;

        private int isUpdatingNow;

        public DnsResolver(TimeSpan cacheTtl, TimeSpan resolveTimeout)
        {
            this.cacheTtl = cacheTtl;
            this.resolveTimeout = resolveTimeout;

            cache = new ConcurrentDictionary<string, (IPAddress[] addresses, DateTime validTo)>(StringComparer.OrdinalIgnoreCase);
            initialUpdateTasks = new ConcurrentDictionary<string, Lazy<Task<IPAddress[]>>>(StringComparer.OrdinalIgnoreCase);
        }

        public IPAddress[] Resolve(string hostname, bool canWait)
        {
            var currentTime = DateTime.UtcNow;

            if (cache.TryGetValue(hostname, out var cacheEntry))
            {
                if (cacheEntry.validTo < currentTime &&
                    Interlocked.CompareExchange(ref isUpdatingNow, 1, 0) == 0)
                {
                    StartResolveAndUpdateTask(hostname, currentTime);
                }

                return cacheEntry.addresses;
            }

            return HandleEmptyCache(hostname, currentTime, canWait);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IPAddress[] HandleEmptyCache(string hostname, DateTime currentTime, bool canWait)
        {
            //(deniaa): Do not inline this method because it prevents from creating unnecessary lambda closures
            // in case item exists in cache.
            var resolveTaskLazy = initialUpdateTasks.GetOrAdd(
                hostname,
                s => new Lazy<Task<IPAddress[]>>(
                    () => ResolveAndUpdateCacheAsync(s, currentTime),
                    LazyThreadSafetyMode.ExecutionAndPublication));

            var resolveTask = resolveTaskLazy.Value;

            if (!canWait)
                return resolveTask.IsCompleted ? resolveTask.GetAwaiter().GetResult() : EmptyAddresses;

            return resolveTask.Wait(resolveTimeout)
                ? resolveTask.GetAwaiter().GetResult()
                : EmptyAddresses;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void StartResolveAndUpdateTask(string hostname, DateTime currentTime)
        {
            //(deniaa): Do not inline this method because it prevents from creating unnecessary lambda closures
            // in case item exists in cache.
            Task.Run(
                async () =>
                {
                    try
                    {
                        await ResolveAndUpdateCacheAsync(hostname, currentTime).ConfigureAwait(false);
                    }
                    finally
                    {
                        Interlocked.Exchange(ref isUpdatingNow, 0);
                    }
                });
        }

        [ItemCanBeNull]
        private static async Task<IPAddress[]> TryResolveInternal(string hostname)
        {
            try
            {
                return await Dns.GetHostAddressesAsync(hostname).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        private async Task<IPAddress[]> ResolveAndUpdateCacheAsync(string hostname, DateTime currentTime)
        {
            var addresses = await TryResolveInternal(hostname).ConfigureAwait(false);
            if (addresses != null)
                cache[hostname] = (addresses, currentTime + cacheTtl);

            return addresses ?? EmptyAddresses;
        }
    }
}