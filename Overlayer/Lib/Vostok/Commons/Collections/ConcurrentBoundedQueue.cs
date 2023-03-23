using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

#pragma warning disable 420

namespace Vostok.Commons.Collections
{
    /// <summary>
    /// <para>Represents a thread-safe "many producers + single consumer" queue suited for scenarios where production is random and frequent, but consumer prefers to work with batches.</para>
    /// <para>It's expected that <see cref="TryAdd"/> is called concurrently from different threads, but <see cref="Drain"/> is not used concurrently from different threads.</para>
    /// <para>The queue has a static predefined capacity. In the event of overflow, new items cannot be added.</para>
    /// </summary>
    [PublicAPI]
    internal class ConcurrentBoundedQueue<T>
        where T : class
    {
        private readonly T[] items;
        private readonly int drainBatchCount;
        private readonly object drainLock;

        private int frontPtr;
        private volatile int itemsCount;
        private volatile int backPtr;
        private volatile DrainSignal canDrainAny;
        private volatile DrainSignal canDrainBatch;

        /// <summary>
        /// Create a new <see cref="ConcurrentBoundedQueue{T}"/> with the given <paramref name="capacity"/>.
        /// </summary>
        public ConcurrentBoundedQueue(int capacity, int drainBatchCount = 1)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "The capacity must be non-negative");
            if (drainBatchCount > capacity)
                throw new ArgumentOutOfRangeException(nameof(drainBatchCount), "The drain count must be less or equal than capacity");
            if (drainBatchCount < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity), "The drain count must be positive");

            items = new T[capacity];
            drainLock = new object();
            canDrainAny = new DrainSignal();
            canDrainBatch = new DrainSignal();
            this.drainBatchCount = drainBatchCount;
        }

        /// <summary>
        /// Returns current count if items in queue.
        /// </summary>
        public int Count => itemsCount;

        /// <summary>
        /// Returns queue capacity (a maximum count of items that can be added).
        /// </summary>
        public int Capacity => items.Length;

        /// <summary>
        /// <para>Attempts to add given <paramref name="item"/> to queue.</para>
        /// <para>This method can be called concurrently from different threads.</para>
        /// <para>This method can be called concurrently with <see cref="Drain"/>.</para>
        /// <para>This method is lock-free.</para>
        /// </summary>
        /// <returns><c>true</c> if item was added, <c>false</c> otherwise (when queue is full).</returns>
        public bool TryAdd(T item)
        {
            while (true)
            {
                var currentCount = itemsCount;
                if (currentCount >= items.Length)
                    return false;

                if (Interlocked.CompareExchange(ref itemsCount, currentCount + 1, currentCount) == currentCount)
                {
                    while (true)
                    {
                        var currentFrontPtr = frontPtr;

                        if (Interlocked.CompareExchange(ref frontPtr, (currentFrontPtr + 1) % items.Length, currentFrontPtr) == currentFrontPtr)
                        {
                            Interlocked.Exchange(ref items[currentFrontPtr], item);

                            canDrainAny.Set();
                            if (currentCount + 1 >= drainBatchCount && itemsCount >= drainBatchCount)
                                canDrainBatch.Set();

                            return true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// <para>Attempts to drain up to <paramref name="count"/> of items from queue to <paramref name="buffer"/>, starting at given <paramref name="index"/>.</para>
        /// <para>The queue is not guaranteed to become empty after a successful drain due to potential races with adder threads.</para>
        /// <para>This method should not be called concurrently with itself from different threads.</para>
        /// <para>This method can be called concurrently with <see cref="TryAdd"/>.</para>
        /// </summary>
        /// <returns>Resulting count of items drained into <paramref name="buffer"/>, starting at <paramref name="index"/>.</returns>
        public int Drain(T[] buffer, int index, int count)
        {
            lock (drainLock)
            {
                var currentCount = itemsCount;
                if (currentCount == 0)
                    return 0;

                var resultCount = 0;

                for (var i = 0; i < Math.Min(count, currentCount); i++)
                {
                    var itemIndex = (backPtr + i) % items.Length;
                    var item = Interlocked.Exchange(ref items[itemIndex], null);
                    if (item == null)
                        break;

                    buffer[index + resultCount++] = item;
                }

                backPtr = (backPtr + resultCount) % items.Length;

                Interlocked.Add(ref itemsCount, -resultCount);

                if (itemsCount == 0)
                {
                    var canDrainStaleBatchSignal = Interlocked.Exchange(ref canDrainBatch, new DrainSignal());
                    var canDrainStaleAnySignal = Interlocked.Exchange(ref canDrainAny, new DrainSignal());
                    canDrainStaleAnySignal.Set();
                    canDrainStaleBatchSignal.Set();

                    if (itemsCount > 0)
                        canDrainAny.Set();
                    if (itemsCount >= drainBatchCount)
                        canDrainBatch.Set();
                } else if (itemsCount < drainBatchCount)
                {
                    Interlocked.Exchange(ref canDrainBatch, new DrainSignal()).Set();

                    if (itemsCount >= drainBatchCount)
                        canDrainBatch.Set();
                }

                return resultCount;
            }
        }

        /// <summary>
        /// Asynchronously waits until something is available to <see cref="Drain"/>.
        /// </summary>
        public Task WaitForNewItemsAsync() => canDrainAny.Task;
        
        internal Task WaitForNewItemsBatchAsync() => canDrainBatch.Task;

        /// <summary>
        /// Asynchronously waits until something is available to <see cref="Drain"/> or the provided <paramref name="timeout"/> expires.
        /// </summary>
        /// <returns><c>true</c> if there is something to drain, <c>false</c> otherwise.</returns>
        public async Task<bool> TryWaitForNewItemsAsync(TimeSpan timeout)
        {
            if (canDrainAny.Task.IsCompleted)
                return true;

            using (var cts = new CancellationTokenSource())
            {
                var waitTimeout = Task.Delay(timeout, cts.Token);

                var result = await Task.WhenAny(canDrainAny.Task, waitTimeout).ConfigureAwait(false);
                if (result == waitTimeout)
                    return false;

                cts.Cancel();
                return true;
            }
        }
        
        /// <summary>
        /// Asynchronously waits until batch is available to <see cref="Drain"/> or the provided <paramref name="timeout"/> expires.
        /// </summary>
        /// <returns><c>true</c> if there is something to drain, <c>false</c> otherwise.</returns>
        public async Task<bool> TryWaitForNewItemsBatchAsync(TimeSpan timeout)
        {
            if (canDrainBatch.Task.IsCompleted)
                return true;

            using (var cts = new CancellationTokenSource())
            {
                var waitTimeout = Task.Delay(timeout, cts.Token);

                var result = await Task.WhenAny(canDrainBatch.Task, waitTimeout).ConfigureAwait(false);
                if (result == waitTimeout)
                    return canDrainAny.Task.IsCompleted;

                cts.Cancel();
                return true;
            }
        }

        private class DrainSignal : TaskCompletionSource<bool>
        {
            private int signalGate;

            public DrainSignal()
                : base(TaskCreationOptions.RunContinuationsAsynchronously)
            {
            }

            public void Set()
            {
                if (!Task.IsCompleted && Interlocked.Exchange(ref signalGate, 1) == 0)
                    TrySetResult(true);
            }
        }
    }
}