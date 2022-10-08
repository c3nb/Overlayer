using JetBrains.Annotations;

namespace Vostok.Metrics
{
    internal static class IMetricContextExtensions_Unwrap
    {
        [NotNull]
        public static IMetricContext Unwrap([NotNull] this IMetricContext context)
        {
            while (context is IMetricContextWrapper wrapper)
                context = wrapper.BaseContext;

            return context;
        }
    }
}