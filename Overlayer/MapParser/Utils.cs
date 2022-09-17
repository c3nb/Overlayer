using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.MapParser
{
    public static class Utils
    {
        public static TValue GetValueSafe<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            if (dict.TryGetValue(key, out TValue value))
                return value;
            return defaultValue;
        }
        public static TValue GetValueSafeOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            if (dict.TryGetValue(key, out TValue value))
                return value;
            return dict[key] = defaultValue;
        }
        public static T Parse<T>(this string value) where T : Enum
        {
            try { return (T)Enum.Parse(typeof(T), value); }
            catch { return default; }
        }
        public static List<T> CastList<T>(this IList list)
        {
            List<T> castedList = new List<T>();
            foreach (object obj in list)
                castedList.Add((T)obj);
            return castedList;
        }
        public static bool IsFinite(this double d)
            => !IsNaN(d) && !IsInfinity(d);
        public unsafe static bool IsNaN(this double d)
            => (*(long*)&d & 0x7FFFFFFFFFFFFFFFL) > 0x7FF0000000000000L;
        public unsafe static bool IsInfinity(this double d)
            => (*(long*)&d & 0x7FFFFFFFFFFFFFFF) == 0x7FF0000000000000;
        public static List<T> SubList<T>(this List<T> list, int from, int to) => list.GetRange(from, to - from);
        public static double ToDeg(this double rad) => rad * (180.0 / Math.PI);
        public static double ToRad(this double deg) => Math.PI * deg / 180.0;
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> forEach)
        {
            foreach (T t in enumerable)
                forEach(t);
            return enumerable;
        }
        public static List<List<T>> Seperate<T>(this List<T> list, int count)
        {
            var result = new List<List<T>>();
            var origCount = list.Count;
            for (int i = 0; i < origCount; i += count)
                result.Add(list.RemoveAndGet(0, Math.Min(list.Count, count)));
            return result;
        }
        public static List<T> RemoveAndGet<T>(this List<T> list, int index, int count)
        {
            var toReturn = list.GetRange(index, count);
            list.RemoveRange(index, count);
            return toReturn;

        }    
    }
}
