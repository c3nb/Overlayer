using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal class AtomicInt
    {
        private int value;

        public AtomicInt(int value) =>
            this.value = value;

        public int Value
        {
            get => value;
            set => Interlocked.Exchange(ref this.value, value);
        }

        public int Increment() =>
            Interlocked.Increment(ref value);

        public int Decrement() =>
            Interlocked.Decrement(ref value);

        public bool TrySet(int newValue, int expectedValue) =>
            Interlocked.CompareExchange(ref value, newValue, expectedValue) == expectedValue;

        public bool TryIncreaseTo(int newValue)
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
        public int Add(int toAdd) =>
            Interlocked.Add(ref value, toAdd);

        public override string ToString() =>
            $"{nameof(Value)}: {Value}";

        /// <summary></summary>
        /// <returns>The original value</returns>
        /// <param name="newValue">The value that replaces the destination value if the comparison results in equality. </param>
        /// <param name="comparand">The value that is compared to the value</param>
        public int CompareExchange(int newValue, int comparand) =>
            Interlocked.CompareExchange(ref value, newValue, comparand);

        /// <summary>Sets a newValue and returns the original value, as an atomic operation.</summary>
        /// <returns>The original value</returns>
        /// <param name="newValue">new value</param>
        public int Exchange(int newValue) =>
            Interlocked.Exchange(ref value, newValue);

        public static implicit operator int([NotNull] AtomicInt atomicInt) =>
            atomicInt.Value;
    }
}