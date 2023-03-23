using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class SetText : Action
    {
        public string decText = "텍스트";
        public string tag = "";
        public double angleOffset = 0;
        public string eventTag = "";
        public SetText() : base(LevelEventType.SetText) { }
        public SetText(string decText, string tag, double angleOffset, string eventTag, bool active) : base(LevelEventType.SetText, active)
        {
            this.decText = decText;
            this.tag = tag;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["decText"] = decText;
            node["tag"] = tag;
            node["angleOffset"] = angleOffset;
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
