using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class SetHoldSound : Action
    {
        public HoldStartSound holdStartSound = HoldStartSound.Fuse;
        public HoldLoopSound holdLoopSound = HoldLoopSound.Fuse;
        public HoldEndSound holdEndSound = HoldEndSound.Fuse;
        public HoldMidSound holdMidSound = HoldMidSound.Fuse;
        public HoldMidSoundType holdMidSoundType = HoldMidSoundType.Once;
        public double holdMidSoundDelay = 0.5;
        public HoldMidSoundTimingRelativeTo holdMidSoundTimingRelativeTo = HoldMidSoundTimingRelativeTo.End;
        public int holdSoundVolume = 100;
        public SetHoldSound() : base(LevelEventType.SetHoldSound) { }
        public SetHoldSound(HoldStartSound holdStartSound, HoldLoopSound holdLoopSound, HoldEndSound holdEndSound, HoldMidSound holdMidSound, HoldMidSoundType holdMidSoundType, double holdMidSoundDelay, HoldMidSoundTimingRelativeTo holdMidSoundTimingRelativeTo, int holdSoundVolume, bool active) : base(LevelEventType.SetHoldSound, active)
        {
            this.holdStartSound = holdStartSound;
            this.holdLoopSound = holdLoopSound;
            this.holdEndSound = holdEndSound;
            this.holdMidSound = holdMidSound;
            this.holdMidSoundType = holdMidSoundType;
            this.holdMidSoundDelay = holdMidSoundDelay;
            this.holdMidSoundTimingRelativeTo = holdMidSoundTimingRelativeTo;
            this.holdSoundVolume = holdSoundVolume;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["holdStartSound"] = holdStartSound.ToString();
            node["holdLoopSound"] = holdLoopSound.ToString();
            node["holdEndSound"] = holdEndSound.ToString();
            node["holdMidSound"] = holdMidSound.ToString();
            node["holdMidSoundType"] = holdMidSoundType.ToString();
            node["holdMidSoundDelay"] = holdMidSoundDelay;
            node["holdMidSoundTimingRelativeTo"] = holdMidSoundTimingRelativeTo.ToString();
            node["holdSoundVolume"] = holdSoundVolume;
            return node;
        }
    }
}
