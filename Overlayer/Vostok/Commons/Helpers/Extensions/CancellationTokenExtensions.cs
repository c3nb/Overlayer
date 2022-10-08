using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Extensions
{
    [PublicAPI]
    internal static class CancellationTokenExtensions
    {
        public static async Task WaitAsync(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            using (cancellationToken.Register(o => ((TaskCompletionSource<bool>)o).TrySetCanceled(), tcs))
            {
                try
                {
                    await tcs.Task.ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }
    }
}