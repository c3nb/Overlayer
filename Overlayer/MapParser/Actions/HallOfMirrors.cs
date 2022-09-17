using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class HallOfMirrors : Action
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
        public HallOfMirrors() : base(LevelEventType.HallOfMirrors) { }
        public HallOfMirrors(Toggle enabled, double angleOffset, string eventTag, bool active) : base(LevelEventType.HallOfMirrors, active)
        {
            this.enabled = enabled;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (enabledflag)
                node["enabled"] = _enabled.ToString();
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
