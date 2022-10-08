using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Commons.Collections;
using PropertiesMutator = System.Func<Vostok.Commons.Collections.ImmutableArrayDictionary<string, object>, Vostok.Commons.Collections.ImmutableArrayDictionary<string, object>>;

// ReSharper disable once CheckNamespace
namespace Vostok.Logging.Abstractions
{
    [PublicAPI]
    public static class WithPropertyLogExtensions
    {
        /// <summary>
        /// <para>Returns a wrapper log that adds a static property with given <paramref name="key"/> and <paramref name="value"/> to each <see cref="LogEvent"/> before logging.</para>
        /// <para>By default, existing properties are not overwritten. This can be changed via <paramref name="allowOverwrite"/> parameter.</para>
        /// </summary>
        [Pure]
        public static ILog WithProperty<T>(this ILog log, string key, T value, bool allowOverwrite = false)
            => WithMutatedProperties(log, eventProperties => eventProperties.Set(key, value, allowOverwrite));

        /// <summary>
        /// <para>Returns a wrapper log that adds a dynamic property with given <paramref name="key"/> and <paramref name="value"/> provider to each <see cref="LogEvent"/> before logging.</para>
        /// <para>By default, existing properties are not overwritten. This can be changed via <paramref name="allowOverwrite"/> parameter.</para>
        /// <para>By default, <c>null</c> values are not added to events. This can be changed via <paramref name="allowNullValues"/> parameter.</para>
        /// </summary>
        [Pure]
        public static ILog WithProperty<T>(this ILog log, string key, Func<T> value, bool allowOverwrite = false, bool allowNullValues = false)
        {
            return WithMutatedProperties(log,
                eventProperties =>
                {
                    var currentValue = value();

                    if (allowNullValues || currentValue != null)
                        return eventProperties.Set(key, currentValue, allowOverwrite);

                    return eventProperties;
                });
        }

        /// <summary>
        /// <para>Returns a wrapper log that adds all of the given properties to each <see cref="LogEvent"/> before logging.</para>
        /// <para>By default, existing properties are not overwritten. This can be changed via <paramref name="allowOverwrite"/> parameter.</para>
        /// <para>By default, <c>null</c> values are not added to events. This can be changed via <paramref name="allowNullValues"/> parameter.</para>
        /// </summary>
        [Pure]
        public static ILog WithProperties(this ILog log, IReadOnlyDictionary<string, object> properties, bool allowOverwrite = false, bool allowNullValues = false)
        {
            return WithMutatedProperties(log,
                eventProperties =>
                {
                    if (properties == null || properties.Count == 0)
                        return eventProperties;

                    foreach (var pair in properties)
                    {
                        if (!allowNullValues && pair.Value == null)
                            continue;

                        eventProperties = eventProperties.Set(pair.Key, pair.Value, allowOverwrite);
                    }

                    return eventProperties;
                });
        }

        /// <summary>
        /// <para>Returns a wrapper log that adds all of the properties returned by given delegate to each <see cref="LogEvent"/> before logging.</para>
        /// <para>By default, existing properties are not overwritten. This can be changed via <paramref name="allowOverwrite"/> parameter.</para>
        /// <para>By default, <c>null</c> values are not added to events. This can be changed via <paramref name="allowNullValues"/> parameter.</para>
        /// </summary>
        [Pure]
        public static ILog WithProperties(this ILog log, Func<IEnumerable<(string, object)>> properties, bool allowOverwrite = false, bool allowNullValues = false)
        {
            if (properties == null)
                return log;
            
            return WithMutatedProperties(log,
                eventProperties =>
                {
                    var sequence = properties();
                    if (sequence == null)
                        return eventProperties;

                    foreach (var (key, value) in sequence)
                    {
                        if (!allowNullValues && value == null)
                            continue;

                        eventProperties = eventProperties.Set(key, value, allowOverwrite);
                    }

                    return eventProperties;
                });
        }

        /// <summary>
        /// <para>Returns a wrapper log that adds all properties of given <paramref name="@object"/> to each <see cref="LogEvent"/> before logging.</para>
        /// <para>By default, existing properties are not overwritten. This can be changed via <paramref name="allowOverwrite"/> parameter.</para>
        /// <para>By default, <c>null</c> values are not added to events. This can be changed via <paramref name="allowNullValues"/> parameter.</para>
        /// <para>Usage example: <c>log.WithObjectProperties(new {A = 1, B = 2})</c></para>
        /// </summary>
        [Pure]
        public static ILog WithObjectProperties<T>(this ILog log, T @object, bool allowOverwrite = false, bool allowNullValues = false)
            => WithMutatedProperties(log, eventProperties => eventProperties.WithObjectProperties(@object, allowOverwrite, allowNullValues));

        /// <summary>
        /// <para>Returns a wrapper log that adds all properties of the object returned by given <paramref name="@object"/> delegate to each <see cref="LogEvent"/> before logging.</para>
        /// <para>By default, existing properties are not overwritten. This can be changed via <paramref name="allowOverwrite"/> parameter.</para>
        /// <para>By default, <c>null</c> values are not added to events. This can be changed via <paramref name="allowNullValues"/> parameter.</para>
        /// <para>Usage example: <c>log.WithObjectProperties(() => new {A = 1, B = 2})</c></para>
        /// </summary>
        [Pure]
        public static ILog WithObjectProperties<T>(this ILog log, Func<T> @object, bool allowOverwrite = false, bool allowNullValues = false)
            => WithMutatedProperties(log, eventProperties => eventProperties.WithObjectProperties(@object(), allowOverwrite, allowNullValues));

        private static ILog WithMutatedProperties(this ILog log, PropertiesMutator mutator)
        {
            if (log is WithMutatedPropertiesLog arbitraryPropertiesLog)
                return arbitraryPropertiesLog.WithMutator(mutator);

            return new WithMutatedPropertiesLog(log, new[] { mutator });
        }

        private class WithMutatedPropertiesLog : ILog
        {
            private readonly ILog baseLog;
            private readonly IReadOnlyList<PropertiesMutator> mutators;
            private PropertiesMutator mutation;

            public WithMutatedPropertiesLog(ILog baseLog, IReadOnlyList<PropertiesMutator> mutators)
            {
                this.baseLog = baseLog ?? throw new ArgumentNullException(nameof(baseLog));
                this.mutators = mutators ?? throw new ArgumentNullException(nameof(mutators));
                mutation = Mutation;
            }

            public WithMutatedPropertiesLog WithMutator(PropertiesMutator mutator)
            {
                var newMutators = new List<PropertiesMutator>(mutators.Count + 1);
                
                newMutators.AddRange(mutators);
                newMutators.Add(mutator);

                return new WithMutatedPropertiesLog(baseLog, newMutators);
            }

            public void Log(LogEvent @event)
            {
                @event = @event?.MutateProperties(mutation);

                baseLog.Log(@event);
            }

            public bool IsEnabledFor(LogLevel level)
            {
                return baseLog.IsEnabledFor(level);
            }

            public ILog ForContext(string context)
            {
                var baseLogForContext = baseLog.ForContext(context);
                return ReferenceEquals(baseLogForContext, baseLog) ? this : new WithMutatedPropertiesLog(baseLogForContext, mutators);
            }

            private ImmutableArrayDictionary<string, object> Mutation(ImmutableArrayDictionary<string, object> properties)
            {
                for (var index = 0; index < mutators.Count; index++)
                {
                    var mutator = mutators[index];
                    properties = mutator(properties);
                }

                return properties;
            }
        }
    }
}
