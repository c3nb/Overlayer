using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class Hold : Action
    {
        public int duration
        {
            get => _duration;
            set
            {
                durationflag = true;
                _duration = value;
            }
        }
        private int _duration = 0;
        private bool durationflag;
        public int distanceMultiplier
        {
            get => _distanceMultiplier;
            set
            {
                distanceMultiplierflag = true;
                _distanceMultiplier = value;
            }
        }
        private int _distanceMultiplier = 100;
        private bool distanceMultiplierflag;
        public Toggle landingAnimation
        {
            get => _landingAnimation;
            set
            {
                landingAnimationflag = true;
                _landingAnimation = value;
            }
        }
        private Toggle _landingAnimation = Toggle.Disabled;
        private bool landingAnimationflag;
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
            if (durationflag)
                node["duration"] = _duration;
            if (distanceMultiplierflag)
                node["distanceMultiplier"] = _distanceMultiplier;
            if (landingAnimationflag)
                node["landingAnimation"] = _landingAnimation.ToString();
            return node;
        }
    }
}
