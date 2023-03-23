using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Extensions
{
    [PublicAPI]
    internal static class IEnumerableExtensions
    {
        public static async Task<IEnumerable<TOut>> LoopAsync<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, Task<TOut>> selector) =>
            await Task.WhenAll(enumerable.Select(selector)).ConfigureAwait(false);
    }
}