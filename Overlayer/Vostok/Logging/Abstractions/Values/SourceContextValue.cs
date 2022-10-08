using System;
using System.Linq;
using JetBrains.Annotations;

namespace Vostok.Logging.Abstractions.Values
{
    /// <summary>
    /// Represents the value of <see cref="WellKnownProperties.SourceContext"/> property.
    /// </summary>
    [PublicAPI]
    public class SourceContextValue : HierarchicalContextValue, IEquatable<SourceContextValue>
    {
        private volatile string stringRepresentation;

        public SourceContextValue([NotNull] string[] contexts)
            : base(contexts)
        {
        }

        public SourceContextValue(string context)
            : base(context)
        {
        }

        public override string ToString() =>
            stringRepresentation ?? (stringRepresentation = "[" + string.Join(" => ", contexts) + "]");

        public static SourceContextValue operator+([CanBeNull] SourceContextValue left, [CanBeNull] string right)
        {
            if (string.IsNullOrEmpty(right))
                return left;

            if (left == null)
                return new SourceContextValue(right);

            var newContexts = AppendToContexts(left.contexts, right);

            return ReferenceEquals(left.contexts, newContexts) ? left : new SourceContextValue(newContexts);
        }

        public static SourceContextValue operator+([CanBeNull] SourceContextValue left, [CanBeNull] SourceContextValue right)
        {
            if (right == null)
                return left;

            if (left == null)
                return right;

            return new SourceContextValue(MergeContexts(left.contexts, right.contexts));
        }

        #region Equality

        public bool Equals(SourceContextValue other) 
            => ReferenceEquals(this, other) || other != null && contexts.SequenceEqual(other.contexts);

        public override bool Equals(object other)
            => Equals(other as SourceContextValue);

        public override int GetHashCode()
            => contexts.Aggregate(contexts.Length, (current, value) => current * 397 ^ value.GetHashCode());

        #endregion
    }
}
