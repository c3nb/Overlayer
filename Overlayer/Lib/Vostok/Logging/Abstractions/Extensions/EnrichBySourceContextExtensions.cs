using System;
using JetBrains.Annotations;

namespace Vostok.Logging.Abstractions
{
    [PublicAPI]
    public static class EnrichBySourceContextExtensions
    {
        /// <summary>
        /// <para>Returns a wrapper log that enriches events made by log with given <paramref name="context"/> passed to <see cref="ILog.ForContext(string)"/> using given <paramref name="enricher"/> delegate.</para>
        /// <para>Provided context value is treated as a case-insensitive prefix.</para>
        /// </summary>
        [Pure]
        public static ILog EnrichBySourceContext([NotNull] this ILog log, [NotNull] string context, [NotNull] Func<ILog, ILog> enricher)
            => new EnrichBySourceContextLog(log, enricher, context);

        private class EnrichBySourceContextLog : ILog
        {
            private readonly ILog baseLog;
            private readonly Func<ILog, ILog> enricher;
            private readonly string contextFilterValue;

            public EnrichBySourceContextLog(ILog baseLog, Func<ILog, ILog> enricher, string contextFilterValue)
            {
                this.baseLog = baseLog ?? throw new ArgumentNullException(nameof(baseLog));
                this.enricher = enricher ?? throw new ArgumentNullException(nameof(enricher));
                this.contextFilterValue = contextFilterValue ?? throw new ArgumentNullException(nameof(contextFilterValue));
            }

            public void Log(LogEvent @event)
                => baseLog.Log(@event);

            public bool IsEnabledFor(LogLevel level) 
                => baseLog.IsEnabledFor(level);

            public ILog ForContext(string context)
            {
                var baseLogForContext = baseLog.ForContext(context);

                if (context.StartsWith(contextFilterValue, StringComparison.OrdinalIgnoreCase))
                    return new EnrichBySourceContextLog(enricher(baseLogForContext), enricher, contextFilterValue);

                return ReferenceEquals(baseLogForContext, baseLog)
                    ? this
                    : new EnrichBySourceContextLog(baseLogForContext, enricher, contextFilterValue);
            }
        }
    }
}
