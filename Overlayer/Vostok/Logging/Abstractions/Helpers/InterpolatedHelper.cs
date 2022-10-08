#if NET6_0_OR_GREATER
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Vostok.Logging.Abstractions.Helpers
{
    internal static class InterpolatedHelper
    {
        private const char Underscore = '_';
        private const char At = '@';
        private const char Dot = '.';

        [Pure]
        public static string EscapeName(string name)
        {
            var shouldEscape = false;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < name.Length; i++)
            {
                if (!IsValidInName(name[i]))
                {
                    shouldEscape = true;
                    break;
                }
            }

            if (!shouldEscape)
                return name;

            return string.Create(name.Length,
                name,
                (chars, buf) =>
                {
                    for (var i = 0; i < chars.Length; i++)
                        chars[i] = IsValidInName(buf[i]) ? buf[i] : Underscore;
                });
        }

        // note (kungurtsev, 25.01.2022): copied from Vostok.Logging.Formatting.Tokenizer.TemplateTokenizer
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidInName(char c) =>
            char.IsLetterOrDigit(c) || c == Underscore || c == At || c == Dot;
    }
}
#endif