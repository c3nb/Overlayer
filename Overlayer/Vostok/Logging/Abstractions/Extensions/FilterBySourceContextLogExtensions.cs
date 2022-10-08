using JetBrains.Annotations;
using Vostok.Logging.Abstractions.Wrappers;

namespace Vostok.Logging.Abstractions
{
    [PublicAPI]
    public static class FilterBySourceContextLogExtensions
    {
        /// <summary>
        /// <para>Returns a wrapper log that only logs events made by log with given <paramref name="context"/> passed to <see cref="ILog.ForContext(string)"/></para>
        /// <para>Provided context value is treated as a case-insensitive prefix.</para>
        /// </summary>
        [Pure]
        public static ILog WithEventsSelectedBySourceContext([NotNull] this ILog log, [NotNull] string context) =>
            new SourceContextFilterLog(log, new[] {context}, true);

        /// <summary>
        /// <para>Returns a wrapper log that only logs events made by log with context equal to the name of <typeparamref name="T"/> passed to <see cref="ILog.ForContext(string)"/></para>
        /// </summary>
        [Pure]
        public static ILog WithEventsSelectedBySourceContext<T>([NotNull] this ILog log) =>
            WithEventsSelectedBySourceContext(log, typeof(T).Name);

        /// <summary>
        /// <para>Returns a wrapper log that drops events made by log with given <paramref name="context"/> passed to <see cref="ILog.ForContext(string)"/></para>
        /// <para>Provided context value is treated as a case-insensitive prefix.</para>
        /// </summary>
        [Pure]
        public static ILog WithEventsDroppedBySourceContext([NotNull] this ILog log, [NotNull] string context) =>
            new SourceContextFilterLog(log, new[] {context}, false);

        /// <summary>
        /// <para>Returns a wrapper log that drops events made by log with context equal to the name of <typeparamref name="T"/> passed to <see cref="ILog.ForContext(string)"/></para>
        /// </summary>
        [Pure]
        public static ILog WithEventsDroppedBySourceContext<T>([NotNull] this ILog log) =>
            WithEventsDroppedBySourceContext(log, typeof(T).Name);

        /// <summary>
        /// <para>Returns a wrapper log that drops events with level lesser than <paramref name="minLevel"/> made by log with given <paramref name="context"/> passed to <see cref="ILog.ForContext(string)"/></para>
        /// <para>Provided context value is treated as a case-insensitive prefix.</para>
        /// </summary>
        [Pure]
        public static ILog WithMinimumLevelForSourceContext([NotNull] this ILog log, [NotNull] string context, LogLevel minLevel)
            => new SourceContextLevelFilterLog(log, new[] {context}, minLevel);

        /// <summary>
        /// <para>Returns a wrapper log that drops events with level lesser than <paramref name="minLevel"/> made by log with context equal to the name of <typeparamref name="T"/> passed to <see cref="ILog.ForContext(string)"/></para>
        /// </summary>
        [Pure]
        public static ILog WithMinimumLevelForSourceContext<T>([NotNull] this ILog log, LogLevel minLevel)
            => new SourceContextLevelFilterLog(log, new[] {typeof(T).Name}, minLevel);

        /// <summary>
        /// <para>Returns a wrapper log that drops events with level lesser than <paramref name="minLevel"/> made by log with given <paramref name="contexts"/> passed to sequence of <see cref="ILog.ForContext(string)"/></para>
        /// <para>Provided context value is treated as a case-insensitive prefix.</para>
        /// </summary>
        [Pure]
        public static ILog WithMinimumLevelForSourceContexts([NotNull] this ILog log, LogLevel minLevel, [NotNull] [ItemNotNull] params string[] contexts)
            => new SourceContextLevelFilterLog(log, contexts, minLevel);

        private class SourceContextFilterLog : ILog
        {
            private readonly ILog baseLog;
            private readonly string[] contextFilterValues;
            private readonly bool filterAllowsEvent;

            public SourceContextFilterLog(ILog baseLog, string[] contextFilterValues, bool filterAllowsEvent)
            {
                this.baseLog = baseLog;
                this.contextFilterValues = contextFilterValues;
                this.filterAllowsEvent = filterAllowsEvent;
            }

            public void Log(LogEvent @event)
            {
                if (@event.HasMatchingSourceContexts(contextFilterValues) != filterAllowsEvent)
                    return;

                baseLog.Log(@event);
            }

            public bool IsEnabledFor(LogLevel level)
                => baseLog.IsEnabledFor(level);

            public ILog ForContext(string context) =>
                new SourceContextWrapper(this, context);
        }

        private class SourceContextLevelFilterLog : ILog
        {
            private readonly ILog baseLog;
            private readonly string[] contextFilterValues;
            private readonly LogLevel minimumContextLevel;

            public SourceContextLevelFilterLog(ILog baseLog, string[] contextFilterValues, LogLevel minimumContextLevel)
            {
                this.baseLog = baseLog;
                this.contextFilterValues = contextFilterValues;
                this.minimumContextLevel = minimumContextLevel;
            }

            public void Log(LogEvent @event)
            {
                if (@event?.Level < minimumContextLevel && @event.HasMatchingSourceContexts(contextFilterValues))
                    return;

                baseLog.Log(@event);
            }

            public bool IsEnabledFor(LogLevel level)
                => baseLog.IsEnabledFor(level);

            public ILog ForContext(string context) =>
                new SourceContextWrapper(this, context);
        }
    }
}