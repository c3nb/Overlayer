using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class SetText : Action
    {
        public string decText
        {
            get => _decText;
            set
            {
                decTextflag = true;
                _decText = value;
            }
        }
        private string _decText = "텍스트";
        private bool decTextflag;
        public string tag
        {
            get => _tag;
            set
            {
                tagflag = true;
                _tag = value;
            }
        }
        private string _tag = "";
        private bool tagflag;
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
        public SetText() : base(LevelEventType.SetText) { }
        public SetText(string decText, string tag, double angleOffset, string eventTag, bool active) : base(LevelEventType.SetText, active)
        {
            this.decText = decText;
            this.tag = tag;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (decTextflag)
                node["decText"] = _decText;
            if (tagflag)
                node["tag"] = _tag;
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
