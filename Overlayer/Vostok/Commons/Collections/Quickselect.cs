using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.Commons.Collections
{
    [PublicAPI]
    internal static class Quickselect
    {
        [ThreadStatic]
        private static Random random;

        public static T QuickSelect<T>(this T[] list, int top, IComparer<T> comparer = null, QuickselectSortOrder order = QuickselectSortOrder.Ascending)
        {
            if (top > list.Length || list.Length == 0)
                throw new InvalidOperationException($"Requested to select top {top} items from array with size {list.Length}.");

            if (comparer == null)
                comparer = Comparer<T>.Default;

            if (top == list.Length)
            {
                list.Swap(list.IndexOfMax(comparer, order), top - 1);
                return list[top - 1];
            }

            if (top <= 0)
            {
                list.Swap(list.IndexOfMin(comparer, order), 0);
                return list[0];
            }

            var startIndex = 0;
            var endIndex = list.Length - 1;
            var pivotIndex = top;

            while (endIndex > startIndex)
            {
                pivotIndex = list.Partition(startIndex, endIndex, pivotIndex, comparer, order);

                if (pivotIndex == top)
                    break;

                if (pivotIndex > top)
                    endIndex = pivotIndex - 1;
                else
                    startIndex = pivotIndex + 1;

                pivotIndex = Next(startIndex, endIndex);
            }

            return list[pivotIndex];
        }

        private static int Partition<T>(this T[] list, int startIndex, int endIndex, int pivotIndex, IComparer<T> comparer, QuickselectSortOrder order)
        {
            Swap(list, pivotIndex, startIndex);

            var pivot = list[startIndex];
            var i = startIndex;
            var j = endIndex + 1;

            while (true)
            {
                do
                {
                    i++;
                } while (i <= endIndex && comparer.CompareWithOrder(list[i], pivot, order) < 0);

                do
                {
                    j--;
                } while (comparer.CompareWithOrder(list[j], pivot, order) > 0);

                if (i >= j)
                {
                    Swap(list, startIndex, j);
                    return j;
                }

                Swap(list, i, j);
            }
        }

        private static void Swap<T>(this T[] list, int index1, int index2)
        {
            // ReSharper disable once SwapViaDeconstruction
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        private static int IndexOfMin<T>(this T[] list, IComparer<T> comparer, QuickselectSortOrder order)
        {
            var minIndex = 0;
            for (var i = 0; i < list.Length; i++)
                if (comparer.CompareWithOrder(list[i], list[minIndex], order) < 0)
                    minIndex = i;
            return minIndex;
        }

        private static int IndexOfMax<T>(this T[] list, IComparer<T> comparer, QuickselectSortOrder order)
            => list.IndexOfMin(comparer, (QuickselectSortOrder)((int)order * -1));

        private static int Next(int minValue, int maxValue)
            => ObtainRandom().Next(minValue, maxValue);

        private static int CompareWithOrder<T>(this IComparer<T> comparer, T first, T second, QuickselectSortOrder order)
            => comparer.Compare(first, second) * (int)order;

        private static Random ObtainRandom()
            => random ?? (random = new Random(Guid.NewGuid().GetHashCode()));
    }
}