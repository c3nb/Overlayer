using JetBrains.Annotations;

namespace Vostok.Metrics
{
    [PublicAPI]
    public interface IMetricContextWrapper
    {
        [NotNull]
        IMetricContext BaseContext { get; }
    }
}