using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class Flash : Action
    {
        public double duration = 1;
        public FlashPlane plane = FlashPlane.Background;
        public string startColor = "ffffff";
        public int startOpacity = 100;
        public string endColor = "ffffff";
        public int endOpacity = 0;
        public double angleOffset = 0;
        public Ease ease = Ease.Linear;
        public string eventTag = "";
        public Flash() : base(LevelEventType.Flash) { }
        public Flash(double duration, FlashPlane plane, string startColor, int startOpacity, string endColor, int endOpacity, double angleOffset, Ease ease, string eventTag, bool active) : base(LevelEventType.Flash, active)
        {
            this.duration = duration;
            this.plane = plane;
            this.startColor = startColor;
            this.startOpacity = startOpacity;
            this.endColor = endColor;
            this.endOpacity = endOpacity;
            this.angleOffset = angleOffset;
            this.ease = ease;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["duration"] = duration;
            node["plane"] = plane.ToString();
            node["startColor"] = startColor;
            node["startOpacity"] = startOpacity;
            node["endColor"] = endColor;
            node["endOpacity"] = endOpacity;
            node["angleOffset"] = angleOffset;
            node["ease"] = ease.ToString();
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
