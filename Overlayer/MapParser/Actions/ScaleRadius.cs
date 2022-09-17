using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class ScaleRadius : Action
    {
        public int scale
        {
            get => _scale;
            set
            {
                scaleflag = true;
                _scale = value;
            }
        }
        private int _scale = 100;
        private bool scaleflag;
        public ScaleRadius() : base(LevelEventType.ScaleRadius) { }
        public ScaleRadius(int scale, bool active) : base(LevelEventType.ScaleRadius, active)
        {
            this.scale = scale;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (scaleflag)
                node["scale"] = _scale;
            return node;
        }
    }
}
