using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class Flash : Action
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
        public FlashPlane plane
        {
            get => _plane;
            set
            {
                planeflag = true;
                _plane = value;
            }
        }
        private FlashPlane _plane = FlashPlane.Background;
        private bool planeflag;
        public string startColor
        {
            get => _startColor;
            set
            {
                startColorflag = true;
                _startColor = value;
            }
        }
        private string _startColor = "ffffff";
        private bool startColorflag;
        public int startOpacity
        {
            get => _startOpacity;
            set
            {
                startOpacityflag = true;
                _startOpacity = value;
            }
        }
        private int _startOpacity = 100;
        private bool startOpacityflag;
        public string endColor
        {
            get => _endColor;
            set
            {
                endColorflag = true;
                _endColor = value;
            }
        }
        private string _endColor = "ffffff";
        private bool endColorflag;
        public int endOpacity
        {
            get => _endOpacity;
            set
            {
                endOpacityflag = true;
                _endOpacity = value;
            }
        }
        private int _endOpacity = 0;
        private bool endOpacityflag;
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
            if (durationflag)
                node["duration"] = _duration;
            if (planeflag)
                node["plane"] = _plane.ToString();
            if (startColorflag)
                node["startColor"] = _startColor;
            if (startOpacityflag)
                node["startOpacity"] = _startOpacity;
            if (endColorflag)
                node["endColor"] = _endColor;
            if (endOpacityflag)
                node["endOpacity"] = _endOpacity;
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (easeflag)
                node["ease"] = _ease.ToString();
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
