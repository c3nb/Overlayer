using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class MoveDecorations : Action
    {
        public double duration = 1;
        public string tag = "sampleTag";
        public string decorationImage = "";
        public Vector2 positionOffset = Vector2.Zero;
        public double rotationOffset = 0;
        public double scale = 100;
        public string color = "ffffff";
        public int depth = 0;
        public Vector2 parallax = Vector2.Zero;
        public double angleOffset = 0;
        public Ease ease = Ease.Linear;
        public string eventTag = "";
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
            node["duration"] = duration;
            node["tag"] = tag;
            node["positionOffset"] = positionOffset.ToNode();
            node["rotationOffset"] = rotationOffset;
            node["scale"] = scale;
            node["color"] = color;
            node["depth"] = depth;
            node["parallax"] = parallax.ToNode();
            node["angleOffset"] = angleOffset;
            node["ease"] = ease.ToString();
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
