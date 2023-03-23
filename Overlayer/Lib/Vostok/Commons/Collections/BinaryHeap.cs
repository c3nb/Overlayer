using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.Commons.Collections
{
    [PublicAPI]
    internal class BinaryHeap<T>
    {
        private readonly IComparer<T> comparer;
        private readonly Func<bool, bool> comparisonPredicate;
        private readonly List<T> items;

        public BinaryHeap(IComparer<T> comparer = null, bool isMaxHeap = false, int capacity = 4)
        {
            this.comparer = comparer ?? Comparer<T>.Default;

            items = new List<T>(capacity);
            comparisonPredicate = isMaxHeap ? r => !r : (Func<bool, bool>)(r => r);
        }

        public int Count => items.Count;

        public void Add(T item)
        {
            var position = items.Count;

            items.Add(item);

            while (true)
            {
                if (position <= 0)
                    break;

                var nextPosition = (position - 1) / 2;
                var nextItem = items[nextPosition];

                if (!comparisonPredicate(comparer.Compare(nextItem, item) > 0))
                    break;

                items[position] = nextItem;
                position = nextPosition;
            }

            items[position] = item;
        }

        public T Peek()
        {
            EnsureNotEmpty();

            return items[0];
        }

        public T RemoveRoot()
        {
            EnsureNotEmpty();

            var result = items[0];
            var temp = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);

            if (items.Count > 0)
            {
                var i = 0;
                while (i < items.Count / 2)
                {
                    var j = 2 * i + 1;
                    if (j < items.Count - 1 && comparisonPredicate(comparer.Compare(items[j], items[j + 1]) > 0))
                    {
                        ++j;
                    }

                    if (comparisonPredicate(comparer.Compare(items[j], temp) >= 0))
                    {
                        break;
                    }

                    items[i] = items[j];
                    i = j;
                }

                items[i] = temp;
            }

            return result;
        }

        private void EnsureNotEmpty()
        {
            if (items.Count == 0)
                throw new InvalidOperationException("The heap is empty.");
        }
    }
}