using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.Logging.Abstractions.Values
{
    [PublicAPI]
    public abstract class HierarchicalContextValue : IReadOnlyList<string>
    {
        protected readonly string[] contexts;

        protected HierarchicalContextValue([NotNull] string[] contexts)
        {
            this.contexts = contexts ?? throw new ArgumentNullException(nameof(contexts));

            if (contexts.Length == 0)
                throw new ArgumentException("Provided contexts array is empty.");
        }

        protected HierarchicalContextValue(string context)
            : this(new[] {context})
        {
        }

        public int Count => contexts.Length;

        public IEnumerator<string> GetEnumerator()
            => (contexts as IList<string>).GetEnumerator();

        public string this[int index] => contexts[index];

        protected static string[] AppendToContexts([NotNull] string[] contexts, [NotNull] string value)
        {
            var newContexts = new string[contexts.Length + 1];

            Array.Copy(contexts, newContexts, contexts.Length);

            newContexts[contexts.Length] = value;

            return newContexts;
        }

        protected static string[] MergeContexts([NotNull] string[] contexts, [NotNull] string[] values)
        {
            var newContexts = new string[contexts.Length + values.Length];

            Array.Copy(contexts, newContexts, contexts.Length);

            Array.Copy(values, 0, newContexts, contexts.Length, values.Length);

            return newContexts;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
