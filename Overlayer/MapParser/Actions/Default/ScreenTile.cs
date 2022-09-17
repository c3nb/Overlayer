using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class ScreenTile : Action
    {
        public Vector2 tile = Vector2.One;
        public double angleOffset = 0;
        public string eventTag = "";
        public ScreenTile() : base(LevelEventType.ScreenTile) { }
        public ScreenTile(Vector2 tile, double angleOffset, string eventTag, bool active) : base(LevelEventType.ScreenTile, active)
        {
            this.tile = tile;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["tile"] = tile.ToNode();
            node["angleOffset"] = angleOffset;
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
