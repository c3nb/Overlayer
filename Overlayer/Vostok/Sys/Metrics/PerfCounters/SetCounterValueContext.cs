namespace Vostok.Sys.Metrics.PerfCounters
{
    /// <summary>
    /// <para>A class that holds resulting counter value.</para>
    /// <para>A instances of this class are reused and MUST NOT BE exposed beyond counter callback scope.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SetCounterValueContext<T>
    {
        /// <summary>
        /// <para>The resulting counter value.</para>
        /// <para>An external code should write observed counter value to the appropriate fields here.</para>
        /// </summary>
        public T Result;

        internal SetCounterValueContext()
        {
        }

        /// <summary>
        /// The Category name of current underlying performance counter.
        /// </summary>
        internal string Category { get; set; }

        /// <summary>
        /// The Counter name of current underlying performance counter.
        /// </summary>
        internal string Counter { get; set; }
    }
}