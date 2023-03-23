using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal class AsyncLock
    {
        private readonly SemaphoreSlim semaphore;
        private readonly Task<IDisposable> cachedReleaserTask;

        public AsyncLock()
        {
            semaphore = new SemaphoreSlim(1, 1);
            cachedReleaserTask = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var waitTask = semaphore.WaitAsync();
            if (waitTask.IsCompleted)
                return cachedReleaserTask;

            return waitTask.ContinueWith((_, state) => (IDisposable)new Releaser((AsyncLock)state), this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public bool TryLockImmediately(out IDisposable releaser)
        {
            if (semaphore.CurrentCount == 1 && semaphore.WaitAsync(0).GetAwaiter().GetResult())
            {
                releaser = cachedReleaserTask.Result;
                return true;
            }

            releaser = null;
            return false;
        }

        private class Releaser : IDisposable
        {
            private readonly AsyncLock lockToRelease;

            public Releaser(AsyncLock lockToRelease)
            {
                this.lockToRelease = lockToRelease;
            }

            public void Dispose()
            {
                lockToRelease.semaphore.Release();
            }
        }
    }
}