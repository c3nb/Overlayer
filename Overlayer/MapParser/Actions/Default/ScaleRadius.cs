using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class ScaleRadius : Action
    {
        public int scale = 100;
        public ScaleRadius() : base(LevelEventType.ScaleRadius) { }
        public ScaleRadius(int scale, bool active) : base(LevelEventType.ScaleRadius, active)
            => this.scale = scale;
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["scale"] = scale;
            return node;
        }
    }
}
