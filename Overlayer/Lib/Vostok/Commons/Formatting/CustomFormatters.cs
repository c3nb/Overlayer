using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Formatting
{
    [PublicAPI]
    internal static class CustomFormatters
    {
        private static readonly Dictionary<Type, Func<object, string>> Formatters
            = new Dictionary<Type, Func<object, string>>
            {
                [typeof(string)] = value => (string)value,
                [typeof(Uri)] = value => value.ToString(),
                [typeof(Enum)] = value => value.ToString(),
                [typeof(IPEndPoint)] = value => value.ToString(),
                [typeof(Encoding)] = value => ((Encoding)value).WebName,
                [typeof(DateTimeOffset)] = value => ((DateTimeOffset)value).ToString("o"),
                [typeof(IEnumerable<byte>)] = value => Convert.ToBase64String(((IEnumerable<byte>)value).ToArray()) 
            };

        public static bool TryFormat(object item, out string s)
        {
            s = null;
            var itemType = item.GetType();

            foreach (var pair in Formatters)
            {
                if (pair.Key.IsAssignableFrom(itemType))
                {
                    s = pair.Value(item);
                    return true;
                }
            }

            return false;
        }
    }
}