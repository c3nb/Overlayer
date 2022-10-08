using JetBrains.Annotations;
using Vostok.Logging.Abstractions.Values;

namespace Vostok.Logging.Abstractions
{
    /// <summary>
    /// Defines names of special well-known properties in <see cref="LogEvent"/>s.
    /// </summary>
    [PublicAPI]
    public static class WellKnownProperties
    {
        /// <summary>
        /// <para>Property that denotes logging events source, such as a class in code base.</para>
        /// <para>Represented by <see cref="SourceContextValue"/>.</para>
        /// </summary>
        public const string SourceContext = "sourceContext";

        /// <summary>
        /// <para>Property that denotes current logical operation context related to code execution flow, such as currently handled query.</para>
        /// <para>Represented by <see cref="OperationContextValue"/>.</para>
        /// </summary>
        public const string OperationContext = "operationContext";

        /// <summary>
        /// <para>Property that denotes current distributed tracing context.</para>
        /// <para>Represented by anything of user's choice.</para>
        /// </summary>
        public const string TraceContext = "traceContext";
    }
}
