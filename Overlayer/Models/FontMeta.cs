using JSON;
using Overlayer.Core.Interfaces;

namespace Overlayer.Models
{
    public class FontMeta : IModel, ICopyable<FontMeta>
    {
        public string name;
        public float lineSpacing = 1;
        public float fontScale = 0.5f;
        public JsonNode Serialize()
        {
            var node = JsonNode.Empty;
            node[nameof(name)] = name;
            node[nameof(lineSpacing)] = lineSpacing;
            node[nameof(fontScale)] = fontScale;
            return node;
        }
        public void Deserialize(JsonNode node)
        {
            name = node[nameof(name)];
            lineSpacing = node[nameof(lineSpacing)];
            fontScale = node[nameof(fontScale)];
        }
        public FontMeta Copy()
        {
            var newMeta = new FontMeta();
            newMeta.name = name;
            newMeta.lineSpacing = lineSpacing;
            newMeta.fontScale = fontScale;
            return newMeta;
        }
    }
}
