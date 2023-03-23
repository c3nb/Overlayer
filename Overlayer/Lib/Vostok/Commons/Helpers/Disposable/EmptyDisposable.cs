using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Disposable
{
    [PublicAPI]
    internal class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}