using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class ShakeScreen : Action
    {
        public double duration
        {
            get => _duration;
            set
            {
                durationflag = true;
                _duration = value;
            }
        }
        private double _duration = 1;
        private bool durationflag;
        public double strength
        {
            get => _strength;
            set
            {
                strengthflag = true;
                _strength = value;
            }
        }
        private double _strength = 100;
        private bool strengthflag;
        public double intensity
        {
            get => _intensity;
            set
            {
                intensityflag = true;
                _intensity = value;
            }
        }
        private double _intensity = 100;
        private bool intensityflag;
        public Toggle fadeOut
        {
            get => _fadeOut;
            set
            {
                fadeOutflag = true;
                _fadeOut = value;
            }
        }
        private Toggle _fadeOut = Toggle.Enabled;
        private bool fadeOutflag;
        public double angleOffset
        {
            get => _angleOffset;
            set
            {
                angleOffsetflag = true;
                _angleOffset = value;
            }
        }
        private double _angleOffset = 0;
        private bool angleOffsetflag;
        public string eventTag
        {
            get => _eventTag;
            set
            {
                eventTagflag = true;
                _eventTag = value;
            }
        }
        private string _eventTag = "";
        private bool eventTagflag;
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
            if (durationflag)
                node["duration"] = _duration;
            if (strengthflag)
                node["strength"] = _strength;
            if (intensityflag)
                node["intensity"] = _intensity;
            if (fadeOutflag)
                node["fadeOut"] = _fadeOut.ToString();
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
