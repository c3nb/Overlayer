using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Commons.Time
{
    [PublicAPI]
    internal class AsyncPeriodicalAction
    {
        private readonly Func<CancellationToken, TimeSpan, Task> action;
        private readonly Action<Exception> errorHandler;
        private readonly Func<TimeSpan> period;
        private readonly bool delayFirstIteration;
        private readonly object startStopSync = new object();

        private volatile Task workerTask;
        private volatile CancellationTokenSource cancellationSource;

        public AsyncPeriodicalAction(
            [NotNull] Func<Task> action,
            [NotNull] Action<Exception> errorHandler,
            [NotNull] Func<TimeSpan> period,
            bool delayFirstIteration = false)
            : this((ct, time) => action(), errorHandler, period, delayFirstIteration)
        {
        }

        public AsyncPeriodicalAction(
            [NotNull] Func<CancellationToken, TimeSpan, Task> action,
            [NotNull] Action<Exception> errorHandler,
            [NotNull] Func<TimeSpan> period,
            bool delayFirstIteration = false)
        {
            this.action = action;
            this.errorHandler = errorHandler;
            this.period = period;
            this.delayFirstIteration = delayFirstIteration;
        }

        public bool IsRunning
        {
            get
            {
                lock (startStopSync)
                {
                    return workerTask != null;
                }
            }
        }

        public void Start()
        {
            lock (startStopSync)
            {
                if (workerTask != null)
                    return;

                cancellationSource = new CancellationTokenSource();
                workerTask = Task.Run(() => WorkerRoutine(cancellationSource.Token));
            }
        }

        public void Stop()
        {
            lock (startStopSync)
            {
                if (workerTask == null)
                    return;

                cancellationSource.Cancel();
                workerTask.GetAwaiter().GetResult();

                cancellationSource.Dispose();
                workerTask.Dispose();

                cancellationSource = null;
                workerTask = null;
            }
        }

        private static async Task DelaySafe(TimeSpan delay, CancellationToken token)
        {
            try
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task WorkerRoutine(CancellationToken token)
        {
            if (delayFirstIteration)
                await DelaySafe(period(), token).ConfigureAwait(false);

            while (!token.IsCancellationRequested)
            {
                var budget = TimeBudget.StartNew(period(), TimeSpan.FromMilliseconds(1));

                try
                {
                    await action(token, budget.Remaining).ConfigureAwait(false);
                }
                catch (OperationCanceledException e) when (e.CancellationToken == token && token.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception error)
                {
                    errorHandler(error);
                }

                await DelaySafe(budget.Remaining, token).ConfigureAwait(false);
            }
        }
    }
}