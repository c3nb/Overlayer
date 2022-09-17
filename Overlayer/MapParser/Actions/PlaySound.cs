using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class PlaySound : Action
    {
        public HitSound hitsound
        {
            get => _hitsound;
            set
            {
                hitsoundflag = true;
                _hitsound = value;
            }
        }
        private HitSound _hitsound = HitSound.Kick;
        private bool hitsoundflag;
        public int hitsoundVolume
        {
            get => _hitsoundVolume;
            set
            {
                hitsoundVolumeflag = true;
                _hitsoundVolume = value;
            }
        }
        private int _hitsoundVolume = 100;
        private bool hitsoundVolumeflag;
        public double angleOffset
        {
            get => _angleOffset;
            set
            {
                angleOffsetflag = true;
                _angleOffset = value;
            }
        }
        private double _angleOffset = 0;
        private bool angleOffsetflag;
        public string eventTag
        {
            get => _eventTag;
            set
            {
                eventTagflag = true;
                _eventTag = value;
            }
        }
        private string _eventTag = "";
        private bool eventTagflag;
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
            if (hitsoundflag)
                node["hitsound"] = _hitsound.ToString();
            if (hitsoundVolumeflag)
                node["hitsoundVolume"] = _hitsoundVolume;
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
