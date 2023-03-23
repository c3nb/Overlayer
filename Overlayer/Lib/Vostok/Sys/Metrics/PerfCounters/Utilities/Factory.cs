using System;
using System.Linq.Expressions;

namespace Vostok.Sys.Metrics.PerfCounters.Utilities
{
    internal static class Factory
    {
        public static T Create<T>()
            where T : new() => Cache<T>.Creator();

        private static Func<T> GenerateFactory<T>()
            where T : new()
            => Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();

        private static class Cache<T>
            where T : new()
        {
            public static readonly Func<T> Creator;

            static Cache() => Creator = GenerateFactory<T>();
        }
    }
}