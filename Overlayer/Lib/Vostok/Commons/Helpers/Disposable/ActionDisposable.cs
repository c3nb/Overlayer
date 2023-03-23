using System;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Disposable
{
    [PublicAPI]
    internal class ActionDisposable : IDisposable
    {
        private volatile Action dispose;

        public ActionDisposable(Action dispose) =>
            this.dispose = dispose;

        public void Dispose() =>
            Interlocked.Exchange(ref dispose, null)?.Invoke();
    }
}