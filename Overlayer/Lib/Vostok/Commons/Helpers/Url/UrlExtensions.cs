using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Url
{
    [PublicAPI]
    internal static class UrlExtensions
    {
        public static string ToStringWithoutQuery(this Uri url) =>
            ToStringWithoutQuery(url.ToString());

        public static string ToStringWithoutQuery(string urlString)
        {
            var queryBeginning = urlString.IndexOf("?", StringComparison.Ordinal);
            if (queryBeginning >= 0)
                urlString = urlString.Substring(0, queryBeginning);

            return urlString;
        }
    }
}