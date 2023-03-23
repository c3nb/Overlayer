using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Waiter = System.Threading.Tasks.TaskCompletionSource<bool>;

#pragma warning disable 420

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal class LifoSemaphore
    {
        private readonly ConcurrentStack<Waiter> waiters;
        private readonly WaitCallback signalWaiterCallback;
        private readonly WaitCallback signalDeferredWaiterCallback;
        private readonly WaitCallback signalDeferredWaitersCallback;
        private volatile int currentCount;

        public LifoSemaphore(int initialCount)
        {
            if (initialCount < 0)
                throw new ArgumentOutOfRangeException(nameof(initialCount), $"Initial count must not be negative, but was '{initialCount}'.");

            currentCount = initialCount;

            waiters = new ConcurrentStack<Waiter>();

            signalWaiterCallback = waiter => ((Waiter)waiter).TrySetResult(true);
            signalDeferredWaiterCallback = _ => SignalDeferredWaiter();
            signalDeferredWaitersCallback = count => SignalDeferredWaiters((int)count);
        }

        public int CurrentCount => Math.Max(0, currentCount);

        public int CurrentQueue => Math.Max(0, -currentCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task WaitAsync()
        {
            var decrementedCount = Interlocked.Decrement(ref currentCount);
            if (decrementedCount >= 0)
                return Task.CompletedTask;

            var waiter = new Waiter(TaskCreationOptions.RunContinuationsAsynchronously);

            waiters.Push(waiter);

            return waiter.Task;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Release()
        {
            // (iloktionov): Если мы увеличили отрицательный count, то из этого следует одно из двух: 
            // (iloktionov): 1. В стеке уже есть waiter, которого можно разблокировать. 
            // (iloktionov): 2. В стеке скоро появится waiter, которого можно разблокировать (гонка с WaitAsync). 
            var countBeforeRelease = Interlocked.Increment(ref currentCount) - 1;
            if (countBeforeRelease < 0)
            {
                SignalWaiter();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Release(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), $"Release count must not be negative, but got value '{count}'.");

            if (count == 0)
                return;

            var countBeforeRelease = Interlocked.Add(ref currentCount, count) - count;
            if (countBeforeRelease < 0)
            {
                SignalWaiters(Math.Min(count, -countBeforeRelease));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SignalWaiter()
        {
            if (waiters.TryPop(out var waiter))
            {
                ThreadPool.UnsafeQueueUserWorkItem(signalWaiterCallback, waiter);
            }
            else
            {
                ThreadPool.UnsafeQueueUserWorkItem(signalDeferredWaiterCallback, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SignalWaiters(int count)
        {
            var remainingCount = count;

            for (var i = 0; i < count; i++)
            {
                if (!waiters.TryPop(out var waiter))
                    break;

                ThreadPool.UnsafeQueueUserWorkItem(signalWaiterCallback, waiter);

                remainingCount--;
            }

            if (remainingCount > 0)
            {
                ThreadPool.UnsafeQueueUserWorkItem(signalDeferredWaitersCallback, remainingCount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SignalDeferredWaiter()
        {
            Waiter waiter;

            var spinner = new SpinWait();

            while (!waiters.TryPop(out waiter))
            {
                spinner.SpinOnce();
            }

            waiter.TrySetResult(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SignalDeferredWaiters(int count)
        {
            for (var i = 0; i < count; i++)
            {
                SignalDeferredWaiter();
            }
        }
    }
}