#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Vostok.Commons.Collections;
using Vostok.Logging.Abstractions.Helpers;

namespace Vostok.Logging.Abstractions.Values
{
    [PublicAPI]
    [InterpolatedStringHandler]
    public ref struct OperationContextStringHandler
    {
        private StringBuilder template = null!;
        private ImmutableArrayDictionary<string, object> properties = null!;

        public OperationContextStringHandler(int literalLength, int formattedCount)
        {
            template = new StringBuilder(literalLength);

            if (formattedCount > 0)
                properties = LogEvent.CreateProperties(Math.Max(4, formattedCount));
        }

        public IReadOnlyDictionary<string, object> Properties => properties;
        public string Template => template.ToString();

        public void AppendLiteral(string value)
        {
            template.Append(value);
        }

        public void AppendFormatted<T>(T value, int alignment, [CallerArgumentExpression("value")] string name = "")
        {
            var defaultHandler = CreateDefaultHandler();
            defaultHandler.AppendFormatted(value, alignment);
            AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
        }

        public void AppendFormatted<T>(T value, string format, [CallerArgumentExpression("value")] string name = "")
        {
            var defaultHandler = CreateDefaultHandler();
            defaultHandler.AppendFormatted(value, format);
            AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
        }

        public void AppendFormatted<T>(T value, int alignment, string format, [CallerArgumentExpression("value")] string name = "")
        {
            var defaultHandler = CreateDefaultHandler();
            defaultHandler.AppendFormatted(value, alignment, format);
            AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
        }

        public void AppendFormatted(object value, [CallerArgumentExpression("value")] string name = "")
        {
            name = InterpolatedHelper.EscapeName(name);

            template.Append('{');
            template.Append(name);
            template.Append('}');

            properties.SetUnsafe(name, value, true);
        }

        private static DefaultInterpolatedStringHandler CreateDefaultHandler() =>
            new DefaultInterpolatedStringHandler(0, 1, CultureInfo.InvariantCulture);
    }
}
#endif