using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class RepeatEvents : Action
    {
        public int repetitions
        {
            get => _repetitions;
            set
            {
                repetitionsflag = true;
                _repetitions = value;
            }
        }
        private int _repetitions = 1;
        private bool repetitionsflag;
        public double interval
        {
            get => _interval;
            set
            {
                intervalflag = true;
                _interval = value;
            }
        }
        private double _interval = 1;
        private bool intervalflag;
        public string tag
        {
            get => _tag;
            set
            {
                tagflag = true;
                _tag = value;
            }
        }
        private string _tag = "";
        private bool tagflag;
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
            if (repetitionsflag)
                node["repetitions"] = _repetitions;
            if (intervalflag)
                node["interval"] = _interval;
            if (tagflag)
                node["tag"] = _tag;
            return node;
        }
    }
}
