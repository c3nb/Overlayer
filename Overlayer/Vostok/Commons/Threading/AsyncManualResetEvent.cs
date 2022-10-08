using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

#pragma warning disable 420

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource<bool> state = CreateTaskCompletionSource();

        public AsyncManualResetEvent(bool isSetInitially)
        {
            if (isSetInitially)
                Set();
        }

        public void Set() => state.TrySetResult(true);

        public bool IsCurrentlySet() => state.Task.IsCompleted;

        public void Reset()
        {
            while (true)
            {
                var currentState = state;
                if (!currentState.Task.IsCompleted)
                    return;
                if (Interlocked.CompareExchange(ref state, CreateTaskCompletionSource(), currentState) == currentState)
                    return;
            }
        }

        public Task WaitAsync() => state.Task;

        public Task WaitAsync(CancellationToken token) =>
            token.CanBeCanceled
                ? WaitAsyncWithCancellation(token)
                // ReSharper disable once MethodSupportsCancellation
                : WaitAsync();

        /// <summary>
        /// Returns <c>true</c> if event set, false if timeout reached.
        /// </summary>
        public Task<bool> WaitAsync(CancellationToken token, TimeSpan timeout)
            => WaitAsyncWithCancellationAndTimeout(token, timeout);

        public TaskAwaiter GetAwaiter() => WaitAsync().GetAwaiter();

        public static implicit operator Task(AsyncManualResetEvent @event) => @event.WaitAsync();

        private static TaskCompletionSource<bool> CreateTaskCompletionSource() =>
            new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        private async Task WaitAsyncWithCancellation(CancellationToken token)
        {
            var tcs = CreateTaskCompletionSource();

            using (token.Register(o => ((TaskCompletionSource<bool>)o).TrySetCanceled(), tcs))
            {
                await Task.WhenAny(state.Task, tcs.Task)
                    .ConfigureAwait(false);

                token.ThrowIfCancellationRequested();
            }
        }

        private async Task<bool> WaitAsyncWithCancellationAndTimeout(CancellationToken token, TimeSpan timeout)
        {
            using (var timeoutCancellation = new CancellationTokenSource())
            using (var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutCancellation.Token))
            {
                var timeoutTask = Task.Delay(timeout, linkedCancellation.Token);

                await Task.WhenAny(state.Task, timeoutTask)
                    .ConfigureAwait(false);

                timeoutCancellation.Cancel();

                token.ThrowIfCancellationRequested();

                return state.Task.IsCompleted;
            }
        }
    }
}