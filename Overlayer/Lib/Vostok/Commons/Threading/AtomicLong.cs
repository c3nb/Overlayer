using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal class AtomicLong
    {
        private long value;

        public AtomicLong(long value) =>
            this.value = value;

        public long Value
        {
            get => Interlocked.Read(ref value);
            set => Interlocked.Exchange(ref this.value, value);
        }

        public long Increment() =>
            Interlocked.Increment(ref value);

        public long Decrement() =>
            Interlocked.Decrement(ref value);

        public bool TrySet(long newValue, long expectedValue) =>
            Interlocked.CompareExchange(ref value, newValue, expectedValue) == expectedValue;

        public override string ToString() =>
            $"{nameof(Value)}: {Value}";

        public bool TryIncreaseTo(long newValue)
        {
            while (true)
            {
                var currentValue = Value;
                if (newValue <= currentValue)
                    return false;

                if (TrySet(newValue, currentValue))
                    return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns>new value</returns>
        public long Add(long toAdd) =>
            Interlocked.Add(ref value, toAdd);

        /// <summary></summary>
        /// <returns>The original value</returns>
        /// <param name="newValue">The value that replaces the destination value if the comparison results in equality. </param>
        /// <param name="comparand">The value that is compared to the value</param>
        public long CompareExchange(long newValue, long comparand) =>
            Interlocked.CompareExchange(ref value, newValue, comparand);

        /// <summary>Sets a newValue and returns the original value, as an atomic operation.</summary>
        /// <returns>The original value</returns>
        /// <param name="newValue">new value</param>
        public long Exchange(long newValue) =>
            Interlocked.Exchange(ref value, newValue);

        public static implicit operator long([NotNull] AtomicLong atomicLong) =>
            atomicLong.Value;

        public static implicit operator AtomicLong(long initialValue) =>
            new AtomicLong(initialValue);
    }
}