using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Vostok.Commons.Threading
{
    [PublicAPI]
    internal static class ThreadSafeRandom
    {
#if NET6_0_OR_GREATER
#else
        [ThreadStatic]
        private static Random random;
#endif

        public static double NextDouble()
        {
            return ObtainThreadStaticRandom().NextDouble();
        }

        public static int Next()
        {
            return ObtainThreadStaticRandom().Next();
        }

        public static int Next(int maxValue)
        {
            return ObtainThreadStaticRandom().Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return ObtainThreadStaticRandom().Next(minValue, maxValue);
        }

        public static long Next(long minValue, long maxValue)
        {
            if (minValue > maxValue)
                ThrowArgumentOutOfRangeException(minValue, maxValue);
            if (minValue == maxValue)
                return minValue;

            return Math.Abs(BitConverter.ToInt64(NextBytes(8), 0) % (maxValue - minValue)) + minValue;
        }

        public static void NextBytes(byte[] buffer)
        {
            ObtainThreadStaticRandom().NextBytes(buffer);
        }

        public static byte[] NextBytes(long size)
        {
            var buffer = new byte[size];

            NextBytes(buffer);

            return buffer;
        }

        public static bool FlipCoin()
        {
            return NextDouble() <= 0.5;
        }

        /// <summary>
        /// Be careful! This method returns an instance of the class <see cref="Random"/> with <see cref="ThreadStaticAttribute"/> attribute. It is safe to use this instance only in a synchronous block of code, such as a loop.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Random ObtainThreadStaticRandom()
        {
#if NET6_0_OR_GREATER
            return Random.Shared;
#else
            return random ?? (random = new Random(Guid.NewGuid().GetHashCode()));
#endif
        }

        private static void ThrowArgumentOutOfRangeException(long minValue, long maxValue) =>
            throw new ArgumentOutOfRangeException(nameof(minValue), $"minValue {minValue} is greater than maxValue {maxValue}");
    }
}