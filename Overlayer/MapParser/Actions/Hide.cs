using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class Hide : Action
    {
        public Toggle hideJudgment
        {
            get => _hideJudgment;
            set
            {
                hideJudgmentflag = true;
                _hideJudgment = value;
            }
        }
        private Toggle _hideJudgment = Toggle.Disabled;
        private bool hideJudgmentflag;
        public Toggle hideTileIcon
        {
            get => _hideTileIcon;
            set
            {
                hideTileIconflag = true;
                _hideTileIcon = value;
            }
        }
        private Toggle _hideTileIcon = Toggle.Disabled;
        private bool hideTileIconflag;
        public Hide() : base(LevelEventType.Hide) { }
        public Hide(Toggle hideJudgment, Toggle hideTileIcon, bool active) : base(LevelEventType.Hide, active)
        {
            this.hideJudgment = hideJudgment;
            this.hideTileIcon = hideTileIcon;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (hideJudgmentflag)
                node["hideJudgment"] = _hideJudgment.ToString();
            if (hideTileIconflag)
                node["hideTileIcon"] = _hideTileIcon.ToString();
            return node;
        }
    }
}
