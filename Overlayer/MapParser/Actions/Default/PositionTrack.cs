using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class PositionTrack : Action
    {
        public Vector2 positionOffset = Vector2.Zero;
        public (int, TileRelativeTo) relativeTo = (0, TileRelativeTo.ThisTile);
        public double rotation = 0;
        public double scale = 100;
        public int opacity = 100;
        public Toggle justThisTile = Toggle.Disabled;
        public Toggle editorOnly = Toggle.Disabled;
        public PositionTrack() : base(LevelEventType.PositionTrack) { }
        public PositionTrack(Vector2 positionOffset, (int, TileRelativeTo) relativeTo, double rotation, double scale, int opacity, Toggle justThisTile, Toggle editorOnly, bool active) : base(LevelEventType.PositionTrack, active)
        {
            this.positionOffset = positionOffset;
            this.relativeTo = relativeTo;
            this.rotation = rotation;
            this.scale = scale;
            this.opacity = opacity;
            this.justThisTile = justThisTile;
            this.editorOnly = editorOnly;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["positionOffset"] = positionOffset.ToNode();

            JsonArray newArr = new JsonArray
            {
                Inline = true
            };
            newArr[0] = relativeTo.Item1;
            newArr[1] = relativeTo.Item2.ToString();
            node["relativeTo"] = newArr;

            node["rotation"] = rotation;
            node["scale"] = scale;
            node["opacity"] = opacity;
            node["justThisTile"] = justThisTile.ToString();
            node["editorOnly"] = editorOnly.ToString();
            return node;
        }
    }
}
