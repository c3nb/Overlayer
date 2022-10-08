using System;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Vostok.Commons.Collections;
using Vostok.Logging.Abstractions.Helpers;
using Vostok.Logging.Abstractions.Values;

namespace Vostok.Logging.Abstractions
{
    [PublicAPI]
    public static class LogEventExtensions
    {
        private static readonly SetToProperties Set = SetImplementation;

        private static readonly SetToProperties SetUnsafe = SetUnsafeImplementation;

        private static readonly Func<string, bool> IsPositionalNameCachedFunction = IsPositionalName;

        [Pure]
        public static LogEvent WithObjectProperties<T>(this LogEvent @event, T @object, bool allowOverwrite = true, bool allowNullValues = true)
            => @event.MutateProperties(eventProperties => eventProperties.WithObjectProperties(@object, allowOverwrite, allowNullValues));

        [Pure]
        public static LogEvent WithParameters(this LogEvent @event, object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return @event;

            return @event.MutateProperties(
                properties => FillImmutableArrayDictionaryWithProperties(@event.MessageTemplate, parameters, properties, Set));
        }

        [Pure]
        internal static ImmutableArrayDictionary<string, object> GenerateInitialParameters(string messageTemplate, object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return null;

            var properties = LogEvent.CreateProperties(Math.Max(4, parameters.Length));

            return FillImmutableArrayDictionaryWithProperties(messageTemplate, parameters, properties, SetUnsafe);
        }

        internal static bool HasMatchingSourceContexts(this LogEvent @event, string[] contexts)
        {
            if (contexts.Length == 0)
                return false;

            if (@event?.Properties == null)
                return false;

            if (!@event.Properties.TryGetValue(WellKnownProperties.SourceContext, out var sourceContextValue))
                return false;

            if (!(sourceContextValue is SourceContextValue sourceContext))
                return false;

            return contexts.All(
                context =>
                    sourceContext.Any(value => value.StartsWith(context, StringComparison.OrdinalIgnoreCase)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableArrayDictionary<string, object> SetImplementation(ImmutableArrayDictionary<string, object> properties, string key, object value)
        {
            return properties.Set(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableArrayDictionary<string, object> SetUnsafeImplementation(ImmutableArrayDictionary<string, object> properties, string key, object value)
        {
            properties.SetUnsafe(key, value, true);
            return properties;
        }

        private static ImmutableArrayDictionary<string, object> FillImmutableArrayDictionaryWithProperties(string messageTemplate, object[] parameters, ImmutableArrayDictionary<string, object> properties, SetToProperties set)
        {
            var templatePropertyNames = TemplatePropertiesExtractor.ExtractPropertyNames(messageTemplate);

            if (ShouldInferNamesForPositionalParameters(templatePropertyNames))
            {
                // (iloktionov): Name positional parameters with corresponding placeholder names from template:
                for (var i = 0; i < Math.Min(parameters.Length, templatePropertyNames.Length); i++)
                    properties = set(properties, templatePropertyNames[i], parameters[i]);

                if (parameters.Length > templatePropertyNames.Length)
                    for (var i = templatePropertyNames.Length; i < parameters.Length; i++)
                        properties = set(properties, i.ToString(), parameters[i]);
            }
            else
            {
                // (iloktionov): Name positional parameters with their indices:
                for (var i = 0; i < parameters.Length; i++)
                    properties = set(properties, i.ToString(), parameters[i]);
            }

            return properties;
        }

        private static bool ShouldInferNamesForPositionalParameters(string[] propertyNames)
        {
            if (propertyNames.Length == 0)
                return false;

            if (propertyNames.All(IsPositionalNameCachedFunction))
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPositionalName(string propertyName)
        {
            foreach (var character in propertyName)
            {
                if (character < '0')
                    return false;

                if (character > '9')
                    return false;
            }

            return true;
        }

        private delegate ImmutableArrayDictionary<string, object> SetToProperties(ImmutableArrayDictionary<string, object> properties, string key, object value);
    }
}