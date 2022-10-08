using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal class AtomicBoolean
    {
        private const int TrueState = 1;
        private const int FalseState = 0;

        private int state;

        public AtomicBoolean(bool initialValue) =>
            state = initialValue ? TrueState : FalseState;

        public bool Value
        {
            get => state == TrueState;
            set => Interlocked.Exchange(ref state, value ? TrueState : FalseState);
        }

        public bool TrySetTrue() =>
            Interlocked.CompareExchange(ref state, TrueState, FalseState) == FalseState;

        public void SetTrue() =>
            Interlocked.Exchange(ref state, TrueState);

        public bool TrySetFalse() =>
            Interlocked.CompareExchange(ref state, FalseState, TrueState) == TrueState;

        public void SetFalse() =>
            Interlocked.Exchange(ref state, FalseState);

        public bool TrySet(bool value) =>
            value ? TrySetTrue() : TrySetFalse();

        public void Set(bool value)
        {
            if (value)
                SetTrue();
            else
                SetFalse();
        }

        public override string ToString() =>
            $"{nameof(Value)}: {Value}";

        public static implicit operator bool([NotNull] AtomicBoolean atomicBoolean) =>
            atomicBoolean.Value;

        public static implicit operator AtomicBoolean(bool initialValue) =>
            new AtomicBoolean(initialValue);
    }
}