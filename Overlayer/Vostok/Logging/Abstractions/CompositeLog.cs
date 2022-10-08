using System;
using JetBrains.Annotations;

namespace Vostok.Logging.Abstractions
{
    /// <summary>
    /// A log that passes messages on to all of the base logs.
    /// </summary>
    [PublicAPI]
    public class CompositeLog : ILog
    {
        private readonly ILog[] baseLogs;

        public CompositeLog(params ILog[] baseLogs)
        {
            this.baseLogs = baseLogs ?? throw new ArgumentNullException(nameof(baseLogs));
        }

        public void Log(LogEvent @event)
        {
            foreach (var baseLog in baseLogs)
                baseLog.Log(@event);
        }

        public bool IsEnabledFor(LogLevel level)
        {
            foreach (var log in baseLogs)
                if (log.IsEnabledFor(level))
                    return true;

            return false;
        }

        public ILog ForContext(string context)
        {
            var baseLogsForContext = new ILog[baseLogs.Length];
            var sameContext = true;

            for (var i = 0; i < baseLogs.Length; i++)
            {
                baseLogsForContext[i] = baseLogs[i].ForContext(context);
                if (baseLogsForContext[i] != baseLogs[i])
                    sameContext = false;
            }

            return sameContext ? this : new CompositeLog(baseLogsForContext);
        }
    }
}
