using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Url
{
    [PublicAPI]
    internal static class UrlNormalizer
    {
        private const char Slash = '/';
        private const char Tilde = '~';
        private const char Dash = '-';

        private const int DefaultLengthLimit = 100;

        private const string TruncatedText = "...(truncated)";

        private static readonly bool[] HexCharactersMap;
        private static readonly bool[] AllowedCharactersMap;

        private static readonly IDetector[] Detectors =
        {
            new LongSegmentsDetector(),
            new ExoticCharactersDetector(),
            new HexSequencesDetector(),
            new NumericSegmentsDetector()
        };

        [ThreadStatic]
        private static StringBuilder resultBuilder;

        static UrlNormalizer()
        {
            PrepareCharacterMaps(out HexCharactersMap, out AllowedCharactersMap);
        }

        public static string NormalizePath(Uri url, int maxLength = DefaultLengthLimit)
        {
            return NormalizePath(url.IsAbsoluteUri ? url.AbsolutePath : url.ToStringWithoutQuery(), maxLength);
        }

        public static string NormalizePath(string path, int maxLength = DefaultLengthLimit)
        {
            var builder = ObtainBuilder();

            foreach (var segment in EnumerateSegments(path))
            {
                if (RequiresSubstitution(segment))
                {
                    builder.Append(Tilde);
                }
                else
                {
                    for (var i = 0; i < segment.Length; i++)
                    {
                        builder.Append(ToLowerFast(segment[i]));
                    }
                }

                if (builder.Length > maxLength)
                    break;

                builder.Append(Slash);
            }

            HandleTrailingSlash(builder);

            TruncateIfNeeded(builder, maxLength);

            return builder.ToString();
        }

        private static StringBuilder ObtainBuilder()
        {
            var builder = resultBuilder ?? (resultBuilder = new StringBuilder(64));

            builder.Clear();

            return builder;
        }

        private static IEnumerable<Segment> EnumerateSegments(string path)
        {
            var segmentBeginning = 0;

            for (var i = 0; i < path.Length; i++)
            {
                var current = path[i];
                if (current == Slash)
                {
                    if (i > segmentBeginning)
                    {
                        yield return new Segment(path, segmentBeginning, i - segmentBeginning);
                    }

                    segmentBeginning = i + 1;
                }
            }

            if (segmentBeginning < path.Length)
            {
                yield return new Segment(path, segmentBeginning, path.Length - segmentBeginning);
            }
        }

        private static bool RequiresSubstitution(Segment segment)
        {
            if (segment.IsEmpty)
                return false;

            for (var i = 0; i < Detectors.Length; i++)
            {
                if (Detectors[i].IsLikelyUnique(segment))
                    return true;
            }

            return false;
        }

        private static void HandleTrailingSlash(StringBuilder builder)
        {
            if (builder.Length == 0)
            {
                builder.Append(Slash);
                return;
            }

            if (builder[builder.Length - 1] == Slash && builder.Length > 1)
                builder.Length--;
        }

        private static void TruncateIfNeeded(StringBuilder builder, int maxLength)
        {
            if (builder.Length <= maxLength)
                return;

            maxLength = Math.Max(maxLength, TruncatedText.Length);

            builder.Length = maxLength - TruncatedText.Length;

            builder.Append(TruncatedText);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char ToLowerFast(char c)
        {
            if (c >= 'A' && c <= 'Z')
            {
                return (char)(c + 32);
            }

            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsHexCharacter(char c)
        {
            return c < 128 && HexCharactersMap[c];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAllowedCharacter(char c)
        {
            return c < 128 && AllowedCharactersMap[c];
        }

        #region LongSegmentsDetector

        private class LongSegmentsDetector : IDetector
        {
            private const int LengthThreshold = 40;

            public bool IsLikelyUnique(Segment segment)
            {
                return segment.Length >= LengthThreshold;
            }
        }

        #endregion

        #region ExoticCharactersDetector

        private class ExoticCharactersDetector : IDetector
        {
            public bool IsLikelyUnique(Segment segment)
            {
                for (var i = 0; i < segment.Length; i++)
                {
                    if (!IsAllowedCharacter(segment[i]))
                        return true;
                }

                return false;
            }
        }

        #endregion

        #region HexSequencesDetector

        private class HexSequencesDetector : IDetector
        {
            private const int MinimumSequenceLength = 8;

            public bool IsLikelyUnique(Segment segment)
            {
                var consecutiveHexChars = 0;

                for (var i = 0; i < segment.Length; i++)
                {
                    if (IsHexCharacter(segment[i]))
                    {
                        if (++consecutiveHexChars == MinimumSequenceLength)
                            return true;
                    }
                    else
                    {
                        consecutiveHexChars = 0;
                    }
                }

                return false;
            }
        }

        #endregion

        #region NumericSegmentsDetector

        private class NumericSegmentsDetector : IDetector
        {
            public bool IsLikelyUnique(Segment segment)
            {
                var sawDigits = false;

                for (var i = 0; i < segment.Length; i++)
                {
                    var current = segment[i];
                    var isDigit = current >= '0' && current <= '9';
                    var isDash = current == Dash;

                    if (!isDash && !isDigit)
                        return false;

                    sawDigits |= isDigit;
                }

                return sawDigits;
            }
        }

        #endregion

        #region Segment

        private struct Segment
        {
            public readonly int Length;

            private readonly string Path;
            private readonly int Offset;

            public Segment(string path, int offset, int length)
            {
                Path = path;
                Offset = offset;
                Length = length;
            }

            public bool IsEmpty => Length == 0;

            public char this[int index] => Path[Offset + index];
        }

        #endregion

        #region IDetector

        private interface IDetector
        {
            bool IsLikelyUnique(Segment segment);
        }

        #endregion

        #region Characters

        private static void PrepareCharacterMaps(out bool[] hexMap, out bool[] allowedMap)
        {
            hexMap = new bool[128];

            foreach (var c in EnumerateHexCharacters())
            {
                hexMap[c] = true;
            }

            allowedMap = new bool[128];

            foreach (var c in EnumerateAllowedCharacters())
            {
                allowedMap[c] = true;
            }
        }

        private static IEnumerable<char> EnumerateHexCharacters()
        {
            for (var c = 'a'; c <= 'f'; c++)
                yield return c;

            for (var c = 'A'; c <= 'F'; c++)
                yield return c;

            for (var c = '0'; c <= '9'; c++)
                yield return c;
        }

        private static IEnumerable<char> EnumerateAllowedCharacters()
        {
            for (var c = 'a'; c <= 'z'; c++)
                yield return c;

            for (var c = 'A'; c <= 'Z'; c++)
                yield return c;

            for (var c = '0'; c <= '9'; c++)
                yield return c;

            yield return '.';
            yield return '-';
            yield return '_';
            yield return '~';
            yield return '*';
            yield return '(';
            yield return ')';
        }

        #endregion
    }
}