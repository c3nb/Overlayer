using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class FreeRoam : Action
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
        private double _duration = 16;
        private bool durationflag;
        public Vector2 size
        {
            get => _size;
            set
            {
                sizeflag = true;
                _size = value;
            }
        }
        private Vector2 _size = new Vector2(4, 4);
        private bool sizeflag;
        public Vector2 positionOffset
        {
            get => _positionOffset;
            set
            {
                positionOffsetflag = true;
                _positionOffset = value;
            }
        }
        private Vector2 _positionOffset = Vector2.Zero;
        private bool positionOffsetflag;
        public double outTime
        {
            get => _outTime;
            set
            {
                outTimeflag = true;
                _outTime = value;
            }
        }
        private double _outTime = 4;
        private bool outTimeflag;
        public Ease outEase
        {
            get => _outEase;
            set
            {
                outEaseflag = true;
                _outEase = value;
            }
        }
        private Ease _outEase = Ease.InOutSine;
        private bool outEaseflag;
        public HitSound hitsoundOnBeats
        {
            get => _hitsoundOnBeats;
            set
            {
                hitsoundOnBeatsflag = true;
                _hitsoundOnBeats = value;
            }
        }
        private HitSound _hitsoundOnBeats = HitSound.None;
        private bool hitsoundOnBeatsflag;
        public HitSound hitsoundOffBeats
        {
            get => _hitsoundOffBeats;
            set
            {
                hitsoundOffBeatsflag = true;
                _hitsoundOffBeats = value;
            }
        }
        private HitSound _hitsoundOffBeats = HitSound.None;
        private bool hitsoundOffBeatsflag;
        public int countdownTicks
        {
            get => _countdownTicks;
            set
            {
                countdownTicksflag = true;
                _countdownTicks = value;
            }
        }
        private int _countdownTicks = 4;
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
            if (durationflag)
                node["duration"] = _duration;
            if (sizeflag)
                node["size"] = _size.ToNode();
            if (positionOffsetflag)
                node["positionOffset"] = _positionOffset.ToNode();
            if (outTimeflag)
                node["outTime"] = _outTime;
            if (outEaseflag)
                node["outEase"] = _outEase.ToString();
            if (hitsoundOnBeatsflag)
                node["hitsoundOnBeats"] = _hitsoundOnBeats.ToString();
            if (hitsoundOffBeatsflag)
                node["hitsoundOffBeats"] = _hitsoundOffBeats.ToString();
            if (countdownTicksflag)
                node["countdownTicks"] = _countdownTicks;
            if (angleCorrectionDirflag)
                node["angleCorrectionDir"] = _angleCorrectionDir;
            return node;
        }
    }
}
