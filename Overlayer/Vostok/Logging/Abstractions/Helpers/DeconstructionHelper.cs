using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Commons.Formatting;

namespace Vostok.Logging.Abstractions.Helpers
{
    internal static class DeconstructionHelper
    {
        public static bool ShouldDeconstruct<T>([CanBeNull] string messageTemplate, T properties)
        {
            if (properties == null)
                return false;

            if (properties is IReadOnlyDictionary<string, object>)
                return true;

            if (properties is IEnumerable)
                return false;

            if (TypesHelper.IsAnonymousType(properties.GetType()))
                return true;

            if (ToStringDetector.HasCustomToString(properties.GetType()))
                return false;

            if (TemplatePropertiesExtractor.ExtractPropertyNames(messageTemplate).Length > 1)
                return true;

            return false;
        }
    }
}
