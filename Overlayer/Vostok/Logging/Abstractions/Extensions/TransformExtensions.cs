using System;
using JetBrains.Annotations;

namespace Vostok.Logging.Abstractions
{
    [PublicAPI]
    public static class TransformExtensions
    {
        /// <summary>
        /// <para>Returns a wrapper transforms log events.</para>
        /// <para>Does not support <see cref="LogLevel"/> transformations.</para>
        /// </summary>
        [Pure]
        public static ILog WithTransformation([NotNull] this ILog log, [NotNull] Func<LogEvent, LogEvent> transform)
            => new TransformLog(log, transform);

        private class TransformLog : ILog
        {
            private readonly ILog baseLog;
            private readonly Func<LogEvent, LogEvent> transform;

            public TransformLog(ILog baseLog, Func<LogEvent, LogEvent> transform)
            {
                this.baseLog = baseLog ?? throw new ArgumentNullException(nameof(baseLog));
                this.transform = transform ?? throw new ArgumentNullException(nameof(transform));
            }

            public void Log(LogEvent @event)
            {
                if (@event != null)
                    @event = transform(@event);

                baseLog.Log(@event);
            }

            public bool IsEnabledFor(LogLevel level) =>
                baseLog.IsEnabledFor(level);

            public ILog ForContext(string context)
            {
                var baseLogForContext = baseLog.ForContext(context);

                return ReferenceEquals(baseLogForContext, baseLog) ? this : new TransformLog(baseLogForContext, transform);
            }
        }
    }
}
