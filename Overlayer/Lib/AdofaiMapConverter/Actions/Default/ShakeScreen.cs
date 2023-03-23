using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class ShakeScreen : Action
    {
        public double duration = 1;
        public double strength = 100;
        public double intensity = 100;
        public Toggle fadeOut = Toggle.Enabled;
        public double angleOffset = 0;
        public string eventTag = "";
        public ShakeScreen() : base(LevelEventType.ShakeScreen) { }
        public ShakeScreen(double duration, double strength, double intensity, Toggle fadeOut, double angleOffset, string eventTag, bool active) : base(LevelEventType.ShakeScreen, active)
        {
            this.duration = duration;
            this.strength = strength;
            this.intensity = intensity;
            this.fadeOut = fadeOut;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["duration"] = duration;
            node["strength"] = strength;
            node["intensity"] = intensity;
            node["fadeOut"] = fadeOut.ToString();
            node["angleOffset"] = angleOffset;
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
