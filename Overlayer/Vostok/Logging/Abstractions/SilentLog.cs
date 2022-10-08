using JetBrains.Annotations;

namespace Vostok.Logging.Abstractions
{
    /// <summary>
    /// Represents a log that just discards all incoming <see cref="LogEvent"/>s.
    /// </summary>
    [PublicAPI]
    public class SilentLog : ILog
    {
        public void Log(LogEvent @event)
        {
        }

        public bool IsEnabledFor(LogLevel level) => false;

        public ILog ForContext(string context) => this;
    }
}
