using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON;

namespace AdofaiMapConverter.Helpers
{
    public static class JsonHelper
    {
        public const char BOM = '\uFEFF';
        public static string TrimLR(this string str)
            => str.Substring(1, Math.Max(0, str.Length - 2));
        public static JsonNode Else(this JsonNode node, JsonNode elseNode)
            => node is JsonLazyCreator ? elseNode : node;
        public static JsonNode ElseEmpty(this JsonNode node)
            => node.Else(string.Empty);
    }
}
