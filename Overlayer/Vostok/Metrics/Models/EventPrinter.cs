using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Metrics.Models
{
    internal static class EventPrinter
    {
        private const string OpeningCurlyBracket = "{";
        private const string ClosingCurlyBracket = "}";

        private const char Quote = '"';
        private const char Comma = ',';
        private const char Colon = ':';
        private const char Space = ' ';
        private const char Equal = '=';

        [NotNull]
        public static string Print([NotNull] MetricEvent @event, MetricEventPrintFormat format)
        {
            var builder = new StringBuilder();

            switch (format)
            {
                case MetricEventPrintFormat.Json:
                    PrintJson(@event, builder);
                    break;

                case MetricEventPrintFormat.Flat:
                    PrintFlat(@event, builder);
                    break;
            }
          
            return builder.ToString();
        }

        [NotNull]
        public static string Print([NotNull] AnnotationEvent @event)
        {
            var builder = new StringBuilder();

            PrintJson(@event, builder);

            return builder.ToString();
        }

        private static void PrintFlat([NotNull] MetricEvent @event, [NotNull] StringBuilder builder)
        {
            builder
                .Append(string.Join(".", @event.Tags.Select(tag => tag.Value)))
                .Append(Space)
                .Append(Equal)
                .Append(Space)
                .Append(@event.Value.ToString("0.000", CultureInfo.InvariantCulture));

            if (!string.IsNullOrEmpty(@event.Unit))
                builder.Append(Space).Append(@event.Unit);
        }

        private static void PrintJson([NotNull] MetricEvent @event, [NotNull] StringBuilder builder)
        {
            builder.AppendLine(OpeningCurlyBracket);

            PrintProperty(builder, nameof(MetricEvent.Value), @event.Value.ToString("0.000", CultureInfo.InvariantCulture), 1, false);

            if (!string.IsNullOrEmpty(@event.Unit))
                PrintProperty(builder, nameof(MetricEvent.Unit), @event.Unit, 1);

            PrintProperty(builder, nameof(MetricEvent.Timestamp), @event.Timestamp.ToString("O", CultureInfo.InvariantCulture), 1);
            PrintObject(builder, nameof(MetricEvent.Tags), @event.Tags.Select(tag => (tag.Key, tag.Value)));

            if (!string.IsNullOrEmpty(@event.AggregationType))
                PrintProperty(builder, nameof(MetricEvent.AggregationType), @event.AggregationType, 1);

            if (@event.AggregationParameters != null)
                PrintObject(builder, nameof(MetricEvent.AggregationParameters), @event.AggregationParameters.Select(pair => (pair.Key, pair.Value)));

            builder.RemoveCommaBeforeNewline();
            builder.AppendLine(ClosingCurlyBracket);
        }

        private static void PrintJson([NotNull] AnnotationEvent @event, [NotNull] StringBuilder builder)
        {
            builder.AppendLine(OpeningCurlyBracket);

            PrintProperty(builder, nameof(AnnotationEvent.Timestamp), @event.Timestamp.ToString("O", CultureInfo.InvariantCulture), 1);
            
            PrintProperty(builder, nameof(AnnotationEvent.Description), @event.Timestamp.ToString("O", CultureInfo.InvariantCulture), 1);
            
            PrintObject(builder, nameof(AnnotationEvent.Tags), @event.Tags.Select(tag => (tag.Key, tag.Value)));

            builder.RemoveCommaBeforeNewline();
            builder.AppendLine(ClosingCurlyBracket);
        }

        private static void PrintProperty(StringBuilder builder, string name, string value, int depth, bool quoteValue = true)
            => builder
                .Indent(depth)
                .PrintPropertyName(name)
                .PrintPropertyValue(value, quoteValue);

        private static void PrintObject(StringBuilder builder, string name, IEnumerable<(string key, string value)> properties)
        {
            builder
                .Indent(1)
                .PrintPropertyName(name)
                .Append(OpeningCurlyBracket)
                .AppendLine();

            foreach (var (key, value) in properties)
                builder
                    .Indent(2)
                    .PrintPropertyName(key)
                    .PrintPropertyValue(value);

            builder
                .RemoveCommaBeforeNewline()
                .Indent(1)
                .Append(ClosingCurlyBracket)
                .Append(Comma)
                .AppendLine();
        }

        private static StringBuilder PrintPropertyName(this StringBuilder builder, string name)
            => builder
                .Append(Quote)
                .Append(name)
                .Append(Quote)
                .Append(Colon)
                .Append(Space);

        private static void PrintPropertyValue(this StringBuilder builder, string value, bool quote = true)
        {
            if (quote)
                builder.Append(Quote);

            builder.Append(value);

            if (quote)
                builder.Append(Quote);

            builder
                .Append(Comma)
                .AppendLine();
        }

        private static StringBuilder RemoveCommaBeforeNewline(this StringBuilder builder)
        {
            var newlineLength = Environment.NewLine.Length;
            if (builder[builder.Length - newlineLength - 1] == Comma)
                builder.Remove(builder.Length - newlineLength - 1, 1);

            return builder;
        }

        private static StringBuilder Indent(this StringBuilder builder, int depth)
            => builder.Append(Space, depth * 3);
    }
}