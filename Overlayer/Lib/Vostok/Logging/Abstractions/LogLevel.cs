using JetBrains.Annotations;

namespace Vostok.Logging.Abstractions
{
    /// <summary>
    /// <list type="bullet">
    ///     <listheader>Recommended usage:</listheader>
    ///     <item><description><see cref="Debug"/> — for verbose output; this log level should usually be ignored on production installations.</description></item>
    ///     <item><description><see cref="Info"/> — for neutral messages.</description></item>
    ///     <item><description><see cref="Warn"/> — for non-critical errors that don't interrupt the normal operation of the application.</description></item>
    ///     <item><description><see cref="Error"/> — for unexpected errors that may require human attention.</description></item>
    ///     <item><description><see cref="Fatal"/> — for critical errors that result in application shutdown.</description></item>
    /// </list>
    /// </summary>
    [PublicAPI]
    public enum LogLevel
    {
        /// <summary>
        /// Used for verbose output. This log level should usually be ignored on production installations.
        /// </summary>
        Debug,

        /// <summary>
        /// Used for neutral messages.
        /// </summary>
        Info,

        /// <summary>
        /// Used for non-critical errors that don't interrupt the normal operation of the application.
        /// </summary>
        Warn,

        /// <summary>
        /// Used for unexpected errors that may require human attention.
        /// </summary>
        Error,

        /// <summary>
        /// Used for critical errors that result in application shutdown.
        /// </summary>
        Fatal
    }
}
