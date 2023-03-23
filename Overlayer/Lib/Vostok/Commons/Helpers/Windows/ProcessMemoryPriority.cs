using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Windows
{
    [PublicAPI]
    internal enum ProcessMemoryPriority : uint
    {
        VeryLow = 1,
        Low = 2,
        Medium = 3,
        BelowNormal = 4,
        Normal = 5
    }
}