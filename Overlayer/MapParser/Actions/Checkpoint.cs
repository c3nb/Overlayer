using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class Checkpoint : Action
    {
        public int tileOffset
        {
            get => _tileOffset;
            set
            {
                tileOffsetflag = true;
                _tileOffset = value;
            }
        }
        private int _tileOffset = 0;
        private bool tileOffsetflag;
        public Checkpoint() : base(LevelEventType.Checkpoint) { }
        public Checkpoint(int tileOffset, bool active) : base(LevelEventType.Checkpoint, active)
        {
            this.tileOffset = tileOffset;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (tileOffsetflag)
                node["tileOffset"] = _tileOffset;
            return node;
        }
    }
}
