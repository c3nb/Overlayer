using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class SetSpeed : Action
    {
        public SpeedType speedType
        {
            get => _speedType;
            set
            {
                speedTypeflag = true;
                _speedType = value;
            }
        }
        private SpeedType _speedType = SpeedType.Bpm;
        private bool speedTypeflag;
        public double beatsPerMinute
        {
            get => _beatsPerMinute;
            set
            {
                beatsPerMinuteflag = true;
                _beatsPerMinute = value;
            }
        }
        private double _beatsPerMinute = 100;
        private bool beatsPerMinuteflag;
        public double bpmMultiplier
        {
            get => _bpmMultiplier;
            set
            {
                bpmMultiplierflag = true;
                _bpmMultiplier = value;
            }
        }
        private double _bpmMultiplier = 1;
        private bool bpmMultiplierflag;
        public SetSpeed() : base(LevelEventType.SetSpeed) { }
        public SetSpeed(SpeedType speedType, double beatsPerMinute, double bpmMultiplier, bool active) : base(LevelEventType.SetSpeed, active)
        {
            this.speedType = speedType;
            this.beatsPerMinute = beatsPerMinute;
            this.bpmMultiplier = bpmMultiplier;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (speedTypeflag)
                node["speedType"] = _speedType.ToString();
            if (beatsPerMinuteflag)
                node["beatsPerMinute"] = _beatsPerMinute;
            if (bpmMultiplierflag)
                node["bpmMultiplier"] = _bpmMultiplier;
            return node;
        }
    }
}
