using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Url
{
    [PublicAPI]
    internal static class UrlParser
    {
        public static Uri[] Parse(IEnumerable<string> urls)
        {
            return urls?.Select(Parse).Where(u => u != null).ToArray();
        }

        public static Uri Parse(string url)
        {
            var uri = !Uri.TryCreate(url, UriKind.Absolute, out var parsed) ? null : parsed;
            if (uri == null || string.IsNullOrEmpty(uri.Host) || uri.Port == -1)
                return null;
            return uri;
        }
    }
}