using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Extensions
{
    [PublicAPI]
    internal static class StringExtensions
    {
        // (iloktionov): Default implementation of string.GetHashCode() returns values that may vary from machine to machine.
        // (iloktionov): We require a stable hash code implementation in order to facilitate sharding of metric events by tags hash.
        public static int GetStableHashCode(this string str)
        {
            unchecked
            {
                if (string.IsNullOrEmpty(str))
                    return 0;

                var length = str.Length;
                var hash = (uint)length;

                var remainder = length & 1;

                length >>= 1;

                var index = 0;
                for (; length > 0; length--)
                {
                    hash += str[index];
                    var temp = (uint)(str[index + 1] << 11) ^ hash;
                    hash = (hash << 16) ^ temp;
                    index += 2;
                    hash += hash >> 11;
                }

                if (remainder == 1)
                {
                    hash += str[index];
                    hash ^= hash << 11;
                    hash += hash >> 17;
                }

                hash ^= hash << 3;
                hash += hash >> 5;
                hash ^= hash << 4;
                hash += hash >> 17;
                hash ^= hash << 25;
                hash += hash >> 6;

                return (int)hash;
            }
        }

        public static bool IgnoreCaseEquals(this string value, string other) =>
            string.Equals(value, other, StringComparison.OrdinalIgnoreCase);

        public static string[] SplitRemovingEmpties(this string input, params char[] separator) =>
            input.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        public static string[] SplitRemovingEmpties(this string input, params string[] separator) =>
            input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
    }
}