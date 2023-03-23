using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class Pause : Action
    {
        public double duration = 1;
        public int countdownTicks = 0;
        public double angleCorrectionDir = -1;
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
            node["duration"] = duration;
            node["countdownTicks"] = countdownTicks;
            node["angleCorrectionDir"] = angleCorrectionDir;
            return node;
        }
    }
}
