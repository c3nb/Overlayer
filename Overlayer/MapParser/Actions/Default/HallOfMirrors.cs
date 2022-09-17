using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class HallOfMirrors : Action
    {
        public Toggle enabled = Toggle.Enabled;
        public double angleOffset = 0;
        public string eventTag = "";
        public HallOfMirrors() : base(LevelEventType.HallOfMirrors) { }
        public HallOfMirrors(Toggle enabled, double angleOffset, string eventTag, bool active) : base(LevelEventType.HallOfMirrors, active)
        {
            this.enabled = enabled;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["enabled"] = enabled.ToString();
            node["angleOffset"] = angleOffset;
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
