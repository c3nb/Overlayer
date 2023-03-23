using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

// ReSharper disable PossibleNullReferenceException

namespace Vostok.Commons.Time
{
    /// <summary>
    /// <para><see cref="PreciseDateTime"/> is an alternative to built-in <see cref="DateTimeOffset"/> with better precision in most environments.</para>
    /// <para>Despite generally providing better precision, it's <b>not any more accurate</b> and <b>not monotonic</b>!</para>
    /// <para>It's based on following techniques:</para>
    /// <list type="number">
    ///     <item><description><c>GetSystemTimePreciseAsFileTime</c> system call on Windows 8 or 2012 and later.</description></item>
    ///     <item><description>Combination of <see cref="DateTimeOffset.UtcNow"/> and <see cref="Stopwatch.GetTimestamp"/> if <see cref="Stopwatch"/> uses a high-precision timer.</description></item>
    ///     <item><description>Plain old <see cref="DateTimeOffset.UtcNow"/> if nothing of above works.</description></item>
    /// </list>
    /// </summary>
    [PublicAPI]
    internal static class PreciseDateTime
    {
        private static readonly Func<DateTimeOffset> utcNowProvider;
        private static volatile OffsetHolder offsetFromUtcHolder;

        static PreciseDateTime()
        {
            utcNowProvider = SelectTimestampProvider();

            UpdateOffsetFromUtc();
        }

        public static DateTimeOffset UtcNow => utcNowProvider();

        public static DateTimeOffset Now
        {
            get
            {
                var offsetFromUtc = offsetFromUtcHolder.Value;

                return new DateTimeOffset(utcNowProvider().DateTime + offsetFromUtc, offsetFromUtc);
            }
        }

        private static Func<DateTimeOffset> SelectTimestampProvider()
        {
            if (WinApiProvider.IsSupported)
                return WinApiProvider.Obtain;

            if (StopwatchProvider.IsSupported)
                return StopwatchProvider.Obtain;

            return () => DateTimeOffset.UtcNow;
        }

        private static void UpdateOffsetFromUtc()
        {
            Interlocked.Exchange(ref offsetFromUtcHolder, new OffsetHolder(DateTimeOffset.Now.Offset));

            Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(_ => UpdateOffsetFromUtc());
        }

        #region WinApiProvider

        private static class WinApiProvider
        {
            static WinApiProvider()
            {
                IsSupported = CanObtain();
            }

            public static bool IsSupported { get; }

            public static DateTimeOffset Obtain()
            {
                GetSystemTimePreciseAsFileTime(out var fileTime);

                return new DateTimeOffset(DateTime.FromFileTimeUtc(fileTime), TimeSpan.Zero);
            }

            private static bool CanObtain()
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return false;

                try
                {
                    Obtain();
                }
                catch
                {
                    return false;
                }

                return true;
            }

            [DllImport("kernel32.dll")]
            private static extern void GetSystemTimePreciseAsFileTime(out long fileTime);
        }

        #endregion

        #region StopwatchProvider

        private static class StopwatchProvider
        {
            private static readonly double TickFrequency = (double)TimeSpan.TicksPerSecond / Stopwatch.Frequency;

            private static readonly long SyncPeriodTicks = TimeSpan.FromSeconds(2).Ticks;

            private static volatile State state;

            public static bool IsSupported => Stopwatch.IsHighResolution;

            public static DateTimeOffset Obtain()
            {
                long currentStopwatchDiff;

                var currentStopwatchTicks = Stopwatch.GetTimestamp();
                var currentState = state;

                if (currentState == null || (currentStopwatchDiff = ToDateTimeTicks(currentStopwatchTicks - currentState.BaseStopwatchTicks)) >= SyncPeriodTicks)
                {
                    var newState = new State(DateTime.UtcNow.Ticks, currentStopwatchTicks);

                    var exchangeResult = Interlocked.CompareExchange(ref state, newState, currentState);

                    currentState = ReferenceEquals(exchangeResult, currentState) ? newState : exchangeResult;

                    currentStopwatchDiff = ReferenceEquals(currentState, newState) ? 0L : Stopwatch.GetTimestamp() - currentState.BaseStopwatchTicks;
                }

                return ToDateTimeOffset(currentState.BaseDateTimeTicks + currentStopwatchDiff);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static long ToDateTimeTicks(double stopwatchTicks)
            {
                stopwatchTicks *= TickFrequency;

                return unchecked((long)stopwatchTicks);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static DateTimeOffset ToDateTimeOffset(long dateTimeTicks)
            {
                return new DateTimeOffset(dateTimeTicks, TimeSpan.Zero);
            }

            private class State
            {
                public readonly long BaseDateTimeTicks;
                public readonly long BaseStopwatchTicks;

                public State(long baseDateTimeTicks, long baseStopwatchTicks)
                {
                    BaseDateTimeTicks = baseDateTimeTicks;
                    BaseStopwatchTicks = baseStopwatchTicks;
                }
            }
        }

        #endregion

        #region OffsetHolder

        private class OffsetHolder
        {
            public readonly TimeSpan Value;

            public OffsetHolder(TimeSpan value)
                => Value = value;
        }

        #endregion
    }
}