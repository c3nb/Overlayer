using AdofaiMapConverter.Types;
using AdofaiMapConverter.Decorations;
using JSON;
using static AdofaiMapConverter.Decorations.DecorationUtils;

namespace AdofaiMapConverter.Decorations.Default
{
    public class AddText : Decoration
    {
        public string decText = "텍스트";
        public FontName font = FontName.Default;
        public Vector2 position = Vector2.Zero;
        public DecPlacementType relativeTo = DecPlacementType.Tile;
        public Vector2 pivotOffset = Vector2.Zero;
        public double rotation = 0;
        public Vector2 scale = Vector2.Hrd;
        public string color = "ffffff";
        public int opacity = 100;
        public double depth = -1;
        public Vector2 parallax = Vector2.MOne;
        public string tag = "";
        public AddText() : base(LevelEventType.AddText, true) { }
        public AddText(string decText, FontName font, Vector2 position, DecPlacementType relativeTo, Vector2 pivotOffset, double rotation, Vector2 scale, string color, int opacity, double depth, Vector2 parallax, string tag, bool visible) : base(LevelEventType.AddText, visible)
        {
            this.decText = decText;
            this.font = font;
            this.position = position;
            this.relativeTo = relativeTo;
            this.pivotOffset = pivotOffset;
            this.rotation = rotation;
            this.scale = scale;
            this.color = color;
            this.opacity = opacity;
            this.depth = depth;
            this.parallax = parallax;
            this.tag = tag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, floor, active);
            node["decText"] = decText;
            node["font"] = font.ToString();
            node["position"] = position.ToNode();
            node["relativeTo"] = relativeTo.ToString();
            node["pivotOffset"] = pivotOffset.ToNode();
            node["rotation"] = rotation;
            node["scale"] = scale.ToNode();
            node["color"] = color;
            node["opacity"] = opacity;
            node["depth"] = depth;
            node["parallax"] = parallax.ToNode();
            node["tag"] = tag;
            return node;
        }
    }
}
