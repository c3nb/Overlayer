using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class MoveDecorations : Action
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
        private double _duration = 1;
        private bool durationflag;
        public string tag
        {
            get => _tag;
            set
            {
                tagflag = true;
                _tag = value;
            }
        }
        private string _tag = "sampleTag";
        private bool tagflag;
        public string decorationImage
        {
            get => _decorationImage;
            set
            {
                decorationImageflag = true;
                _decorationImage = value;
            }
        }
        private string _decorationImage = "";
        private bool decorationImageflag;
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
        public string color
        {
            get => _color;
            set
            {
                colorflag = true;
                _color = value;
            }
        }
        private string _color = "ffffff";
        private bool colorflag;
        public int depth
        {
            get => _depth;
            set
            {
                depthflag = true;
                _depth = value;
            }
        }
        private int _depth = 0;
        private bool depthflag;
        public Vector2 parallax
        {
            get => _parallax;
            set
            {
                parallaxflag = true;
                _parallax = value;
            }
        }
        private Vector2 _parallax = Vector2.Zero;
        private bool parallaxflag;
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
        public MoveDecorations() : base(LevelEventType.MoveDecorations) { }
        public MoveDecorations(double duration, string tag, string decorationImage, Vector2 positionOffset, double rotationOffset, double scale, string color, int depth, Vector2 parallax, double angleOffset, Ease ease, string eventTag, bool active) : base(LevelEventType.MoveDecorations, active)
        {
            this.duration = duration;
            this.tag = tag;
            this.decorationImage = decorationImage;
            this.positionOffset = positionOffset;
            this.rotationOffset = rotationOffset;
            this.scale = scale;
            this.color = color;
            this.depth = depth;
            this.parallax = parallax;
            this.angleOffset = angleOffset;
            this.ease = ease;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (durationflag)
                node["duration"] = _duration;
            if (tagflag)
                node["tag"] = _tag;
            if (decorationImageflag)
                node["decorationImage"] = _decorationImage;
            if (positionOffsetflag)
                node["positionOffset"] = _positionOffset.ToNode();
            if (rotationOffsetflag)
                node["rotationOffset"] = _rotationOffset;
            if (scaleflag)
                node["scale"] = _scale;
            if (colorflag)
                node["color"] = _color;
            if (depthflag)
                node["depth"] = _depth;
            if (parallaxflag)
                node["parallax"] = _parallax.ToNode();
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (easeflag)
                node["ease"] = _ease.ToString();
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
