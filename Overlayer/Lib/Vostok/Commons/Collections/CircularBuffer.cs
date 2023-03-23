using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.Commons.Collections
{
    [PublicAPI]
    internal class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] items;
        private int currentIndex;

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            items = new T[capacity];
        }

        public T First
        {
            get
            {
                if (Count == 0)
                    throw new InvalidOperationException("Buffer does not contain any elements.");

                return items[TransformIndex(0)];
            }
        }

        public T Last
        {
            get
            {
                if (Count == 0)
                    throw new InvalidOperationException("Buffer does not contain any elements.");

                return items[TransformIndex(Count - 1)];
            }
        }

        public int Count { get; private set; }

        public int Capacity => items.Length;

        public bool IsFull => Count == Capacity;

        public void Add(T item)
        {
            items[currentIndex] = item;

            currentIndex++;
            currentIndex %= items.Length;

            if (Count < items.Length)
                Count++;
        }

        public void Clear()
        {
            Count = 0;
            currentIndex = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var index = 0; index < Count; index++)
            {
                yield return items[TransformIndex(index)];
            }
        }

        public IEnumerable<T> EnumerateReverse()
        {
            for (var index = Count - 1; index >= 0; index--)
            {
                yield return items[TransformIndex(index)];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int TransformIndex(int index)
        {
            return (currentIndex - Count + index + items.Length) % items.Length;
        }
    }
}