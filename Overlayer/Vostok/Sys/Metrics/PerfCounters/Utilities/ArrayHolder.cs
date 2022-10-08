using System;

namespace Vostok.Sys.Metrics.PerfCounters.Utilities
{
    internal class ArrayHolder
    {
        private const int MinSize = 64;
        private const int MinSizeForShrink = 16 * 1024;

        private byte[] array = Array.Empty<byte>();

        public byte[] Get(int size)
        {
            if (array.Length < size)
                Reallocate(size);
            return array;
        }

        public void Shrink(int size)
        {
            if (size < MinSizeForShrink)
                return;

            if (array.Length > 2 * size)
                Reallocate(size);
        }

        private static int GetSize(int minSize) =>
            minSize > int.MaxValue / 4 ? minSize : Math.Max(4 * minSize / 3, MinSize);

        private void Reallocate(int minSize) => array = new byte[GetSize(minSize)];
    }
}