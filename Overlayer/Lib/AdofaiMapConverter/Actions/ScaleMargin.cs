using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class ScaleMargin : Action
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
        public ScaleMargin() : base(LevelEventType.ScaleMargin) { }
        public ScaleMargin(int scale, bool active) : base(LevelEventType.ScaleMargin, active)
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
