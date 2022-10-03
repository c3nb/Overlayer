using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.Utils
{
    public static class ArrayUtils
    {
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
        public static T[] MoveFirst<T>(this T[] array, T item)
        {
            Array.Copy(array, 0, array, 1, array.Length - 1);
            array[0] = item;
            return array;
        }
        public static T[] MoveLast<T>(this T[] array, T item)
        {
            int length = array.Length;
            Array.Copy(array, 1, array, 0, length - 1);
            array[length - 1] = item;
            return array;
        }

        public static void Push<T>(ref T[] array, T item)
            => array = array.Push(item);
        public static void Pop<T>(ref T[] array, out T item)
            => array = array.Pop(out item);
        public static void Copy<T>(this T[] array, out T[] result)
        {
            var len = array.Length;
            result = new T[len];
            Array.Copy(array, 0, result, 0, len);
        }
        public static void Add<T>(ref T[] array, T item)
            => array = array.Add(item);
        public static void AddRange<T>(ref T[] array, IEnumerable<T> items)
            => array = array.AddRange(items);
        public static void Insert<T>(ref T[] array, int index, T item)
            => array = array.Insert(index, item);
        public static void InsertRange<T>(ref T[] array, int index, IEnumerable<T> items)
            => array = array.InsertRange(index, items);
        public static void MoveFirst<T>(ref T[] array, T item)
            => array = array.MoveFirst(item);
        public static void MoveLast<T>(ref T[] array, T item)
            => array = array.MoveLast(item);
        public static T[] Empty<T>() => new T[0];
    }
}
