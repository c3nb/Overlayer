using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class Checkpoint : Action
    {
        public int tileOffset = 0;
        public Checkpoint() : base(LevelEventType.Checkpoint) { }
        public Checkpoint(int tileOffset, bool active) : base(LevelEventType.Checkpoint, active)
            => this.tileOffset = tileOffset;
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["tileOffset"] = tileOffset;
            return node;
        }
    }
}
