using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Timer
{
    [PublicAPI]
    public static class ITimerExtensions_Measurement
    {
        /// <inheritdoc cref="ITimer.Report"/>
        public static void Report([NotNull] this ITimer timer, TimeSpan value)
            => timer.Report(TimeValuesConverter.ConvertOrThrow(value, timer.Unit));

        public static IDisposable Measure([NotNull] this ITimer timer)
            => new Measurement(timer);

        public static Measurement MeasureElapsed([NotNull] this ITimer timer)
            => new Measurement(timer);

        public static T MeasureSync<T>([NotNull] this ITimer timer, [NotNull] Func<T> func)
        {
            using (timer.Measure())
                return func();
        }

        public static void MeasureSync([NotNull] this ITimer timer, [NotNull] Action action)
        {
            using (timer.Measure())
                action();
        }

        public static async Task<T> MeasureAsync<T>([NotNull] this ITimer timer, [NotNull] Func<Task<T>> func)
        {
            using (timer.Measure())
                return await func().ConfigureAwait(false);
        }

        public static async Task MeasureAsync([NotNull] this ITimer timer, [NotNull] Func<Task> action)
        {
            using (timer.Measure())
                await action().ConfigureAwait(false);
        }
    }
}