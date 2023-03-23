using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class ColorTrack : Action
    {
        public TrackColorType trackColorType = TrackColorType.Single;
        public string trackColor = "debb7b";
        public string secondaryTrackColor = "ffffff";
        public double trackColorAnimDuration = 2;
        public TrackColorPulse trackColorPulse = TrackColorPulse.None;
        public double trackPulseLength = 10;
        public TrackStyle trackStyle = TrackStyle.Standard;
        public string trackTexture = "";
        public double trackTextureScale = 1;
        public ColorTrack() : base(LevelEventType.ColorTrack) { }
        public ColorTrack(TrackColorType trackColorType, string trackColor, string secondaryTrackColor, double trackColorAnimDuration, TrackColorPulse trackColorPulse, double trackPulseLength, TrackStyle trackStyle, string trackTexture, double trackTextureScale, bool active) : base(LevelEventType.ColorTrack, active)
        {
            this.trackColorType = trackColorType;
            this.trackColor = trackColor;
            this.secondaryTrackColor = secondaryTrackColor;
            this.trackColorAnimDuration = trackColorAnimDuration;
            this.trackColorPulse = trackColorPulse;
            this.trackPulseLength = trackPulseLength;
            this.trackStyle = trackStyle;
            this.trackTexture = trackTexture;
            this.trackTextureScale = trackTextureScale;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["trackColorType"] = trackColorType.ToString();
            node["trackColor"] = trackColor;
            node["secondaryTrackColor"] = secondaryTrackColor;
            node["trackColorAnimDuration"] = trackColorAnimDuration;
            node["trackColorPulse"] = trackColorPulse.ToString();
            node["trackPulseLength"] = trackPulseLength;
            node["trackStyle"] = trackStyle.ToString();
            node["trackTexture"] = trackTexture;
            node["trackTextureScale"] = trackTextureScale;
            return node;
        }
    }
}
