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
        
    }
}
