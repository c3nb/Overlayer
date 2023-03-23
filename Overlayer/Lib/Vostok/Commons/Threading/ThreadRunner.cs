using System;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    /// <summary>
    /// Runs action in separate thread. Rarely needed in code. Use Task for most cases
    /// </summary>
    [PublicAPI]
    internal class ThreadRunner
    {
        public static Thread Run(
            [NotNull] Action<object> threadRoutine,
            [CanBeNull] Action<Exception> errorHandler = null,
            [CanBeNull] Action<Thread> customizeThreadBeforeStart = null)
        {
            var t = new Thread(Wrap(threadRoutine, errorHandler))
            {
                IsBackground = true
            };
            customizeThreadBeforeStart?.Invoke(t);
            t.Start();
            return t;
        }

        public static Thread Run(
            [NotNull] Action threadRoutine,
            [CanBeNull] Action<Exception> errorHandler = null,
            [CanBeNull] Action<Thread> customizeThreadBeforeStart = null)
        {
            return Run(o => threadRoutine(), errorHandler, customizeThreadBeforeStart);
        }

        private static ParameterizedThreadStart Wrap(Action<object> threadRoutine, Action<Exception> errorHandler)
        {
            return parameter =>
            {
                try
                {
                    threadRoutine(parameter);
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
                catch (Exception e)
                {
                    errorHandler?.Invoke(e);
                }
            };
        }
    }
}