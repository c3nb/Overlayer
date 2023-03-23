using System;
#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal static class GuidGenerator
    {
#if NET6_0_OR_GREATER
        [SkipLocalsInit]
        public static unsafe Guid GenerateNotCryptoQualityGuid()
        {
            var bytes = stackalloc byte[16];
            
            Random.Shared.NextBytes(new Span<byte>(bytes, 16));

            return *(Guid*)bytes;
        }
#else
        public static unsafe Guid GenerateNotCryptoQualityGuid()
        {
            var bytes = stackalloc byte[16];
            var dst = bytes;

            var random = ThreadSafeRandom.ObtainThreadStaticRandom();
            
            for (var i = 0; i < 4; i++)
            {
                *(int*)dst = random.Next();
                dst += 4;
            }

            return *(Guid*)bytes;
        }
#endif
    }
}
