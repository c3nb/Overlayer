using System;
using System.Runtime.Serialization;

namespace Vostok.Sys.Metrics.PerfCounters.Exceptions
{
    internal class InvalidCategoryException : InvalidOperationException
    {
        public InvalidCategoryException(string category)
            : base($"Category with name '{category}' doesn't exists.")
        {
        }

        public InvalidCategoryException(string category, Exception innerException)
            : base($"Category with name '{category}' doesn't exists.", innerException)
        {
        }

        protected InvalidCategoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}