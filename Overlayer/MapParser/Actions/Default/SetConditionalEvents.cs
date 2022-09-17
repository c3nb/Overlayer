using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class SetConditionalEvents : Action
    {
        public string perfectTag = "NONE";
        public string hitTag = "NONE";
        public string barelyTag = "NONE";
        public string missTag = "NONE";
        public string lossTag = "NONE";
        public SetConditionalEvents() : base(LevelEventType.SetConditionalEvents) { }
        public SetConditionalEvents(string perfectTag, string hitTag, string barelyTag, string missTag, string lossTag, bool active) : base(LevelEventType.SetConditionalEvents, active)
        {
            this.perfectTag = perfectTag;
            this.hitTag = hitTag;
            this.barelyTag = barelyTag;
            this.missTag = missTag;
            this.lossTag = lossTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["perfectTag"] = perfectTag;
            node["hitTag"] = hitTag;
            node["barelyTag"] = barelyTag;
            node["missTag"] = missTag;
            node["lossTag"] = lossTag;
            return node;
        }
    }
}
