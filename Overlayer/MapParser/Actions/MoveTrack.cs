using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class MoveTrack : Action
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
        public double gapLength
        {
            get => _gapLength;
            set
            {
                gapLengthflag = true;
                _gapLength = value;
            }
        }
        private double _gapLength = 0;
        private bool gapLengthflag;
        public double duration
        {
            get => _duration;
            set
            {
                durationflag = true;
                _duration = value;
            }
        }
        private double _duration = 1;
        private bool durationflag;
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
        public double rotationOffset
        {
            get => _rotationOffset;
            set
            {
                rotationOffsetflag = true;
                _rotationOffset = value;
            }
        }
        private double _rotationOffset = 0;
        private bool rotationOffsetflag;
        public double scale
        {
            get => _scale;
            set
            {
                scaleflag = true;
                _scale = value;
            }
        }
        private double _scale = 100;
        private bool scaleflag;
        public int opacity
        {
            get => _opacity;
            set
            {
                opacityflag = true;
                _opacity = value;
            }
        }
        private int _opacity = 100;
        private bool opacityflag;
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
        public Ease ease
        {
            get => _ease;
            set
            {
                easeflag = true;
                _ease = value;
            }
        }
        private Ease _ease = Ease.Linear;
        private bool easeflag;
        public Toggle maxVfxOnly
        {
            get => _maxVfxOnly;
            set
            {
                maxVfxOnlyflag = true;
                _maxVfxOnly = value;
            }
        }
        private Toggle _maxVfxOnly = Toggle.Disabled;
        private bool maxVfxOnlyflag;
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
        public MoveTrack() : base(LevelEventType.MoveTrack) { }
        public MoveTrack((int, TileRelativeTo) startTile, (int, TileRelativeTo) endTile, double gapLength, double duration, Vector2 positionOffset, double rotationOffset, double scale, int opacity, double angleOffset, Ease ease, Toggle maxVfxOnly, string eventTag, bool active) : base(LevelEventType.MoveTrack, active)
        {
            this.startTile = startTile;
            this.endTile = endTile;
            this.gapLength = gapLength;
            this.duration = duration;
            this.positionOffset = positionOffset;
            this.rotationOffset = rotationOffset;
            this.scale = scale;
            this.opacity = opacity;
            this.angleOffset = angleOffset;
            this.ease = ease;
            this.maxVfxOnly = maxVfxOnly;
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
            if (gapLengthflag)
                node["gapLength"] = _gapLength;
            if (durationflag)
                node["duration"] = _duration;
            if (positionOffsetflag)
                node["positionOffset"] = _positionOffset.ToNode();
            if (rotationOffsetflag)
                node["rotationOffset"] = _rotationOffset;
            if (scaleflag)
                node["scale"] = _scale;
            if (opacityflag)
                node["opacity"] = _opacity;
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (easeflag)
                node["ease"] = _ease.ToString();
            if (maxVfxOnlyflag)
                node["maxVfxOnly"] = _maxVfxOnly.ToString();
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
