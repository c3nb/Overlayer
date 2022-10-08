using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal struct ThreadPoolState
    {
        public ThreadPoolState(int minWorkerThreads, int usedWorkerThreads, int minIocpThreads, int usedIocpThreads)
            : this()
        {
            MinWorkerThreads = minWorkerThreads;
            UsedWorkerThreads = usedWorkerThreads;
            MinIocpThreads = minIocpThreads;
            UsedIocpThreads = usedIocpThreads;
        }

        public int MinWorkerThreads { get; }
        public int UsedWorkerThreads { get; }
        public int MinIocpThreads { get; }
        public int UsedIocpThreads { get; }

        #region Equality

        public override bool Equals(object obj) =>
            !ReferenceEquals(null, obj) &&
            obj is ThreadPoolState state && Equals(state);

        public bool Equals(ThreadPoolState other) =>
            MinWorkerThreads == other.MinWorkerThreads &&
            UsedWorkerThreads == other.UsedWorkerThreads &&
            MinIocpThreads == other.MinIocpThreads &&
            UsedIocpThreads == other.UsedIocpThreads;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MinWorkerThreads;
                hashCode = (hashCode * 397) ^ UsedWorkerThreads;
                hashCode = (hashCode * 397) ^ MinIocpThreads;
                hashCode = (hashCode * 397) ^ UsedIocpThreads;
                return hashCode;
            }
        }

        #endregion
    }
}