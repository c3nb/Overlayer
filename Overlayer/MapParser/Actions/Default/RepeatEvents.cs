using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class RepeatEvents : Action
    {
        public int repetitions = 1;
        public double interval = 1;
        public string tag = "";
        public RepeatEvents() : base(LevelEventType.RepeatEvents) { }
        public RepeatEvents(int repetitions, double interval, string tag, bool active) : base(LevelEventType.RepeatEvents, active)
        {
            this.repetitions = repetitions;
            this.interval = interval;
            this.tag = tag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["repetitions"] = repetitions;
            node["interval"] = interval;
            node["tag"] = tag;
            return node;
        }
    }
}
