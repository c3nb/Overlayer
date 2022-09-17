using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class EditorComment : Action
    {
        public string comment = "주석 내용을 여기에 적을 수 있습니다.  줄바꿈과 <color=#00FF00>색상</color>을 지원합니다.";
        public EditorComment(string comment, bool active) : base(LevelEventType.EditorComment, active)
            => this.comment = comment;
        public EditorComment() : base(LevelEventType.EditorComment) { }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["comment"] = comment;
            return node;
        }
    }
}
