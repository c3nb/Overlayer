using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class Hold : Action
    {
        public int duration = 0;
        public int distanceMultiplier = 100;
        public Toggle landingAnimation = Toggle.Disabled;
        public Hold() : base(LevelEventType.Hold) { }
        public Hold(int duration, int distanceMultiplier, Toggle landingAnimation, bool active) : base(LevelEventType.Hold, active)
        {
            this.duration = duration;
            this.distanceMultiplier = distanceMultiplier;
            this.landingAnimation = landingAnimation;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["duration"] = duration;
            node["distanceMultiplier"] = distanceMultiplier;
            node["landingAnimation"] = landingAnimation.ToString();
            return node;
        }
    }
}
