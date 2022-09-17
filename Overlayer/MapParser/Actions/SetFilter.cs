using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class SetFilter : Action
    {
        public Filter filter
        {
            get => _filter;
            set
            {
                filterflag = true;
                _filter = value;
            }
        }
        private Filter _filter = Filter.Grayscale;
        private bool filterflag;
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
        public Toggle disableOthers
        {
            get => _disableOthers;
            set
            {
                disableOthersflag = true;
                _disableOthers = value;
            }
        }
        private Toggle _disableOthers = Toggle.Disabled;
        private bool disableOthersflag;
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
            if (filterflag)
                node["filter"] = _filter.ToString();
            if (enabledflag)
                node["enabled"] = _enabled.ToString();
            if (intensityflag)
                node["intensity"] = _intensity;
            if (durationflag)
                node["duration"] = _duration;
            if (easeflag)
                node["ease"] = _ease.ToString();
            if (disableOthersflag)
                node["disableOthers"] = _disableOthers.ToString();
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
