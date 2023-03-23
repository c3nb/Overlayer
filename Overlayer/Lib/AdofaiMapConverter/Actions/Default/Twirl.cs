using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class Twirl : Action
    {
        public Twirl() : base(LevelEventType.Twirl) { }
        public Twirl(bool active) : base(LevelEventType.Twirl, active) { }
        public override JsonNode ToNode() => InitNode(eventType, active);
    }
}
