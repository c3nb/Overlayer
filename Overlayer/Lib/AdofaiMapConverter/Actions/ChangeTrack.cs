using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class ChangeTrack : Action
    {
        public TrackColorType trackColorType
        {
            get => _trackColorType;
            set
            {
                trackColorTypeflag = true;
                _trackColorType = value;
            }
        }
        private TrackColorType _trackColorType = TrackColorType.Single;
        private bool trackColorTypeflag;
        public string trackColor
        {
            get => _trackColor;
            set
            {
                trackColorflag = true;
                _trackColor = value;
            }
        }
        private string _trackColor = "debb7b";
        private bool trackColorflag;
        public string secondaryTrackColor
        {
            get => _secondaryTrackColor;
            set
            {
                secondaryTrackColorflag = true;
                _secondaryTrackColor = value;
            }
        }
        private string _secondaryTrackColor = "ffffff";
        private bool secondaryTrackColorflag;
        public double trackColorAnimDuration
        {
            get => _trackColorAnimDuration;
            set
            {
                trackColorAnimDurationflag = true;
                _trackColorAnimDuration = value;
            }
        }
        private double _trackColorAnimDuration = 2;
        private bool trackColorAnimDurationflag;
        public TrackColorPulse trackColorPulse
        {
            get => _trackColorPulse;
            set
            {
                trackColorPulseflag = true;
                _trackColorPulse = value;
            }
        }
        private TrackColorPulse _trackColorPulse = TrackColorPulse.None;
        private bool trackColorPulseflag;
        public double trackPulseLength
        {
            get => _trackPulseLength;
            set
            {
                trackPulseLengthflag = true;
                _trackPulseLength = value;
            }
        }
        private double _trackPulseLength = 10;
        private bool trackPulseLengthflag;
        public TrackStyle trackStyle
        {
            get => _trackStyle;
            set
            {
                trackStyleflag = true;
                _trackStyle = value;
            }
        }
        private TrackStyle _trackStyle = TrackStyle.Standard;
        private bool trackStyleflag;
        public TrackAnimationType trackAnimation
        {
            get => _trackAnimation;
            set
            {
                trackAnimationflag = true;
                _trackAnimation = value;
            }
        }
        private TrackAnimationType _trackAnimation = TrackAnimationType.None;
        private bool trackAnimationflag;
        public int beatsAhead
        {
            get => _beatsAhead;
            set
            {
                beatsAheadflag = true;
                _beatsAhead = value;
            }
        }
        private int _beatsAhead = 3;
        private bool beatsAheadflag;
        public TrackAnimationType2 trackDisappearAnimation
        {
            get => _trackDisappearAnimation;
            set
            {
                trackDisappearAnimationflag = true;
                _trackDisappearAnimation = value;
            }
        }
        private TrackAnimationType2 _trackDisappearAnimation = TrackAnimationType2.None;
        private bool trackDisappearAnimationflag;
        public int beatsBehind
        {
            get => _beatsBehind;
            set
            {
                beatsBehindflag = true;
                _beatsBehind = value;
            }
        }
        private int _beatsBehind = 4;
        private bool beatsBehindflag;
        public ChangeTrack() : base(LevelEventType.ChangeTrack) { }
        public ChangeTrack(TrackColorType trackColorType, string trackColor, string secondaryTrackColor, double trackColorAnimDuration, TrackColorPulse trackColorPulse, double trackPulseLength, TrackStyle trackStyle, TrackAnimationType trackAnimation, int beatsAhead, TrackAnimationType2 trackDisappearAnimation, int beatsBehind, bool active) : base(LevelEventType.ChangeTrack, active)
        {
            this.trackColorType = trackColorType;
            this.trackColor = trackColor;
            this.secondaryTrackColor = secondaryTrackColor;
            this.trackColorAnimDuration = trackColorAnimDuration;
            this.trackColorPulse = trackColorPulse;
            this.trackPulseLength = trackPulseLength;
            this.trackStyle = trackStyle;
            this.trackAnimation = trackAnimation;
            this.beatsAhead = beatsAhead;
            this.trackDisappearAnimation = trackDisappearAnimation;
            this.beatsBehind = beatsBehind;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (trackColorTypeflag)
                node["trackColorType"] = _trackColorType.ToString();
            if (trackColorflag)
                node["trackColor"] = _trackColor;
            if (secondaryTrackColorflag)
                node["secondaryTrackColor"] = _secondaryTrackColor;
            if (trackColorAnimDurationflag)
                node["trackColorAnimDuration"] = _trackColorAnimDuration;
            if (trackColorPulseflag)
                node["trackColorPulse"] = _trackColorPulse.ToString();
            if (trackPulseLengthflag)
                node["trackPulseLength"] = _trackPulseLength;
            if (trackStyleflag)
                node["trackStyle"] = _trackStyle.ToString();
            if (trackAnimationflag)
                node["trackAnimation"] = _trackAnimation.ToString();
            if (beatsAheadflag)
                node["beatsAhead"] = _beatsAhead;
            if (trackDisappearAnimationflag)
                node["trackDisappearAnimation"] = _trackDisappearAnimation.ToString();
            if (beatsBehindflag)
                node["beatsBehind"] = _beatsBehind;
            return node;
        }
    }
}
