using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class ScaleMargin : Action
    {
        public int scale = 100;
        public ScaleMargin() : base(LevelEventType.ScaleMargin) { }
        public ScaleMargin(int scale, bool active) : base(LevelEventType.ScaleMargin, active)
            => this.scale = scale;
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["scale"] = scale;
            return node;
        }
    }
}
