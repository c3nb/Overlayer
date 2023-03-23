using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class SetHoldSound : Action
    {
        public HoldStartSound holdStartSound
        {
            get => _holdStartSound;
            set
            {
                holdStartSoundflag = true;
                _holdStartSound = value;
            }
        }
        private HoldStartSound _holdStartSound = HoldStartSound.Fuse;
        private bool holdStartSoundflag;
        public HoldLoopSound holdLoopSound
        {
            get => _holdLoopSound;
            set
            {
                holdLoopSoundflag = true;
                _holdLoopSound = value;
            }
        }
        private HoldLoopSound _holdLoopSound = HoldLoopSound.Fuse;
        private bool holdLoopSoundflag;
        public HoldEndSound holdEndSound
        {
            get => _holdEndSound;
            set
            {
                holdEndSoundflag = true;
                _holdEndSound = value;
            }
        }
        private HoldEndSound _holdEndSound = HoldEndSound.Fuse;
        private bool holdEndSoundflag;
        public HoldMidSound holdMidSound
        {
            get => _holdMidSound;
            set
            {
                holdMidSoundflag = true;
                _holdMidSound = value;
            }
        }
        private HoldMidSound _holdMidSound = HoldMidSound.Fuse;
        private bool holdMidSoundflag;
        public HoldMidSoundType holdMidSoundType
        {
            get => _holdMidSoundType;
            set
            {
                holdMidSoundTypeflag = true;
                _holdMidSoundType = value;
            }
        }
        private HoldMidSoundType _holdMidSoundType = HoldMidSoundType.Once;
        private bool holdMidSoundTypeflag;
        public double holdMidSoundDelay
        {
            get => _holdMidSoundDelay;
            set
            {
                holdMidSoundDelayflag = true;
                _holdMidSoundDelay = value;
            }
        }
        private double _holdMidSoundDelay = 0.5;
        private bool holdMidSoundDelayflag;
        public HoldMidSoundTimingRelativeTo holdMidSoundTimingRelativeTo
        {
            get => _holdMidSoundTimingRelativeTo;
            set
            {
                holdMidSoundTimingRelativeToflag = true;
                _holdMidSoundTimingRelativeTo = value;
            }
        }
        private HoldMidSoundTimingRelativeTo _holdMidSoundTimingRelativeTo = HoldMidSoundTimingRelativeTo.End;
        private bool holdMidSoundTimingRelativeToflag;
        public int holdSoundVolume
        {
            get => _holdSoundVolume;
            set
            {
                holdSoundVolumeflag = true;
                _holdSoundVolume = value;
            }
        }
        private int _holdSoundVolume = 100;
        private bool holdSoundVolumeflag;
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
            if (holdStartSoundflag)
                node["holdStartSound"] = _holdStartSound.ToString();
            if (holdLoopSoundflag)
                node["holdLoopSound"] = _holdLoopSound.ToString();
            if (holdEndSoundflag)
                node["holdEndSound"] = _holdEndSound.ToString();
            if (holdMidSoundflag)
                node["holdMidSound"] = _holdMidSound.ToString();
            if (holdMidSoundTypeflag)
                node["holdMidSoundType"] = _holdMidSoundType.ToString();
            if (holdMidSoundDelayflag)
                node["holdMidSoundDelay"] = _holdMidSoundDelay;
            if (holdMidSoundTimingRelativeToflag)
                node["holdMidSoundTimingRelativeTo"] = _holdMidSoundTimingRelativeTo.ToString();
            if (holdSoundVolumeflag)
                node["holdSoundVolume"] = _holdSoundVolume;
            return node;
        }
    }
}
