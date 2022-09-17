using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class Bloom : Action
    {
        public Toggle enabled
        {
            get => _enabled;
            set
            {
                enabledflag = true;
                _enabled = value;
            }
        }
        private Toggle _enabled = Toggle.Enabled;
        private bool enabledflag;
        public double threshold
        {
            get => _threshold;
            set
            {
                thresholdflag = true;
                _threshold = value;
            }
        }
        private double _threshold = 50;
        private bool thresholdflag;
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
        public string color
        {
            get => _color;
            set
            {
                colorflag = true;
                _color = value;
            }
        }
        private string _color = "ffffff";
        private bool colorflag;
        public double duration
        {
            get => _duration;
            set
            {
                durationflag = true;
                _duration = value;
            }
        }
        private double _duration = 0;
        private bool durationflag;
        public Ease ease
        {
            get => _ease;
            set
            {
                easeflag = true;
                _ease = value;
            }
        }
        private Ease _ease = Ease.Linear;
        private bool easeflag;
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
            if (enabledflag)
                node["enabled"] = _enabled.ToString();
            if (thresholdflag)
                node["threshold"] = _threshold;
            if (intensityflag)
                node["intensity"] = _intensity;
            if (colorflag)
                node["color"] = _color;
            if (durationflag)
                node["duration"] = _duration;
            if (easeflag)
                node["ease"] = _ease.ToString();
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
