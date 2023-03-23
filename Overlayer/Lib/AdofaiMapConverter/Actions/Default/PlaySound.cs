using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class PlaySound : Action
    {
        public HitSound hitsound = HitSound.Kick;
        public int hitsoundVolume = 100;
        public double angleOffset = 0;
        public string eventTag = "";
        public PlaySound() : base(LevelEventType.PlaySound) { }
        public PlaySound(HitSound hitsound, int hitsoundVolume, double angleOffset, string eventTag, bool active) : base(LevelEventType.PlaySound, active)
        {
            this.hitsound = hitsound;
            this.hitsoundVolume = hitsoundVolume;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["hitsound"] = hitsound.ToString();
            node["hitsoundVolume"] = hitsoundVolume;
            node["angleOffset"] = angleOffset;
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
