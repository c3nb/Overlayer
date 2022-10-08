using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Vostok.Metrics.Primitives.Caching
{
    internal class PerContextCache
    {
        private readonly ConcurrentDictionary<(string name, Type type, object details), Lazy<object>> items =
            new ConcurrentDictionary<(string name, Type type, object details), Lazy<object>>();

        public object Obtain(string name, Type type, object details, Func<object> factory)
            => items.GetOrAdd((name, type, details), _ => new Lazy<object>(factory, LazyThreadSafetyMode.ExecutionAndPublication)).Value;
    }
}