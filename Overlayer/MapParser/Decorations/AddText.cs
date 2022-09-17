using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Decorations.DecorationUtils;

namespace Overlayer.MapParser.Decorations
{
    public class AddText : Decoration
    {
        public string decText
        {
            get => _decText;
            set
            {
                decTextflag = true;
                _decText = value;
            }
        }
        private string _decText = "텍스트";
        private bool decTextflag;
        public FontName font
        {
            get => _font;
            set
            {
                fontflag = true;
                _font = value;
            }
        }
        private FontName _font = FontName.Default;
        private bool fontflag;
        public Vector2 position
        {
            get => _position;
            set
            {
                positionflag = true;
                _position = value;
            }
        }
        private Vector2 _position = Vector2.Zero;
        private bool positionflag;
        public DecPlacementType relativeTo
        {
            get => _relativeTo;
            set
            {
                relativeToflag = true;
                _relativeTo = value;
            }
        }
        private DecPlacementType _relativeTo = DecPlacementType.Tile;
        private bool relativeToflag;
        public Vector2 pivotOffset
        {
            get => _pivotOffset;
            set
            {
                pivotOffsetflag = true;
                _pivotOffset = value;
            }
        }
        private Vector2 _pivotOffset = Vector2.Zero;
        private bool pivotOffsetflag;
        public double rotation
        {
            get => _rotation;
            set
            {
                rotationflag = true;
                _rotation = value;
            }
        }
        private double _rotation = 0;
        private bool rotationflag;
        public Vector2 scale
        {
            get => _scale;
            set
            {
                scaleflag = true;
                _scale = value;
            }
        }
        private Vector2 _scale = Vector2.Hrd;
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
        public double depth
        {
            get => _depth;
            set
            {
                depthflag = true;
                _depth = value;
            }
        }
        private double _depth = -1;
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
        private Vector2 _parallax = Vector2.MOne;
        private bool parallaxflag;
        public string tag
        {
            get => _tag;
            set
            {
                tagflag = true;
                _tag = value;
            }
        }
        private string _tag = "";
        private bool tagflag;
        public AddText() : base(LevelEventType.AddText) { }
        public AddText(string decText, FontName font, Vector2 position, DecPlacementType relativeTo, Vector2 pivotOffset, double rotation, Vector2 scale, string color, int opacity, double depth, Vector2 parallax, string tag, bool active) : base(LevelEventType.AddText, active)
        {
            this.decText = decText;
            this.font = font;
            this.position = position;
            this.relativeTo = relativeTo;
            this.pivotOffset = pivotOffset;
            this.rotation = rotation;
            this.scale = scale;
            this.color = color;
            this.opacity = opacity;
            this.depth = depth;
            this.parallax = parallax;
            this.tag = tag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, floor, visible);
            if (decTextflag)
                node["decText"] = _decText;
            if (fontflag)
                node["font"] = _font.ToString();
            if (positionflag)
                node["position"] = _position.ToNode();
            if (relativeToflag)
                node["relativeTo"] = _relativeTo.ToString();
            if (pivotOffsetflag)
                node["pivotOffset"] = _pivotOffset.ToNode();
            if (rotationflag)
                node["rotation"] = _rotation;
            if (scaleflag)
                node["scale"] = _scale.ToNode();
            if (colorflag)
                node["color"] = _color;
            if (opacityflag)
                node["opacity"] = _opacity;
            if (depthflag)
                node["depth"] = _depth;
            if (parallaxflag)
                node["parallax"] = _parallax.ToNode();
            if (tagflag)
                node["tag"] = _tag;
            return node;
        }
    }
}
