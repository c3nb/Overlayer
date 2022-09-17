using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class ColorTrack : Action
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
        public string trackTexture
        {
            get => _trackTexture;
            set
            {
                trackTextureflag = true;
                _trackTexture = value;
            }
        }
        private string _trackTexture = "";
        private bool trackTextureflag;
        public double trackTextureScale
        {
            get => _trackTextureScale;
            set
            {
                trackTextureScaleflag = true;
                _trackTextureScale = value;
            }
        }
        private double _trackTextureScale = 1;
        private bool trackTextureScaleflag;
        public ColorTrack() : base(LevelEventType.ColorTrack) { }
        public ColorTrack(TrackColorType trackColorType, string trackColor, string secondaryTrackColor, double trackColorAnimDuration, TrackColorPulse trackColorPulse, double trackPulseLength, TrackStyle trackStyle, string trackTexture, double trackTextureScale, bool active) : base(LevelEventType.ColorTrack, active)
        {
            this.trackColorType = trackColorType;
            this.trackColor = trackColor;
            this.secondaryTrackColor = secondaryTrackColor;
            this.trackColorAnimDuration = trackColorAnimDuration;
            this.trackColorPulse = trackColorPulse;
            this.trackPulseLength = trackPulseLength;
            this.trackStyle = trackStyle;
            this.trackTexture = trackTexture;
            this.trackTextureScale = trackTextureScale;
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
            if (trackTextureflag)
                node["trackTexture"] = _trackTexture;
            if (trackTextureScaleflag)
                node["trackTextureScale"] = _trackTextureScale;
            return node;
        }
    }
}
