// ReSharper disable All
// Copied from https://github.com/MiloszKrajewski/K4os.Compression.LZ4

using System;

namespace K4os.Compression.LZ4.Legacy
{
    /// <summary>
    /// Legacy picker for LZ4. Do not use unless you have dependency on some old data.
    /// </summary>
    internal class LZ4Wrapper
    {
        private const int WRAP_OFFSET_0 = 0;
        private const int WRAP_OFFSET_4 = sizeof(int);
        private const int WRAP_OFFSET_8 = 2 * sizeof(int);
        private const int WRAP_LENGTH = WRAP_OFFSET_8;

        /// <summary>Compresses and wraps given input byte buffer.</summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <returns>Compressed buffer.</returns>
        /// <exception cref="System.ArgumentException">inputBuffer size of inputLength is invalid</exception>
        public static byte[] Wrap(
            byte[] inputBuffer,
            int inputOffset,
            int inputLength)
        {
            inputLength = Math.Min(inputBuffer.Length - inputOffset, inputLength);
            if (inputLength < 0)
                throw new ArgumentException("inputBuffer size of inputLength is invalid");

            if (inputLength == 0)
                return new byte[WRAP_LENGTH];

            var outputLength = inputLength; // MaximumOutputLength(inputLength);
            var outputBuffer = new byte[outputLength];

            outputLength = LZ4Codec.Encode(
                inputBuffer,
                inputOffset,
                inputLength,
                outputBuffer,
                0,
                outputLength);

            byte[] result;

            if (outputLength >= inputLength || outputLength <= 0)
            {
                result = new byte[inputLength + WRAP_LENGTH];
                Poke32(result, WRAP_OFFSET_0, (uint)inputLength);
                Poke32(result, WRAP_OFFSET_4, (uint)inputLength);
                Buffer.BlockCopy(inputBuffer, inputOffset, result, WRAP_OFFSET_8, inputLength);
            }
            else
            {
                result = new byte[outputLength + WRAP_LENGTH];
                Poke32(result, WRAP_OFFSET_0, (uint)inputLength);
                Poke32(result, WRAP_OFFSET_4, (uint)outputLength);
                Buffer.BlockCopy(outputBuffer, 0, result, WRAP_OFFSET_8, outputLength);
            }

            return result;
        }

        /// <summary>Unwraps the specified compressed buffer.</summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <returns>Uncompressed buffer.</returns>
        /// <exception cref="System.ArgumentException">
        ///     inputBuffer size is invalid or inputBuffer size is invalid or has been corrupted
        /// </exception>
        public static byte[] Unwrap(byte[] inputBuffer, int inputOffset = 0)
        {
            var inputLength = inputBuffer.Length - inputOffset;
            if (inputLength < WRAP_LENGTH)
                throw new ArgumentException("inputBuffer size is invalid");

            var outputLength = (int)Peek32(inputBuffer, inputOffset + WRAP_OFFSET_0);
            inputLength = (int)Peek32(inputBuffer, inputOffset + WRAP_OFFSET_4);
            if (inputLength > inputBuffer.Length - inputOffset - WRAP_LENGTH)
                throw new ArgumentException("inputBuffer size is invalid or has been corrupted");

            byte[] result;

            if (inputLength >= outputLength)
            {
                result = new byte[inputLength];
                Buffer.BlockCopy(
                    inputBuffer,
                    inputOffset + WRAP_OFFSET_8,
                    result,
                    0,
                    inputLength);
            }
            else
            {
                result = new byte[outputLength];
                LZ4Codec.Decode(
                    inputBuffer,
                    inputOffset + WRAP_OFFSET_8,
                    inputLength,
                    result,
                    0,
                    outputLength);
            }

            return result;
        }

        /// <summary>Sets uint32 value in byte buffer.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        private static void Poke32(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }

        /// <summary>Gets uint32 from byte buffer.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The value.</returns>
        private static uint Peek32(byte[] buffer, int offset)
        {
            // NOTE: It's faster than BitConverter.ToUInt32 (surprised? me too)
            return
                // ReSharper disable once RedundantCast
                ((uint)buffer[offset]) |
                ((uint)buffer[offset + 1] << 8) |
                ((uint)buffer[offset + 2] << 16) |
                ((uint)buffer[offset + 3] << 24);
        }
    }
}