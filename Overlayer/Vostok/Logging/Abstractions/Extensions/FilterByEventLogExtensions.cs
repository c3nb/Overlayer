using System;
using JetBrains.Annotations;

namespace Vostok.Logging.Abstractions
{
    [PublicAPI]
    public static class FilterByEventLogExtensions
    {
        /// <summary>
        /// <para>Returns a wrapper log that only logs events matched by <paramref name="allow"/> predicate.</para>
        /// </summary>
        [Pure]
        public static ILog SelectEvents([NotNull] this ILog log, [NotNull] Predicate<LogEvent> allow)
        {
            return new EventFilterLog(log, allow, true);
        }

        /// <summary>
        /// <para>Returns a wrapper log that drops events matched by <paramref name="reject"/> predicate.</para>
        /// </summary>
        [Pure]
        public static ILog DropEvents([NotNull] this ILog log, [NotNull] Predicate<LogEvent> reject)
        {
            return new EventFilterLog(log, reject, false);
        }

        private class EventFilterLog : ILog
        {
            private readonly ILog baseLog;
            private readonly Predicate<LogEvent> criterion;
            private readonly bool criterionAllowsEvents;

            public EventFilterLog(ILog baseLog, Predicate<LogEvent> criterion, bool criterionAllowsEvents)
            {
                this.baseLog = baseLog ?? throw new ArgumentNullException(nameof(baseLog));
                this.criterion = criterion ?? throw new ArgumentNullException(nameof(criterion));
                this.criterionAllowsEvents = criterionAllowsEvents;
            }

            public void Log(LogEvent @event)
            {
                var criterionMatches = criterion(@event);
                if (criterionMatches == criterionAllowsEvents)
                {
                    baseLog.Log(@event);
                }
            }

            public bool IsEnabledFor(LogLevel level)
            {
                return baseLog.IsEnabledFor(level);
            }

            public ILog ForContext(string context)
            {
                var baseLogForContext = baseLog.ForContext(context);

                return ReferenceEquals(baseLogForContext, baseLog) ? this : new EventFilterLog(baseLogForContext, criterion, criterionAllowsEvents);
            }
        }
    }
}
