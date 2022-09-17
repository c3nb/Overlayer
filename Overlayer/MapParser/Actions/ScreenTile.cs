using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class ScreenTile : Action
    {
        public Vector2 tile
        {
            get => _tile;
            set
            {
                tileflag = true;
                _tile = value;
            }
        }
        private Vector2 _tile = Vector2.One;
        private bool tileflag;
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
        public ScreenTile() : base(LevelEventType.ScreenTile) { }
        public ScreenTile(Vector2 tile, double angleOffset, string eventTag, bool active) : base(LevelEventType.ScreenTile, active)
        {
            this.tile = tile;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (tileflag)
                node["tile"] = _tile.ToNode();
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
