// ReSharper disable All
// Copied from https://github.com/MiloszKrajewski/K4os.Compression.LZ4

using JetBrains.Annotations;

namespace K4os.Compression.LZ4.Legacy
{
    /// <summary>
    /// Utility class with factory methods to create legacy LZ4 (lz4net) compression and decompression streams.
    /// </summary>
    [PublicAPI]
    internal static class LZ4Legacy
    {
        /// <summary>Compresses and wraps given input byte buffer.</summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <returns>Compressed buffer.</returns>
        /// <exception cref="System.ArgumentException">inputBuffer size of inputLength is invalid</exception>
        public static byte[] Wrap(
            byte[] inputBuffer,
            int inputOffset = 0,
            int inputLength = int.MaxValue) =>
            LZ4Wrapper.Wrap(inputBuffer, inputOffset, inputLength);

        /// <summary>Unwraps the specified compressed buffer.</summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <returns>Uncompressed buffer.</returns>
        /// <exception cref="System.ArgumentException">
        ///     inputBuffer size is invalid or inputBuffer size is invalid or has been corrupted
        /// </exception>
        public static byte[] Unwrap(byte[] inputBuffer, int inputOffset = 0) =>
            LZ4Wrapper.Unwrap(inputBuffer, inputOffset);
    }
}