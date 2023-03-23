using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers
{
    [PublicAPI]
    internal class Enum<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly bool IsEnum;
        private static readonly Dictionary<string, T> KeyToValue;
        private static readonly Dictionary<string, T> KeyToValueIgnoreCase;

        static Enum()
        {
            IsEnum = typeof(T).IsEnum;

            if (!IsEnum)
                return;

            var enumKeys = Enum.GetNames(typeof(T));
            var enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToArray();

            KeyToValue = new Dictionary<string, T>(enumKeys.Length, StringComparer.Ordinal);
            KeyToValueIgnoreCase = new Dictionary<string, T>(enumKeys.Length, StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < enumKeys.Length; i++)
            {
                KeyToValue.Add(enumKeys[i], enumValues[i]);
                KeyToValueIgnoreCase.Add(enumKeys[i], enumValues[i]);
            }
        }

        public static T Parse(string input, bool ignoreCase = false)
        {
            ValidateType();

            var keyToValue = ignoreCase ? KeyToValueIgnoreCase : KeyToValue;

            if (string.IsNullOrEmpty(input) || !keyToValue.TryGetValue(input, out var parsed))
                throw new FormatException($"'{input}' is not a valid {typeof(T)}.");

            return parsed;
        }

        public static bool TryParse(string input, out T parsed, bool ignoreCase = false)
        {
            ValidateType();

            parsed = default;

            var keyToValue = ignoreCase ? KeyToValueIgnoreCase : KeyToValue;

            return !string.IsNullOrEmpty(input) && keyToValue.TryGetValue(input, out parsed);
        }

        public static T[] GetValues() => KeyToValue.Values.ToArray();

        private static void ValidateType()
        {
            if (!IsEnum)
                throw new InvalidOperationException($"Type '{typeof(T)}' is not a enum.");
        }
    }
}