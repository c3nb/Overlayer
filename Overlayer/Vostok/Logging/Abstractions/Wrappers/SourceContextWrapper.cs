using System;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions.Values;

namespace Vostok.Logging.Abstractions.Wrappers
{
    /// <summary>
    /// <para>An <see cref="ILog"/> wrapper that helps to implement <see cref="ILog.ForContext"/> method in custom logs efficiently.</para>
    /// <para><see cref="SourceContextWrapper"/> enriches all passing events with a <see cref="WellKnownProperties.SourceContext"/> property with given value.</para>
    /// <para>It's <see cref="ILog.ForContext"/> implementation prevents formation of wrapper chains by unwrapping to a base log that is not a <see cref="SourceContextWrapper"/>.</para>
    /// </summary>
    [PublicAPI]
    public class SourceContextWrapper : ILog
    {
        public SourceContextWrapper([NotNull] ILog log, [NotNull] SourceContextValue context)
        {
            BaseLog = log ?? throw new ArgumentNullException(nameof(log));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public SourceContextWrapper([NotNull] ILog log, [NotNull] string context)
            : this(log, new SourceContextValue(context))
        {
        }

        [NotNull]
        public ILog BaseLog { get; }

        [NotNull]
        public SourceContextValue Context { get; }

        public void Log(LogEvent @event)
        {
            @event = @event?.WithProperty(WellKnownProperties.SourceContext, Context + GetSourceContext(@event), true);

            BaseLog.Log(@event);
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return BaseLog.IsEnabledFor(level);
        }

        public ILog ForContext(string context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return new SourceContextWrapper(UnwrapBaseLog(), Context + context);
        }

        private ILog UnwrapBaseLog()
        {
            var result = BaseLog;

            while (result is SourceContextWrapper wrapper)
            {
                result = wrapper.BaseLog;
            }

            return result;
        }

        [CanBeNull]
        private SourceContextValue GetSourceContext([CanBeNull] LogEvent @event)
        {
            if (@event?.Properties == null)
                return null;

            return @event.Properties.TryGetValue(WellKnownProperties.SourceContext, out var value) ? value as SourceContextValue : null;
        }
    }
}
