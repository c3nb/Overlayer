using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class SetFilter : Action
    {
        public Filter filter = Filter.Grayscale;
        public Toggle enabled = Toggle.Enabled;
        public double intensity = 100;
        public double duration = 0;
        public Ease ease = Ease.Linear;
        public Toggle disableOthers = Toggle.Disabled;
        public double angleOffset = 0;
        public string eventTag = "";
        public SetFilter() : base(LevelEventType.SetFilter) { }
        public SetFilter(Filter filter, Toggle enabled, double intensity, double duration, Ease ease, Toggle disableOthers, double angleOffset, string eventTag, bool active) : base(LevelEventType.SetFilter, active)
        {
            this.filter = filter;
            this.enabled = enabled;
            this.intensity = intensity;
            this.duration = duration;
            this.ease = ease;
            this.disableOthers = disableOthers;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["filter"] = filter.ToString();
            node["enabled"] = enabled.ToString();
            node["intensity"] = intensity;
            node["duration"] = duration;
            node["ease"] = ease.ToString();
            node["disableOthers"] = disableOthers.ToString();
            node["angleOffset"] = angleOffset;
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
