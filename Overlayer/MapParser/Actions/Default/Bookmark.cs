using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
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
