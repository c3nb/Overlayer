using System;
using System.Collections.Generic;
using System.Linq;

namespace Overlayer.Utils
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> forEach)
        {
            foreach (T element in enumerable)
                forEach(element);
        }
        public static T[] Push<T>(this T[] array, T item)
        {
            return array.Add(item);
        }
        public static T[] Pop<T>(this T[] array, out T item)
        {
            int length = array.Length;
            if (length == 0)
            {
                item = default(T);
                return array;
            }    
            item = array[length - 1];
            Array.Resize(ref array, length - 1);
            return array;
        }
        public static T[] Copy<T>(this T[] array)
        {
            var len = array.Length;
            T[] arr = new T[len];
            Array.Copy(array, 0, arr, 0, len);
            return arr;
        }
        public static T[] Add<T>(this T[] array, T item)
        {
            int length = array.Length;
            Array.Resize(ref array, length + 1);
            array[length] = item;
            return array;
        }
        public static T[] AddRange<T>(this T[] array, IEnumerable<T> items)
        {
            int count = items is Array arr ? arr.Length : items.Count();
            int length = array.Length;
            Array.Resize(ref array, length + count);
            int index = length;
            foreach (T item in items)
                array[index++] = item;
            return array;
        }
        public static T[] Insert<T>(this T[] array, int index, T item)
        {
            int length = array.Length;
            Array.Resize(ref array, length + 1);
            if (index < length)
                Array.Copy(array, index, array, index + 1, length - index);
            array[index] = item;
            return array;
        }
        public static T[] InsertRange<T>(this T[] array, int index, IEnumerable<T> items)
        {
            int length = array.Length;
            int count = items is Array a ? a.Length : items.Count();
            Array.Resize(ref array, count + length);
            if (index < length)
                Array.Copy(array, index, array, index + count, length - index);
            Array itemsArray = items is Array arr ? arr : items.ToArray();
            itemsArray.CopyTo(array, index);
            return array;
        }
    }
}
