using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class Bloom : Action
    {
        public Toggle enabled = Toggle.Enabled;
        public double threshold = 50;
        public double intensity = 100;
        public string color = "ffffff";
        public double duration = 0;
        public Ease ease = Ease.Linear;
        public double angleOffset = 0;
        public string eventTag = "";
        public Bloom() : base(LevelEventType.Bloom) { }
        public Bloom(Toggle enabled, double threshold, double intensity, string color, double duration, Ease ease, double angleOffset, string eventTag, bool active) : base(LevelEventType.Bloom, active)
        {
            this.enabled = enabled;
            this.threshold = threshold;
            this.intensity = intensity;
            this.color = color;
            this.duration = duration;
            this.ease = ease;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["enabled"] = enabled.ToString();
            node["threshold"] = threshold;
            node["intensity"] = intensity;
            node["color"] = color;
            node["duration"] = duration;
            node["ease"] = ease.ToString();
            node["angleOffset"] = angleOffset;
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
