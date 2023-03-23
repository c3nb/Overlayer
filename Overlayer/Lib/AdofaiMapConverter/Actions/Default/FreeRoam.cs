using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class FreeRoam : Action
    {
        public double duration = 16;
        public Vector2 size = new Vector2(4, 4);
        public Vector2 positionOffset = Vector2.Zero;
        public double outTime = 4;
        public Ease outEase = Ease.InOutSine;
        public HitSound hitsoundOnBeats = HitSound.None;
        public HitSound hitsoundOffBeats = HitSound.None;
        public int countdownTicks = 4;
        public double angleCorrectionDir = -1;
        public FreeRoam() : base(LevelEventType.FreeRoam) { }
        public FreeRoam(double duration, Vector2 size, Vector2 positionOffset, double outTime, Ease outEase, HitSound hitsoundOnBeats, HitSound hitsoundOffBeats, int countdownTicks, double angleCorrectionDir, bool active) : base(LevelEventType.FreeRoam, active)
        {
            this.duration = duration;
            this.size = size;
            this.positionOffset = positionOffset;
            this.outTime = outTime;
            this.outEase = outEase;
            this.hitsoundOnBeats = hitsoundOnBeats;
            this.hitsoundOffBeats = hitsoundOffBeats;
            this.countdownTicks = countdownTicks;
            this.angleCorrectionDir = angleCorrectionDir;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["duration"] = duration;
            node["size"] = size.ToNode();
            node["positionOffset"] = positionOffset.ToNode();
            node["outTime"] = outTime;
            node["outEase"] = outEase.ToString();
            node["hitsoundOnBeats"] = hitsoundOnBeats.ToString();
            node["hitsoundOffBeats"] = hitsoundOffBeats.ToString();
            node["countdownTicks"] = countdownTicks;
            node["angleCorrectionDir"] = angleCorrectionDir;
            return node;
        }
    }
}
