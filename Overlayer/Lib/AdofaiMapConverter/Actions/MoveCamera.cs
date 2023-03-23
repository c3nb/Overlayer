using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class MoveCamera : Action
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
        public CamMovementType relativeTo
        {
            get => _relativeTo;
            set
            {
                relativeToflag = true;
                _relativeTo = value;
            }
        }
        private CamMovementType _relativeTo = CamMovementType.Player;
        private bool relativeToflag;
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
        public int zoom
        {
            get => _zoom;
            set
            {
                zoomflag = true;
                _zoom = value;
            }
        }
        private int _zoom = 0;
        private bool zoomflag;
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
        public Toggle dontDisable
        {
            get => _dontDisable;
            set
            {
                dontDisableflag = true;
                _dontDisable = value;
            }
        }
        private Toggle _dontDisable = Toggle.Disabled;
        private bool dontDisableflag;
        public Toggle minVfxOnly
        {
            get => _minVfxOnly;
            set
            {
                minVfxOnlyflag = true;
                _minVfxOnly = value;
            }
        }
        private Toggle _minVfxOnly = Toggle.Disabled;
        private bool minVfxOnlyflag;
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
        public MoveCamera() : base(LevelEventType.MoveCamera) { }
        public MoveCamera(double duration, CamMovementType relativeTo, Vector2 position, double rotation, int zoom, double angleOffset, Ease ease, Toggle dontDisable, Toggle minVfxOnly, string eventTag, bool active) : base(LevelEventType.MoveCamera, active)
        {
            this.duration = duration;
            this.relativeTo = relativeTo;
            this.position = position;
            this.rotation = rotation;
            this.zoom = zoom;
            this.angleOffset = angleOffset;
            this.ease = ease;
            this.dontDisable = dontDisable;
            this.minVfxOnly = minVfxOnly;
            this.eventTag = eventTag;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (durationflag)
                node["duration"] = _duration;
            if (relativeToflag)
                node["relativeTo"] = _relativeTo.ToString();
            if (positionflag)
                node["position"] = _position.ToNode();
            if (rotationflag)
                node["rotation"] = _rotation;
            if (zoomflag)
                node["zoom"] = _zoom;
            if (angleOffsetflag)
                node["angleOffset"] = _angleOffset;
            if (easeflag)
                node["ease"] = _ease.ToString();
            if (dontDisableflag)
                node["dontDisable"] = _dontDisable.ToString();
            if (minVfxOnlyflag)
                node["minVfxOnly"] = _minVfxOnly.ToString();
            if (eventTagflag)
                node["eventTag"] = _eventTag;
            return node;
        }
    }
}
