using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class SetConditionalEvents : Action
    {
        public string perfectTag
        {
            get => _perfectTag;
            set
            {
                perfectTagflag = true;
                _perfectTag = value;
            }
        }
        private string _perfectTag = "NONE";
        private bool perfectTagflag;
        public string hitTag
        {
            get => _hitTag;
            set
            {
                hitTagflag = true;
                _hitTag = value;
            }
        }
        private string _hitTag = "NONE";
        private bool hitTagflag;
        public string barelyTag
        {
            get => _barelyTag;
            set
            {
                barelyTagflag = true;
                _barelyTag = value;
            }
        }
        private string _barelyTag = "NONE";
        private bool barelyTagflag;
        public string missTag
        {
            get => _missTag;
            set
            {
                missTagflag = true;
                _missTag = value;
            }
        }
        private string _missTag = "NONE";
        private bool missTagflag;
        public string lossTag
        {
            get => _lossTag;
            set
            {
                lossTagflag = true;
                _lossTag = value;
            }
        }
        private string _lossTag = "NONE";
        private bool lossTagflag;
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
            if (perfectTagflag)
                node["perfectTag"] = _perfectTag;
            if (hitTagflag)
                node["hitTag"] = _hitTag;
            if (barelyTagflag)
                node["barelyTag"] = _barelyTag;
            if (missTagflag)
                node["missTag"] = _missTag;
            if (lossTagflag)
                node["lossTag"] = _lossTag;
            return node;
        }
    }
}
