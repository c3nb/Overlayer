using System;
using Vostok.Sys.Metrics.PerfCounters.InstanceNames;

namespace Vostok.Sys.Metrics.PerfCounters
{
    public static class PerformanceCounterFactoryExtensions
    {
        public static IPerformanceCounter<double> CreateCounter(
            this IPerformanceCounterFactory factory,
            string category,
            string counter,
            string instance = null)
            => factory
                .Create<double>()
                .AddCounter(category, counter, (context, value) => context.Result = value)
                .Build(instance);

        public static IPerformanceCounter<double> CreateCounter(
            this IPerformanceCounterFactory factory,
            string category,
            string counter,
            Func<string> instanceNameProvider)
            => factory
                .Create<double>()
                .AddCounter(category, counter, (context, value) => context.Result = value)
                .Build(instanceNameProvider);
        
        public static IPerformanceCounter<double> CreateProcessCategoryCounter(
            this IPerformanceCounterFactory factory,
            string counter,
            int processId)
            => factory
                .Create<double>()
                .AddCounter("Process", counter, (context, value) => context.Result = value)
                .Build(InstanceNameProviders.Process.ForPid(processId));
        
        public static IPerformanceCounter<double> CreateDotNetCategoriesCounter(
            this IPerformanceCounterFactory factory,
            string category,
            string counter,
            int processId)
            => factory
                .Create<double>()
                .AddCounter(category, counter, (context, value) => context.Result = value)
                .Build(InstanceNameProviders.DotNet.ForPid(processId));
    }
}