using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Decorations.DecorationUtils;

namespace AdofaiMapConverter.Decorations
{
    public class AddDecoration : Decoration
    {
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
        public Vector2 tile
        {
            get => _tile;
            set
            {
                tileflag = true;
                _tile = value;
            }
        }
        private Vector2 _tile = Vector2.One;
        private bool tileflag;
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
        private double _depth = 0;
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
        public Toggle imageSmoothing
        {
            get => _imageSmoothing;
            set
            {
                imageSmoothingflag = true;
                _imageSmoothing = value;
            }
        }
        private Toggle _imageSmoothing = Toggle.Enabled;
        private bool imageSmoothingflag;
        public Toggle failHitbox
        {
            get => _failHitbox;
            set
            {
                failHitboxflag = true;
                _failHitbox = value;
            }
        }
        private Toggle _failHitbox = Toggle.Disabled;
        private bool failHitboxflag;
        public Hitbox failHitboxType
        {
            get => _failHitboxType;
            set
            {
                failHitboxTypeflag = true;
                _failHitboxType = value;
            }
        }
        private Hitbox _failHitboxType = Hitbox.Box;
        private bool failHitboxTypeflag;
        public Vector2 failHitboxScale
        {
            get => _failHitboxScale;
            set
            {
                failHitboxScaleflag = true;
                _failHitboxScale = value;
            }
        }
        private Vector2 _failHitboxScale = Vector2.Hrd;
        private bool failHitboxScaleflag;
        public Vector2 failHitboxOffset
        {
            get => _failHitboxOffset;
            set
            {
                failHitboxOffsetflag = true;
                _failHitboxOffset = value;
            }
        }
        private Vector2 _failHitboxOffset = Vector2.Zero;
        private bool failHitboxOffsetflag;
        public double failHitboxRotation
        {
            get => _failHitboxRotation;
            set
            {
                failHitboxRotationflag = true;
                _failHitboxRotation = value;
            }
        }
        private double _failHitboxRotation = 0;
        private bool failHitboxRotationflag;
        public string components
        {
            get => _components;
            set
            {
                componentsflag = true;
                _components = value;
            }
        }
        private string _components = "";
        private bool componentsflag;
        public AddDecoration() : base(LevelEventType.AddDecoration) { }
        public AddDecoration(string decorationImage, Vector2 position, DecPlacementType relativeTo, Vector2 pivotOffset, double rotation, Vector2 scale, Vector2 tile, string color, int opacity, double depth, Vector2 parallax, string tag, Toggle imageSmoothing, Toggle failHitbox, Hitbox failHitboxType, Vector2 failHitboxScale, Vector2 failHitboxOffset, double failHitboxRotation, string components, bool active) : base(LevelEventType.AddDecoration, active)
        {
            this.decorationImage = decorationImage;
            this.position = position;
            this.relativeTo = relativeTo;
            this.pivotOffset = pivotOffset;
            this.rotation = rotation;
            this.scale = scale;
            this.tile = tile;
            this.color = color;
            this.opacity = opacity;
            this.depth = depth;
            this.parallax = parallax;
            this.tag = tag;
            this.imageSmoothing = imageSmoothing;
            this.failHitbox = failHitbox;
            this.failHitboxType = failHitboxType;
            this.failHitboxScale = failHitboxScale;
            this.failHitboxOffset = failHitboxOffset;
            this.failHitboxRotation = failHitboxRotation;
            this.components = components;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, floor, visible);
            if (decorationImageflag)
                node["decorationImage"] = _decorationImage;
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
            if (tileflag)
                node["tile"] = _tile.ToNode();
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
            if (imageSmoothingflag)
                node["imageSmoothing"] = _imageSmoothing.ToString();
            if (failHitboxflag)
                node["failHitbox"] = _failHitbox.ToString();
            if (failHitboxTypeflag)
                node["failHitboxType"] = _failHitboxType.ToString();
            if (failHitboxScaleflag)
                node["failHitboxScale"] = _failHitboxScale.ToNode();
            if (failHitboxOffsetflag)
                node["failHitboxOffset"] = _failHitboxOffset.ToNode();
            if (failHitboxRotationflag)
                node["failHitboxRotation"] = _failHitboxRotation;
            if (componentsflag)
                node["components"] = _components;
            return node;
        }
    }
}
