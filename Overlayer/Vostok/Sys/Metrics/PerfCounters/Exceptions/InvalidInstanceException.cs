using System;
using System.Runtime.Serialization;

namespace Vostok.Sys.Metrics.PerfCounters.Exceptions
{
    internal class InvalidInstanceException : InvalidOperationException
    {
        public InvalidInstanceException(string instance)
            : base($"Instance with name '{instance}' doesn't exists.")
        {
        }

        public InvalidInstanceException(string instance, Exception innerException)
            : base($"Instance with name: '{instance}' doesn't exists.", innerException)
        {
        }

        protected InvalidInstanceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}