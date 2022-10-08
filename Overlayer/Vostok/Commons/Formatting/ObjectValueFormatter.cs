using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Vostok.Commons.Formatting
{
    /// <summary>
    /// <para>A helper used to format arbitrary object values.</para>
    /// <para>See <see cref="Format(TextWriter,object,string,IFormatProvider)"/> method for details.</para>
    /// </summary>
    [PublicAPI]
    internal static class ObjectValueFormatter
    {
        private const int StringBuilderCapacity = 64;
        private const int MaximumRecursionDepth = 10;

        /// <inheritdoc cref="Format(TextWriter,object,string,IFormatProvider)"/>
        public static string Format(
            [CanBeNull] object value,
            [CanBeNull] string format = null,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            var builder = StringBuilderCache.Acquire(StringBuilderCapacity);
            var writer = new StringWriter(builder);

            Format(writer, value, format, formatProvider);

            StringBuilderCache.Release(builder);

            return builder.ToString();
        }

        /// <summary>
        /// <para>Formats given property <paramref name="value"/>.</para>
        /// <para>Here's how it works:</para>
        /// <list type="number">
        ///     <item><description>If <paramref name="value"/> is <c>null</c>, nothing happens. <para/></description></item>
        ///     <item><description>If <paramref name="value"/> is a <see cref="string"/>, it's written as is. <para/></description></item>
        ///     <item><description>If <paramref name="value"/> implements <see cref="IFormattable"/>, it's formatted using given <paramref name="format"/> and <paramref name="formatProvider"/>. <para/></description></item>
        ///     <item><description>If <paramref name="value"/>'s type explicitly overrides <see cref="Object.ToString"/>, we just use that. <para/></description></item>
        ///     <item><description>If <paramref name="value"/> implements <see cref="IReadOnlyDictionary{TKey,TValue}"/>, it's formatted as a JSON object. <para/></description></item>
        ///     <item><description>If <paramref name="value"/> implements <see cref="IEnumerable"/>, it's formatted as a JSON array. <para/></description></item>
        ///     <item><description>If <paramref name="value"/>'s type has any properties with public getters, it's formatted as JSON object. <para/></description></item>
        ///     <item><description>If nothing of above applies, default <see cref="Object.ToString"/> result is used. <para/></description></item>
        /// </list>
        /// </summary>
        public static void Format(
            [NotNull] TextWriter writer,
            [CanBeNull] object value,
            [CanBeNull] string format = null,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (value == null)
                return;

            Type valueType;

            if (value is string str)
                writer.Write(str);
            else if (value is IFormattable formattable)
                writer.Write(formattable.ToString(format, formatProvider ?? CultureInfo.InvariantCulture));
            else if (HasCustomToString(valueType = value.GetType()))
                writer.Write(value.ToString());
            else if (IsSimpleDictionary(valueType))
                FormatDictionaryAsJson(writer, value, 1);
            else if (value is IEnumerable<byte> bytes)
                writer.Write(Convert.ToBase64String(bytes.ToArray()));
            else if (value is IEnumerable enumerable)
                FormatSequenceAsJson(writer, enumerable, 1);
            else if (HasPublicProperties(valueType))
                FormatObjectPropertiesAsJson(writer, value, 1);
            else
                writer.Write(value.ToString());
        }

        private static bool HasCustomToString(Type type) =>
            ToStringDetector.HasCustomToString(type);

        private static bool IsSimpleDictionary(Type type) =>
            DictionaryInspector.IsSimpleDictionary(type);

        private static bool HasPublicProperties(Type type) =>
            ObjectPropertiesExtractor.HasProperties(type);

        private static void FormatAsJson(TextWriter writer, object value, int depth)
        {
            Type valueType;

            if (value == null)
                writer.Write("null");
            else if (depth > MaximumRecursionDepth)
                writer.Write("\"<too deep>\"");
            else if (HasCustomToString(valueType = value.GetType()))
                FormatValueAsJson(writer, value);
            else if (IsSimpleDictionary(valueType))
                FormatDictionaryAsJson(writer, value, depth);
            else if (value is IEnumerable enumerable)
                FormatSequenceAsJson(writer, enumerable, depth);
            else if (HasPublicProperties(valueType))
                FormatObjectPropertiesAsJson(writer, value, depth);
            else FormatValueAsJson(writer, value);
        }

        private static void FormatDictionaryAsJson(TextWriter writer, object dictionary, int depth) =>
            FormatPropertiesAsJson(writer, DictionaryInspector.EnumerateSimpleDictionary(dictionary), depth);

        private static void FormatObjectPropertiesAsJson(TextWriter writer, object value, int depth) =>
            FormatPropertiesAsJson(writer, ObjectPropertiesExtractor.ExtractProperties(value), depth);

        private static void FormatSequenceAsJson(TextWriter writer, IEnumerable sequence, int depth)
        {
            writer.Write('[');

            var delimiter = string.Empty;

            foreach (var value in sequence)
            {
                writer.Write(delimiter);

                FormatAsJson(writer, value, depth + 1);

                delimiter = ", ";
            }

            writer.Write(']');
        }

        private static void FormatPropertiesAsJson(TextWriter writer, IEnumerable<(string, object)> properties, int depth)
        {
            writer.Write('{');

            var delimiter = string.Empty;

            foreach (var (name, value) in properties)
            {
                writer.Write(delimiter);
                writer.Write('\"');
                writer.Write(name);
                writer.Write("\": ");

                FormatAsJson(writer, value, depth + 1);

                delimiter = ", ";
            }

            writer.Write('}');
        }

        private static void FormatValueAsJson(TextWriter writer, object value)
        {
            writer.Write('\"');
            writer.Write(value.ToString());
            writer.Write('\"');
        }
    }
}