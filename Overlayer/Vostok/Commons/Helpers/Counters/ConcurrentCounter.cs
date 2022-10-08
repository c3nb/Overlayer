using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Counters;

[PublicAPI]
internal class ConcurrentCounter
{
    private volatile int value;

    public void Increment() => Interlocked.Increment(ref value);

    public int CollectAndReset() => Interlocked.Exchange(ref value, 0);
}