using System;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Time
{
    [PublicAPI]
    internal class PeriodicalAction
    {
        private const int StateStopped = 0;
        private const int StateRunning = 1;

        private readonly Action workerRoutine;
        private readonly Action<Exception> errorHandler;
        private readonly AutoResetEvent stopEvent;
        private readonly object startStopSync = new object();

        private volatile int state;
        private volatile Thread workerThread;

        public PeriodicalAction(
            [NotNull] Action action,
            [NotNull] Action<Exception> errorHandler,
            [NotNull] Func<TimeSpan> period,
            bool delayFirstIteration = false)
            : this(_ => action(), errorHandler, period, delayFirstIteration)
        {
        }

        public PeriodicalAction(
            [NotNull] Action<TimeSpan> action,
            [NotNull] Action<Exception> errorHandler,
            [NotNull] Func<TimeSpan> period,
            bool delayFirstIteration = false)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (period == null)
                throw new ArgumentNullException(nameof(period));

            this.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));

            stopEvent = new AutoResetEvent(false);

            workerRoutine = () =>
            {
                if (delayFirstIteration)
                    stopEvent.WaitOne(period());

                while (IsRunning)
                {
                    var actionBudget = TimeBudget.StartNew(period());

                    try
                    {
                        action(actionBudget.Remaining);
                    }
                    catch (Exception error)
                    {
                        errorHandler(error);
                    }

                    stopEvent.WaitOne(actionBudget.Remaining);
                }
            };
        }

        public bool IsRunning => state == StateRunning;

        public void Start()
        {
            lock (startStopSync)
            {
                if (Interlocked.CompareExchange(ref state, StateRunning, StateStopped) != StateStopped)
                    return;

                workerThread = new Thread(
                    _ =>
                    {
                        try
                        {
                            workerRoutine();
                        }
                        catch (ThreadAbortException)
                        {
                        }
                    })
                {
                    IsBackground = true
                };

                workerThread.Start();
            }
        }

        public void Stop()
        {
            lock (startStopSync)
            {
                if (Interlocked.CompareExchange(ref state, StateStopped, StateRunning) != StateRunning)
                    return;

                stopEvent.Set();

                workerThread?.Join();
                workerThread = null;

                stopEvent.Reset();
            }
        }
    }
}