using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class RecolorTrack : Action
    {
        public (int, TileRelativeTo) startTile = (0, TileRelativeTo.ThisTile);
        public (int, TileRelativeTo) endTile = (0, TileRelativeTo.ThisTile);
        public TrackColorType trackColorType = TrackColorType.Single;
        public string trackColor = "debb7b";
        public string secondaryTrackColor = "ffffff";
        public double trackColorAnimDuration = 2;
        public TrackColorPulse trackColorPulse = TrackColorPulse.None;
        public double trackPulseLength = 10;
        public TrackStyle trackStyle = TrackStyle.Standard;
        public double angleOffset = 0;
        public string eventTag = "";
        public RecolorTrack() : base(LevelEventType.RecolorTrack) { }
        public RecolorTrack((int, TileRelativeTo) startTile, (int, TileRelativeTo) endTile, TrackColorType trackColorType, string trackColor, string secondaryTrackColor, double trackColorAnimDuration, TrackColorPulse trackColorPulse, double trackPulseLength, TrackStyle trackStyle, double angleOffset, string eventTag, bool active) : base(LevelEventType.RecolorTrack, active)
        {
            this.startTile = startTile;
            this.endTile = endTile;
            this.trackColorType = trackColorType;
            this.trackColor = trackColor;
            this.secondaryTrackColor = secondaryTrackColor;
            this.trackColorAnimDuration = trackColorAnimDuration;
            this.trackColorPulse = trackColorPulse;
            this.trackPulseLength = trackPulseLength;
            this.trackStyle = trackStyle;
            this.angleOffset = angleOffset;
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

            node["trackColorType"] = trackColorType.ToString();
            node["trackColor"] = trackColor;
            node["secondaryTrackColor"] = secondaryTrackColor;
            node["trackColorAnimDuration"] = trackColorAnimDuration;
            node["trackColorPulse"] = trackColorPulse.ToString();
            node["trackPulseLength"] = trackPulseLength;
            node["trackStyle"] = trackStyle.ToString();
            node["angleOffset"] = angleOffset;
            node["eventTag"] = eventTag;
            return node;
        }
    }
}
