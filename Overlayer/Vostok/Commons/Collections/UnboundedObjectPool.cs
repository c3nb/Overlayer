using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Collections
{
    [PublicAPI]
    internal class UnboundedObjectPool<T>
    {
        private readonly ConcurrentQueue<T> items = new ConcurrentQueue<T>();
        private readonly Func<T> itemFactory;

        public UnboundedObjectPool([NotNull] Func<T> itemFactory)
        {
            this.itemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
        }

        public T Acquire() =>
            items.TryDequeue(out var item) ? item : itemFactory();

        public IDisposable Acquire(out T item) =>
            new Releaser(item = Acquire(), this);

        public void Return(T item) =>
            items.Enqueue(item);

        private class Releaser : IDisposable
        {
            private readonly T item;
            private volatile UnboundedObjectPool<T> pool;

            public Releaser(T item, UnboundedObjectPool<T> pool)
            {
                this.item = item;
                this.pool = pool;
            }

            public void Dispose()
            {
                Interlocked.Exchange(ref pool, null)?.Return(item);
            }
        }
    }
}