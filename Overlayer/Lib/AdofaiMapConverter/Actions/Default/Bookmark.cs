using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class Bookmark : Action
    {
        public Bookmark() : base(LevelEventType.Bookmark) { }
        public Bookmark(bool active) : base(LevelEventType.Bookmark, active)
        {
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            return node;
        }
    }
}
