using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Vostok.Commons.Collections;
using Vostok.Logging.Abstractions.Helpers;

namespace Vostok.Logging.Abstractions.Values
{
    /// <summary>
    /// Represents the value of <see cref="WellKnownProperties.OperationContext"/> property.
    /// </summary>
    [PublicAPI]
    public class OperationContextValue : HierarchicalContextValue
    {
        private volatile string stringRepresentation;

        public OperationContextValue([NotNull] string[] contexts)
            : this(contexts, null)
        {
        }

        public OperationContextValue([NotNull] string[] contexts, [CanBeNull] IReadOnlyDictionary<string, object> properties)
            : base(contexts)
        {
            Properties = properties;
        }

        public OperationContextValue(string context)
            : this(context, null)
        {
        }

        public OperationContextValue(string context, [CanBeNull] IReadOnlyDictionary<string, object> properties)
            : base(context)
        {
            Properties = properties;
        }

        [CanBeNull]
        public IReadOnlyDictionary<string, object> Properties { get; }

        public override string ToString() =>
            stringRepresentation ?? (stringRepresentation = ToStringInternal());

        public static IReadOnlyDictionary<string, object> CreateProperties([CanBeNull] string template, [CanBeNull] object properties) =>
            DeconstructionHelper.ShouldDeconstruct(template, properties)
                ? LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true)
                : CreateProperties(template, new[] {properties});

        public static IReadOnlyDictionary<string, object> CreateProperties([CanBeNull] string template, [CanBeNull] params object[] parameters) =>
            LogEventExtensions.GenerateInitialParameters(template, parameters);

        public static OperationContextValue operator+([CanBeNull] OperationContextValue left, [CanBeNull] string right) =>
            left + (right, null);

        public static OperationContextValue operator+([CanBeNull] OperationContextValue left, (string OperationContext, IReadOnlyDictionary<string, object> Properties) right)
        {
            if (string.IsNullOrEmpty(right.OperationContext) && right.Properties == null)
                return left;

            if (left == null)
                return new OperationContextValue(right.OperationContext, right.Properties);

            var newContexts = right.OperationContext == null ? left.contexts : AppendToContexts(left.contexts, right.OperationContext);
            var newProperties = CombineProperties(left.Properties, right.Properties);

            return ReferenceEquals(left.contexts, newContexts) && ReferenceEquals(left.Properties, newProperties)
                ? left
                : new OperationContextValue(newContexts, newProperties);
        }

        private static IReadOnlyDictionary<string, object> CombineProperties(IReadOnlyDictionary<string, object> left, IReadOnlyDictionary<string, object> right)
        {
            if (left == null || right == null)
                return left ?? right;

            var result = left as ImmutableArrayDictionary<string, object> ?? new ImmutableArrayDictionary<string, object>(left, StringComparer.Ordinal);

            foreach (var pair in right)
                result = result.Set(pair.Key, pair.Value);

            return result;
        }

        private string ToStringInternal()
        {
            var builder = new StringBuilder(contexts.Sum(c => c.Length) + contexts.Length * 3);

            for (var index = 0; index < contexts.Length; index++)
            {
                builder
                    .Append('[')
                    .Append(contexts[index])
                    .Append(']');

                if (index < contexts.Length - 1)
                    builder.Append(' ');
            }

            return builder.ToString();
        }

        #region Equality

        public bool Equals(OperationContextValue other)
            => ReferenceEquals(this, other) || other != null && contexts.SequenceEqual(other.contexts);

        public override bool Equals(object other)
            => Equals(other as OperationContextValue);

        public override int GetHashCode()
            => contexts.Aggregate(contexts.Length, (current, value) => (current * 397) ^ value.GetHashCode());

        #endregion
    }
}