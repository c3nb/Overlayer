using System;
using JetBrains.Annotations;
using K4os.Compression.LZ4.Engine;

// ReSharper disable All
// Copied from https://github.com/MiloszKrajewski/K4os.Compression.LZ4

namespace K4os.Compression.LZ4
{
    /// <summary>
    /// Static class exposing LZ4 block compression methods.
    /// </summary>
    [PublicAPI]
    internal class LZ4Codec
    {
        /// <summary>Maximum size after compression.</summary>
        /// <param name="length">Length of input buffer.</param>
        /// <returns>Maximum length after compression.</returns>
        public static int MaximumOutputSize(int length) =>
            LZ4_xx.LZ4_compressBound(length);

        /// <summary>Compresses data from one buffer into another.</summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="sourceOffset">Input buffer offset.</param>
        /// <param name="sourceLength">Input buffer length.</param>
        /// <param name="target">Output buffer.</param>
        /// <param name="targetOffset">Output buffer offset.</param>
        /// <param name="targetLength">Output buffer length.</param>
        /// <returns>Number of bytes written, or negative value if output buffer is too small.</returns>
        public static unsafe int Encode(
            byte[] source,
            int sourceOffset,
            int sourceLength,
            byte[] target,
            int targetOffset,
            int targetLength)
        {
            Validate(source, sourceOffset, sourceLength);
            Validate(target, targetOffset, targetLength);

            fixed (byte* sourceP = source)
            fixed (byte* targetP = target)
                return Encode(
                    sourceP + sourceOffset,
                    sourceLength,
                    targetP + targetOffset,
                    targetLength);
        }

        /// <summary>Decompresses data from given buffer.</summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="sourceOffset">Input buffer offset.</param>
        /// <param name="sourceLength">Input buffer length.</param>
        /// <param name="target">Output buffer.</param>
        /// <param name="targetOffset">Output buffer offset.</param>
        /// <param name="targetLength">Output buffer length.</param>
        /// <returns>Number of bytes written, or negative value if output buffer is too small.</returns>
        public static unsafe int Decode(
            byte[] source,
            int sourceOffset,
            int sourceLength,
            byte[] target,
            int targetOffset,
            int targetLength)
        {
            Validate(source, sourceOffset, sourceLength);
            Validate(target, targetOffset, targetLength);

            fixed (byte* sourceP = source)
            fixed (byte* targetP = target)
                return Decode(
                    sourceP + sourceOffset,
                    sourceLength,
                    targetP + targetOffset,
                    targetLength);
        }

        /// <summary>Compresses data from one buffer into another.</summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="sourceLength">Length of input buffer.</param>
        /// <param name="target">Output buffer.</param>
        /// <param name="targetLength">Output buffer length.</param>
        /// <returns>Number of bytes written, or negative value if output buffer is too small.</returns>
        private static unsafe int Encode(
            byte* source,
            int sourceLength,
            byte* target,
            int targetLength)
        {
            if (sourceLength <= 0)
                return 0;

            var encoded = LZ4_64.LZ4_compress_default(source, target, sourceLength, targetLength);
            return encoded <= 0 ? -1 : encoded;
        }

        /// <summary>Decompresses data from given buffer.</summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="sourceLength">Input buffer length.</param>
        /// <param name="target">Output buffer.</param>
        /// <param name="targetLength">Output buffer length.</param>
        /// <returns>Number of bytes written, or negative value if output buffer is too small.</returns>
        private static unsafe int Decode(
            byte* source,
            int sourceLength,
            byte* target,
            int targetLength)
        {
            if (sourceLength <= 0)
                return 0;

            var decoded = LZ4_xx.LZ4_decompress_safe(source, target, sourceLength, targetLength);
            return decoded <= 0 ? -1 : decoded;
        }

        private static void Validate<T>(T[] buffer, int offset, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException(
                    nameof(buffer),
                    "cannot be null");

            var valid = offset >= 0 && length >= 0 && offset + length <= buffer.Length;
            if (!valid)
                throw new ArgumentException(
                    $"invalid offset/length combination: {offset}/{length}");
        }
    }
}