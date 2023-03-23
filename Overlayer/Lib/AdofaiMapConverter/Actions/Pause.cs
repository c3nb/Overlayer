using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class Pause : Action
    {
        public double duration
        {
            get => _duration;
            set
            {
                durationflag = true;
                _duration = value;
            }
        }
        private double _duration = 1;
        private bool durationflag;
        public int countdownTicks
        {
            get => _countdownTicks;
            set
            {
                countdownTicksflag = true;
                _countdownTicks = value;
            }
        }
        private int _countdownTicks = 0;
        private bool countdownTicksflag;
        public double angleCorrectionDir
        {
            get => _angleCorrectionDir;
            set
            {
                angleCorrectionDirflag = true;
                _angleCorrectionDir = value;
            }
        }
        private double _angleCorrectionDir = -1;
        private bool angleCorrectionDirflag;
        public Pause() : base(LevelEventType.Pause) { }
        public Pause(double duration, int countdownTicks, double angleCorrectionDir, bool active) : base(LevelEventType.Pause, active)
        {
            this.duration = duration;
            this.countdownTicks = countdownTicks;
            this.angleCorrectionDir = angleCorrectionDir;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (durationflag)
                node["duration"] = _duration;
            if (countdownTicksflag)
                node["countdownTicks"] = _countdownTicks;
            if (angleCorrectionDirflag)
                node["angleCorrectionDir"] = _angleCorrectionDir;
            return node;
        }
    }
}
