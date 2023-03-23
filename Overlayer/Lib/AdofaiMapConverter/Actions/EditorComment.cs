using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class EditorComment : Action
    {
        public string comment
        {
            get => _comment;
            set
            {
                commentflag = true;
                _comment = value;
            }
        }
        private string _comment = "주석 내용을 여기에 적을 수 있습니다.  줄바꿈과 <color=#00FF00>색상</color>을 지원합니다.";
        private bool commentflag;
        public EditorComment() : base(LevelEventType.EditorComment) { }
        public EditorComment(string comment, bool active) : base(LevelEventType.EditorComment, active)
        {
            this.comment = comment;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (commentflag)
                node["comment"] = _comment;
            return node;
        }
    }
}
