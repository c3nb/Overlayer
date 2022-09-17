using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class ScreenScroll : Action
    {
        public Vector2 scroll = Vector2.Zero;
        public double angleOffset = 0;
        public string eventTag = "";
        public ScreenScroll() : base(LevelEventType.ScreenScroll) { }
        public ScreenScroll(Vector2 scroll, double angleOffset, string eventTag, bool active) : base(LevelEventType.ScreenScroll, active)
        {
            this.scroll = scroll;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["scroll"] = scroll.ToNode();
            node["angleOffset"] = angleOffset;
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
