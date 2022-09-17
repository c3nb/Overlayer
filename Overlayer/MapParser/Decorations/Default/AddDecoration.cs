using Overlayer.MapParser.Types;
using Overlayer.MapParser.Decorations;
using JSON;
using static Overlayer.MapParser.Decorations.DecorationUtils;

namespace Overlayer.MapParser.Decorations.Default
{
    public class AddDecoration : Decoration
    {
        public string decorationImage = "";
        public Vector2 position = Vector2.Zero;
        public DecPlacementType relativeTo = DecPlacementType.Tile;
        public Vector2 pivotOffset = Vector2.Zero;
        public double rotation = 0;
        public Vector2 scale = Vector2.Hrd;
        public Vector2 tile = Vector2.One;
        public string color = "ffffff";
        public int opacity = 100;
        public double depth = 0;
        public Vector2 parallax = Vector2.Zero;
        public string tag = "";
        public Toggle imageSmoothing = Toggle.Enabled;
        public Toggle failHitbox = Toggle.Disabled;
        public Hitbox failHitboxType = Hitbox.Box;
        public Vector2 failHitboxScale = Vector2.Hrd;
        public Vector2 failHitboxOffset = Vector2.Zero;
        public double failHitboxRotation = 0;
        public string components = "";
        public AddDecoration() : base(LevelEventType.AddDecoration, true) { }
        public AddDecoration(string decorationImage, Vector2 position, DecPlacementType relativeTo, Vector2 pivotOffset, double rotation, Vector2 scale, Vector2 tile, string color, int opacity, double depth, Vector2 parallax, string tag, Toggle imageSmoothing, Toggle failHitbox, Hitbox failHitboxType, Vector2 failHitboxScale, Vector2 failHitboxOffset, double failHitboxRotation, string components, bool visible) : base(LevelEventType.AddDecoration, visible)
        {
            this.decorationImage = decorationImage;
            this.position = position;
            this.relativeTo = relativeTo;
            this.pivotOffset = pivotOffset;
            this.rotation = rotation;
            this.scale = scale;
            this.tile = tile;
            this.color = color;
            this.opacity = opacity;
            this.depth = depth;
            this.parallax = parallax;
            this.tag = tag;
            this.imageSmoothing = imageSmoothing;
            this.failHitbox = failHitbox;
            this.failHitboxType = failHitboxType;
            this.failHitboxScale = failHitboxScale;
            this.failHitboxOffset = failHitboxOffset;
            this.failHitboxRotation = failHitboxRotation;
            this.components = components;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, floor, active);
            node["decorationImage"] = decorationImage;
            node["position"] = position.ToNode();
            node["relativeTo"] = relativeTo.ToString();
            node["pivotOffset"] = pivotOffset.ToNode();
            node["rotation"] = rotation;
            node["scale"] = scale.ToNode();
            node["tile"] = tile.ToNode();
            node["color"] = color;
            node["opacity"] = opacity;
            node["depth"] = depth;
            node["parallax"] = parallax.ToNode();
            node["tag"] = tag;
            node["imageSmoothing"] = imageSmoothing.ToString();
            node["failHitbox"] = failHitbox.ToString();
            node["failHitboxType"] = failHitboxType.ToString();
            node["failHitboxScale"] = failHitboxScale.ToNode();
            node["failHitboxOffset"] = failHitboxOffset.ToNode();
            node["failHitboxRotation"] = failHitboxRotation;
            node["components"] = components;
            return node;
        }
    }
}
