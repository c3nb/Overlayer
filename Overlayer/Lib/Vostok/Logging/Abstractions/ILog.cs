using JetBrains.Annotations;

namespace Vostok.Logging.Abstractions
{
    /// <summary>
    /// <para>Represents a log.</para>
    /// <para>Implementations are expected to be thread-safe and never throw exceptions in any method.</para>
    /// </summary>
    [PublicAPI]
    public interface ILog
    {
        /// <summary>
        /// Logs the given <see cref="LogEvent"/>. This method should not be called directly in most cases. 
        /// Use one of the <see cref="LogExtensions.Debug(ILog,string)"/>, <see cref="LogExtensions.Info(ILog,string)"/>, <see cref="LogExtensions.Warn(ILog,string)"/>, <see cref="LogExtensions.Error(ILog,string)"/> or <see cref="LogExtensions.Fatal(ILog,string)"/> extension methods instead.
        /// </summary>
        void Log([CanBeNull] LogEvent @event);

        /// <summary>
        /// <para>Returns whether the current log is configured to log events of the given <see cref="LogLevel"/>.</para>
        /// <para>In case you use the <see cref="Log"/> method directly, call this method to avoid unnecessary construction of <see cref="LogEvent"/>s.</para>
        /// </summary>
        bool IsEnabledFor(LogLevel level);

        /// <summary>
        /// <para>Returns a copy of the log operating in the given source <paramref name="context" />.</para>
        /// <para>The nature of this context is described by following statements:</para>
        /// <list type="bullet">
        ///     <item><description>It is meant to denote the source of logging events, such as a class.</description></item>
        ///     <item><description>It is tied to the returned instance of <see cref="ILog"/>.</description></item>
        ///     <item><description>It is hierarchical: chained <see cref="ForContext"/> calls produce an ordered sequence of contexts.</description></item>
        /// </list>
        /// <para>
        ///     It is generally expected that implementations would enrich incoming <see cref="LogEvent"/>s with <see cref="WellKnownProperties.SourceContext"/> property containing this context.
        ///     <see cref="Wrappers.SourceContextWrapper"/> helps with this.
        ///     However, handling of source contexts is implementation-specific and may differ from the approach described above.
        /// </para>
        /// </summary>
        [NotNull]
        ILog ForContext([NotNull] string context);
    }
}
