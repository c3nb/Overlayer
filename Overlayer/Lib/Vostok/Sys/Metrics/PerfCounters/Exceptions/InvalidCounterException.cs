using System;
using System.Runtime.Serialization;

namespace Vostok.Sys.Metrics.PerfCounters.Exceptions
{
    internal class InvalidCounterException : InvalidOperationException
    {
        public InvalidCounterException(string category, string counter)
            : base($"Counter with name '{counter}' doesn't exists in '{category}' category.")
        {
        }

        public InvalidCounterException(string category, string counter, Exception innerException)
            : base($"Counter with name '{counter}' doesn't exists in '{category}' category.", innerException)
        {
        }

        protected InvalidCounterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}