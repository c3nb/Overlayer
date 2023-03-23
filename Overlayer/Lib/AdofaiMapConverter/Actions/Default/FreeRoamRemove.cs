using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class FreeRoamRemove : Action
    {
        public Vector2 position = new Vector2(1, 0);
        public Vector2 size = Vector2.One;
        public FreeRoamRemove() : base(LevelEventType.FreeRoamRemove) { }
        public FreeRoamRemove(Vector2 position, Vector2 size, bool active) : base(LevelEventType.FreeRoamRemove, active)
        {
            this.position = position;
            this.size = size;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["position"] = position.ToNode();
            node["size"] = size.ToNode();
            return node;
        }
    }
}
