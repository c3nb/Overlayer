using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class RecolorTrack : Action
    {
        public (int, TileRelativeTo) startTile
        {
            get => _startTile;
            set
            {
                startTileflag = true;
                _startTile = value;
            }
        }
        private (int, TileRelativeTo) _startTile = (0, TileRelativeTo.ThisTile);
        private bool startTileflag;
        public (int, TileRelativeTo) endTile
        {
            get => _endTile;
            set
            {
                endTileflag = true;
                _endTile = value;
            }
        }
        private (int, TileRelativeTo) _endTile = (0, TileRelativeTo.ThisTile);
        private bool endTileflag;
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
        public RecolorTrack() : base(LevelEventType.RecolorTrack) { }
        public RecolorTrack((int, TileRelativeTo) startTile, (int, TileRelativeTo) endTile, TrackColorType trackColorType, string trackColor, string secondaryTrackColor, double trackColorAnimDuration, TrackColorPulse trackColorPulse, double trackPulseLength, TrackStyle trackStyle, double angleOffset, string eventTag, bool active) : base(LevelEventType.RecolorTrack, active)
        {
            this.startTile = startTile;
            this.endTile = endTile;
            this.trackColorType = trackColorType;
            this.trackColor = trackColor;
            this.secondaryTrackColor = secondaryTrackColor;
            this.trackColorAnimDuration = trackColorAnimDuration;
            this.trackColorPulse = trackColorPulse;
            this.trackPulseLength = trackPulseLength;
            this.trackStyle = trackStyle;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (startTileflag)
            {
                JsonArray _startTileArr = new JsonArray();
                _startTileArr[0] = startTile.Item1;
                _startTileArr[1] = startTile.Item2.ToString();
                node["startTile"] = _startTileArr;
            }
            if (endTileflag)
            {
                JsonArray _endTileArr = new JsonArray();
                _endTileArr[0] = endTile.Item1;
                _endTileArr[1] = endTile.Item2.ToString();
                node["endTile"] = _endTileArr;
            }
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
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
