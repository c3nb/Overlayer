using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;

#pragma warning disable 420

namespace Vostok.Commons.Collections
{
    /// <summary>
    /// <para><see cref="CachingTransform{TRaw,TProcessed}"/> helps to obtain a value derived from external source by applying a transformation.</para>
    /// <para>It assumes that transformation is expensive to perform and caches transformed result for last observed source value for efficiency.</para>
    /// </summary>
    /// <typeparam name="TRaw">Type of raw source value.</typeparam>
    /// <typeparam name="TProcessed">Type of processed result.</typeparam>
    [PublicAPI]
    internal class CachingTransform<TRaw, TProcessed>
    {
        private readonly Func<TRaw> provider;
        private readonly Func<TRaw, TProcessed> processor;
        private readonly IEqualityComparer<TRaw> comparer;
        private readonly object syncObject;

        private volatile Tuple<TRaw, TProcessed> cache;

        /// <param name="processor">Transformation function applied to raw values.</param>
        /// <param name="provider">Provider function used for <see cref="Get()"/> without parameters.</param>
        /// <param name="comparer">Comparer used to check cache validity.</param>
        /// <param name="preventParallelProcessing">If set to <c>true</c>, prevents parallel execution of <paramref name="processor"/> function.</param>
        public CachingTransform(
            Func<TRaw, TProcessed> processor,
            Func<TRaw> provider = null,
            IEqualityComparer<TRaw> comparer = null,
            bool preventParallelProcessing = true)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            this.provider = provider;
            this.comparer = comparer ?? SelectDefaultComparer();

            syncObject = preventParallelProcessing ? new object() : null;
        }

        /// <summary>
        /// <para>Returns processed value that corresponds to current raw value from provided delegate defined in constructor.</para>
        /// <para>Does not handle any exceptions produced by user delegates.</para>
        /// </summary>
        public TProcessed Get()
        {
            if (provider == null)
                throw new InvalidOperationException("Raw value provider delegate is not defined.");

            return Get(provider());
        }

        /// <summary>
        /// <para>Returns processed value that corresponds to given <paramref name="raw"/> value.</para>
        /// <para>Does not handle any exceptions produced by user delegates.</para>
        /// </summary>
        public TProcessed Get(TRaw raw)
        {
            var currentCache = cache;

            if (IsValidCache(currentCache, raw))
                return currentCache.Item2;

            // (iloktionov): Null syncObject means that we can execute processor delegate from multiple threads without locks:
            if (syncObject == null)
            {
                var processed = processor(raw);

                Interlocked.CompareExchange(ref cache, Tuple.Create(raw, processed), currentCache);

                return processed;
            }

            // (iloktionov): Otherwise we fall back to double-checked locking:
            lock (syncObject)
            {
                if (IsValidCache(cache, raw))
                    return cache.Item2;

                var processed = processor(raw);

                Interlocked.Exchange(ref cache, Tuple.Create(raw, processed));

                return processed;
            }
        }

        private static IEqualityComparer<TRaw> SelectDefaultComparer()
        {
            if (typeof(TRaw).IsValueType)
                return EqualityComparer<TRaw>.Default;

            return ByReferenceEqualityComparer<TRaw>.Instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidCache(Tuple<TRaw, TProcessed> currentCache, TRaw actualRaw)
        {
            return currentCache != null && comparer.Equals(currentCache.Item1, actualRaw);
        }
    }
}