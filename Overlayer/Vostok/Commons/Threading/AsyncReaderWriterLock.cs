using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal class AsyncReaderWriterLock
    {
        private readonly Task<IDisposable> readerReleaserTask;
        private readonly Task<IDisposable> writerReleaserTask;
        private readonly Queue<TaskCompletionSource<IDisposable>> waitingWriters;
        private TaskCompletionSource<IDisposable> waitingReader;
        private int readersWaiting;

        // (iloktionov): Meaning of status values:
        // 0 - no one holds the lock
        // -1 - a writer holds the lock
        // N > 0 - N readers hold the lock
        private int status;

        public AsyncReaderWriterLock()
        {
            readerReleaserTask = Task.FromResult((IDisposable)new Releaser(this, false));
            writerReleaserTask = Task.FromResult((IDisposable)new Releaser(this, true));
            waitingWriters = new Queue<TaskCompletionSource<IDisposable>>();
            waitingReader = new TaskCompletionSource<IDisposable>(TaskCreationOptions.RunContinuationsAsynchronously);
            status = 0;
        }

        public Task<IDisposable> ReadLockAsync()
        {
            lock (waitingWriters)
            {
                if (status >= 0 && waitingWriters.Count == 0)
                {
                    status++;
                    return readerReleaserTask;
                }

                readersWaiting++;

                return waitingReader.Task.ContinueWith(t => t.Result);
            }
        }

        public Task<IDisposable> WriteLockAsync()
        {
            lock (waitingWriters)
            {
                if (status == 0)
                {
                    status = -1;
                    return writerReleaserTask;
                }

                var waiter = new TaskCompletionSource<IDisposable>(TaskCreationOptions.RunContinuationsAsynchronously);

                waitingWriters.Enqueue(waiter);

                return waiter.Task;
            }
        }

        public bool TryObtainReadLockImmediately(out IDisposable releaser)
        {
            lock (waitingWriters)
            {
                if (status >= 0 && waitingWriters.Count == 0)
                {
                    status++;
                    releaser = readerReleaserTask.Result;
                    return true;
                }

                releaser = null;
                return false;
            }
        }

        public bool TryObtainWriteLockImmediately(out IDisposable releaser)
        {
            lock (waitingWriters)
            {
                if (status == 0)
                {
                    status = -1;
                    releaser = writerReleaserTask.Result;
                    return true;
                }

                releaser = null;
                return false;
            }
        }

        private void ReadRelease()
        {
            TaskCompletionSource<IDisposable> toWake = null;

            lock (waitingWriters)
            {
                status--;
                if (status == 0 && waitingWriters.Count > 0)
                {
                    status = -1;
                    toWake = waitingWriters.Dequeue();
                }
            }

            if (toWake != null)
                ThreadPool.UnsafeQueueUserWorkItem(r => toWake.TrySetResult((Releaser)r), new Releaser(this, true));
        }

        private void WriteRelease()
        {
            TaskCompletionSource<IDisposable> toWake = null;
            var toWakeIsWriter = false;

            lock (waitingWriters)
            {
                if (waitingWriters.Count > 0)
                {
                    toWake = waitingWriters.Dequeue();
                    toWakeIsWriter = true;
                }
                else if (readersWaiting > 0)
                {
                    toWake = waitingReader;
                    status = readersWaiting;
                    readersWaiting = 0;
                    waitingReader = new TaskCompletionSource<IDisposable>(TaskCreationOptions.RunContinuationsAsynchronously);
                }
                else status = 0;
            }

            if (toWake != null)
                ThreadPool.UnsafeQueueUserWorkItem(r => toWake.TrySetResult((Releaser)r), new Releaser(this, toWakeIsWriter));
        }

        private class Releaser : IDisposable
        {
            private readonly AsyncReaderWriterLock lockToRelease;
            private readonly bool isWriter;

            public Releaser(AsyncReaderWriterLock lockToRelease, bool isWriter)
            {
                this.lockToRelease = lockToRelease;
                this.isWriter = isWriter;
            }

            public void Dispose()
            {
                if (isWriter)
                {
                    lockToRelease.WriteRelease();
                }
                else
                {
                    lockToRelease.ReadRelease();
                }
            }
        }
    }
}