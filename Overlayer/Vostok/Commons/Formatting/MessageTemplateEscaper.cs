using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace Vostok.Commons.Formatting
{
    /// <summary>
    /// Escapes given template according to https://vostok.gitbook.io/logging/concepts/syntax/message-templates
    /// </summary>
    [PublicAPI]
    internal class MessageTemplateEscaper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Escape(string template) =>
            template.Contains("{") || template.Contains("}") 
                ? EscapeInner(template) 
                : template;

        private static string EscapeInner(string template)
        {
            var stringBuilder = new StringBuilder();

            foreach (var chr in template)
            {
                if (chr == '{' || chr == '}')
                    stringBuilder.Append(chr);
                stringBuilder.Append(chr);
            }

            return stringBuilder.ToString();
        }
    }
}