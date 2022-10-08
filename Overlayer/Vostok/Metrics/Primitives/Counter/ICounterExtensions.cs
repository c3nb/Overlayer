using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Counter
{
    [PublicAPI]
    public static class ICounterExtensions
    {
        /// <summary>
        /// Increment the value of given <paramref name="counter"/> by 1.
        /// </summary>
        public static void Increment([NotNull] this ICounter counter)
            => counter.Add(1L);
    }
}