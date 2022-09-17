using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class CustomBackground : Action
    {
        public string color
        {
            get => _color;
            set
            {
                colorflag = true;
                _color = value;
            }
        }
        private string _color = "000000";
        private bool colorflag;
        public string bgImage
        {
            get => _bgImage;
            set
            {
                bgImageflag = true;
                _bgImage = value;
            }
        }
        private string _bgImage = "";
        private bool bgImageflag;
        public string imageColor
        {
            get => _imageColor;
            set
            {
                imageColorflag = true;
                _imageColor = value;
            }
        }
        private string _imageColor = "ffffff";
        private bool imageColorflag;
        public Vector2 parallax
        {
            get => _parallax;
            set
            {
                parallaxflag = true;
                _parallax = value;
            }
        }
        private Vector2 _parallax = Vector2.Hrd;
        private bool parallaxflag;
        public BgDisplayMode bgDisplayMode
        {
            get => _bgDisplayMode;
            set
            {
                bgDisplayModeflag = true;
                _bgDisplayMode = value;
            }
        }
        private BgDisplayMode _bgDisplayMode = BgDisplayMode.FitToScreen;
        private bool bgDisplayModeflag;
        public Toggle lockRot
        {
            get => _lockRot;
            set
            {
                lockRotflag = true;
                _lockRot = value;
            }
        }
        private Toggle _lockRot = Toggle.Disabled;
        private bool lockRotflag;
        public Toggle loopBG
        {
            get => _loopBG;
            set
            {
                loopBGflag = true;
                _loopBG = value;
            }
        }
        private Toggle _loopBG = Toggle.Disabled;
        private bool loopBGflag;
        public double unscaledSize
        {
            get => _unscaledSize;
            set
            {
                unscaledSizeflag = true;
                _unscaledSize = value;
            }
        }
        private double _unscaledSize = 100;
        private bool unscaledSizeflag;
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
        public CustomBackground() : base(LevelEventType.CustomBackground) { }
        public CustomBackground(string color, string bgImage, string imageColor, Vector2 parallax, BgDisplayMode bgDisplayMode, Toggle lockRot, Toggle loopBG, double unscaledSize, double angleOffset, string eventTag, bool active) : base(LevelEventType.CustomBackground, active)
        {
            this.color = color;
            this.bgImage = bgImage;
            this.imageColor = imageColor;
            this.parallax = parallax;
            this.bgDisplayMode = bgDisplayMode;
            this.lockRot = lockRot;
            this.loopBG = loopBG;
            this.unscaledSize = unscaledSize;
            this.angleOffset = angleOffset;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (colorflag)
                node["color"] = _color;
            if (bgImageflag)
                node["bgImage"] = _bgImage;
            if (imageColorflag)
                node["imageColor"] = _imageColor;
            if (parallaxflag)
                node["parallax"] = _parallax.ToNode();
            if (bgDisplayModeflag)
                node["bgDisplayMode"] = _bgDisplayMode.ToString();
            if (lockRotflag)
                node["lockRot"] = _lockRot.ToString();
            if (loopBGflag)
                node["loopBG"] = _loopBG.ToString();
            if (unscaledSizeflag)
                node["unscaledSize"] = _unscaledSize;
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
