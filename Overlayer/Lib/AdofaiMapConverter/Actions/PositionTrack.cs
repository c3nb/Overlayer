using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class PositionTrack : Action
    {
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
        public (int, TileRelativeTo) relativeTo
        {
            get => _relativeTo;
            set
            {
                relativeToflag = true;
                _relativeTo = value;
            }
        }
        private (int, TileRelativeTo) _relativeTo = (0, TileRelativeTo.ThisTile);
        private bool relativeToflag;
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
        public Toggle justThisTile
        {
            get => _justThisTile;
            set
            {
                justThisTileflag = true;
                _justThisTile = value;
            }
        }
        private Toggle _justThisTile = Toggle.Disabled;
        private bool justThisTileflag;
        public Toggle editorOnly
        {
            get => _editorOnly;
            set
            {
                editorOnlyflag = true;
                _editorOnly = value;
            }
        }
        private Toggle _editorOnly = Toggle.Disabled;
        private bool editorOnlyflag;
        public PositionTrack() : base(LevelEventType.PositionTrack) { }
        public PositionTrack(Vector2 positionOffset, (int, TileRelativeTo) relativeTo, double rotation, double scale, int opacity, Toggle justThisTile, Toggle editorOnly, bool active) : base(LevelEventType.PositionTrack, active)
        {
            this.positionOffset = positionOffset;
            this.relativeTo = relativeTo;
            this.rotation = rotation;
            this.scale = scale;
            this.opacity = opacity;
            this.justThisTile = justThisTile;
            this.editorOnly = editorOnly;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (positionOffsetflag)
                node["positionOffset"] = _positionOffset.ToNode();
            if (relativeToflag)
            {
                JsonArray _relativeToArr = new JsonArray();
                _relativeToArr[0] = relativeTo.Item1;
                _relativeToArr[1] = relativeTo.Item2.ToString();
                node["relativeTo"] = _relativeToArr;
            }
            if (rotationflag)
                node["rotation"] = _rotation;
            if (scaleflag)
                node["scale"] = _scale;
            if (opacityflag)
                node["opacity"] = _opacity;
            if (justThisTileflag)
                node["justThisTile"] = _justThisTile.ToString();
            if (editorOnlyflag)
                node["editorOnly"] = _editorOnly.ToString();
            return node;
        }
    }
}
