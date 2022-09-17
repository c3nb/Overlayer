using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class ScreenScroll : Action
    {
        public Vector2 scroll
        {
            get => _scroll;
            set
            {
                scrollflag = true;
                _scroll = value;
            }
        }
        private Vector2 _scroll = Vector2.Zero;
        private bool scrollflag;
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
        public ScreenScroll() : base(LevelEventType.ScreenScroll) { }
        public ScreenScroll(Vector2 scroll, double angleOffset, string eventTag, bool active) : base(LevelEventType.ScreenScroll, active)
        {
            this.scroll = scroll;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (scrollflag)
                node["scroll"] = _scroll.ToNode();
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
