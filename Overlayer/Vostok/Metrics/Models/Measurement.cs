using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Vostok.Metrics.Primitives.Timer;

namespace Vostok.Metrics.Models
{
    [PublicAPI]
    public class Measurement : Stopwatch, IDisposable
    {
        private readonly ITimer timer;

        public Measurement(ITimer timer)
        {
            this.timer = timer;

            Start();
        }

        public void Dispose()
        {
            Stop();
            timer.Report(Elapsed);
        }
    }
}