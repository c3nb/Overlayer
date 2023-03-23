using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class MoveCamera : Action
    {
        public double duration = 1;
        public CamMovementType relativeTo = CamMovementType.Player;
        public Vector2 position = Vector2.Zero;
        public double rotation;
        public int zoom;
        public double angleOffset = 0;
        public Ease ease = Ease.Linear;
        public Toggle dontDisable = Toggle.Disabled;
        public Toggle minVfxOnly = Toggle.Disabled;
        public string eventTag = "";
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
            node["duration"] = duration;
            node["relativeTo"] = relativeTo.ToString();
            node["position"] = position.ToNode();
            node["rotation"] = rotation;
            node["zoom"] = zoom;
            node["angleOffset"] = angleOffset;
            node["ease"] = ease.ToString();
            node["dontDisable"] = dontDisable.ToString();
            node["minVfxOnly"] = minVfxOnly.ToString();
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
