using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class MoveTrack : Action
    {
        public (int, TileRelativeTo) startTile = (0, TileRelativeTo.ThisTile);
        public (int, TileRelativeTo) endTile = (0, TileRelativeTo.ThisTile);
        public double gapLength = 0;
        public double duration = 1;
        public Vector2 positionOffset = Vector2.Zero;
        public double rotationOffset = 0;
        public double scale = 100;
        public int opacity = 100;
        public double angleOffset = 0;
        public Ease ease = Ease.Linear;
        public Toggle maxVfxOnly = Toggle.Disabled;
        public string eventTag = "";
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

            JsonArray newArr = new JsonArray();
            newArr.Inline = true;
            newArr[0] = startTile.Item1;
            newArr[1] = startTile.Item2.ToString();
            node["startTile"] = newArr;

            JsonArray newArr2 = new JsonArray();
            newArr2.Inline = true;
            newArr2[0] = endTile.Item1;
            newArr2[1] = endTile.Item2.ToString();
            node["endTile"] = newArr2;

            node["gapLength"] = gapLength;
            node["duration"] = duration;
            node["positionOffset"] = positionOffset.ToNode();
            node["rotationOffset"] = rotationOffset;
            node["scale"] = scale;
            node["opacity"] = opacity;
            node["angleOffset"] = angleOffset;
            node["ease"] = ease.ToString();
            node["maxVfxOnly"] = maxVfxOnly.ToString();
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
