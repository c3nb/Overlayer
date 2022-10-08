using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers
{
    [PublicAPI]
    internal static class NumericTypeParser<TNumber>
    {
        private static readonly TryParseDelegate TryParseMethod = GenerateTryParseMethod();

        // ReSharper disable once StaticMemberInGenericType
        private static readonly CultureInfo[] AllCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToArray();

        public static bool TryParse(string input, out TNumber result)
        {
            if (TryParseMethod == null)
                throw new NotSupportedException($"{typeof(TNumber)} is not a floating-point number type.");

            if (input == null)
            {
                result = default;
                return false;
            }

            if (TryParseMethod(input.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return true;

            foreach (var culture in AllCultures)
            {
                if (TryParseMethod(input, NumberStyles.Any, culture, out result))
                    return true;
            }

            return false;
        }

        public static TNumber Parse(string input)
        {
            if (TryParse(input, out var result))
                return result;

            throw new FormatException($"The provided string '{input}' is not a valid {typeof(TNumber)}.");
        }

        private static TryParseDelegate GenerateTryParseMethod()
        {
            var method = typeof(TNumber).GetMethod("TryParse", new[] {typeof(string), typeof(NumberStyles), typeof(IFormatProvider), typeof(TNumber).MakeByRefType()});

            if (method == null)
                return null;

            var input = Expression.Parameter(typeof(string));
            var style = Expression.Parameter(typeof(NumberStyles));
            var provider = Expression.Parameter(typeof(IFormatProvider));
            var result = Expression.Parameter(typeof(TNumber).MakeByRefType());

            var body = Expression.Call(null, method, input, style, provider, result);

            return Expression.Lambda<TryParseDelegate>(body, input, style, provider, result).Compile();
        }

        private delegate bool TryParseDelegate(string input, NumberStyles style, IFormatProvider provider, out TNumber result);
    }
}