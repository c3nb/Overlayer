using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class Twirl : Action
    {
        public Twirl() : base(LevelEventType.Twirl) { }
        public Twirl(bool active) : base(LevelEventType.Twirl, active) { }
        public override JsonNode ToNode() => InitNode(eventType, active);
    }
}
