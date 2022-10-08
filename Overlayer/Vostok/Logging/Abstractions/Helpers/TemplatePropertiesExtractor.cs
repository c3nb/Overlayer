using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Vostok.Commons.Collections;

namespace Vostok.Logging.Abstractions.Helpers
{
    internal static class TemplatePropertiesExtractor
    {
        private const int CacheCapacity = 10000;

        private const char OpeningBrace = '{';
        private const char ClosingBrace = '}';
        private const char Underscore = '_';
        private const char Whitespace = ' ';
        private const char Colon = ':';
        private const char Dot = '.';

        private static readonly RecyclingBoundedCache<string, string[]> Cache
            = new RecyclingBoundedCache<string, string[]>(CacheCapacity, StringComparer.Ordinal);

        public static string[] ExtractPropertyNames([CanBeNull] string template)
        {
            if (string.IsNullOrEmpty(template))
                return Array.Empty<string>();

            return Cache.Obtain(template, t => ExtractPropertyNamesInternal(t).ToArray());
        }

        private static IEnumerable<string> ExtractPropertyNamesInternal([NotNull] string template)
        {
            var offset = 0;

            while (true)
            {
                SkipText(template, ref offset);

                if (offset >= template.Length)
                    yield break;

                var propertyName = TryExtractPropertyName(template, offset, out offset);
                if (propertyName != null)
                    yield return propertyName;

                if (offset >= template.Length)
                    yield break;
            }
        }

        private static void SkipText(string template, ref int offset)
        {
            do
            {
                var currentCharacter = template[offset];
                if (currentCharacter == OpeningBrace)
                {
                    // (iloktionov): When we encounter an opening brace:
                    // (iloktionov): 1. If the next symbol is also an opening brace, we consume it as text (escaping).
                    // (iloktionov): 2. If the next symbol is something different, we end the text token and try to parse a named token.
                    if (offset + 1 < template.Length && template[offset + 1] == OpeningBrace)
                    {
                        offset++;
                    }
                    else break;
                }
                else
                {
                    // (iloktionov): Handle escaping with double braces:
                    if (currentCharacter == ClosingBrace)
                    {
                        if (offset + 1 < template.Length && template[offset + 1] == ClosingBrace)
                        {
                            offset++;
                        }
                    }
                }

                offset++;
            } while (offset < template.Length);
        }

        [CanBeNull]
        private static string TryExtractPropertyName(string template, int offset, out int next)
        {
            var beginning = offset++;

            // (iloktionov): Just move on until we encounter something that should not be in a named token:
            while (offset < template.Length && IsValidInNamedToken(template[offset]))
                offset++;

            // (iloktionov): If we reached the end of template or didn't stop on a closing brace, there will be no named token:
            if (offset == template.Length || template[offset] != ClosingBrace)
            {
                next = offset;
                return null;
            }

            next = offset + 1;

            // (iloktionov): Raw content is token with braces included, like '{prop:format}'.
            var rawOffset = beginning;
            var rawLength = next - rawOffset;

            // (iloktionov): Token content is token without braces, like 'prop:format'.
            var tokenOffset = rawOffset + 1;
            var tokenLength = rawLength - 2;

            if (TryParsePropertyName(template, tokenOffset, tokenLength, out var name))
                return name;

            return null;
        }

        private static bool TryParsePropertyName(string template, int offset, int length, out string name)
        {
            name = null;

            if (length == 0)
                return false;

            var formatDelimiter = template.IndexOf(Colon, offset, length);

            name = formatDelimiter < offset
                ? template.Substring(offset, length)
                : template.Substring(offset, formatDelimiter - offset);

            return IsValidName(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            for (var i = 0; i < name.Length; i++)
                if (!IsValidInName(name[i]))
                    return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidInNamedToken(char c)
        {
            return IsValidInName(c) || IsValidInFormat(c) || c == Colon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidInName(char c)
        {
            return char.IsLetterOrDigit(c) || c == Underscore || c == Dot;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidInFormat(char c)
        {
            return c != ClosingBrace && (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || c == Whitespace);
        }
    }
}
