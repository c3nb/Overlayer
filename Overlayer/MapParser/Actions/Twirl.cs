using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class Twirl : Action
    {
        public Twirl() : base(LevelEventType.Twirl) { }
        public Twirl(bool active) : base(LevelEventType.Twirl, active)
        {
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            return node;
        }
    }
}
