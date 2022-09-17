using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class ChangeTrack : Action
    {
        public TrackColorType trackColorType = TrackColorType.Single;
        public string trackColor = "debb7b";
        public string secondaryTrackColor = "ffffff";
        public double trackColorAnimDuration = 2;
        public TrackColorPulse trackColorPulse = TrackColorPulse.None;
        public double trackPulseLength = 10;
        public TrackStyle trackStyle = TrackStyle.Standard;
        public TrackAnimationType trackAnimation = TrackAnimationType.None;
        public int beatsAhead = 3;
        public TrackAnimationType2 trackDisappearAnimation = TrackAnimationType2.None;
        public int beatsBehind = 4;
        public ChangeTrack() : base(LevelEventType.ChangeTrack) { }
        public ChangeTrack(TrackColorType trackColorType, string trackColor, string secondaryTrackColor, double trackColorAnimDuration, TrackColorPulse trackColorPulse, double trackPulseLength, TrackStyle trackStyle, TrackAnimationType trackAnimation, int beatsAhead, TrackAnimationType2 trackDisappearAnimation, int beatsBehind, bool active) : base(LevelEventType.ChangeTrack, active)
        {
            this.trackColorType = trackColorType;
            this.trackColor = trackColor;
            this.secondaryTrackColor = secondaryTrackColor;
            this.trackColorAnimDuration = trackColorAnimDuration;
            this.trackColorPulse = trackColorPulse;
            this.trackPulseLength = trackPulseLength;
            this.trackStyle = trackStyle;
            this.trackAnimation = trackAnimation;
            this.beatsAhead = beatsAhead;
            this.trackDisappearAnimation = trackDisappearAnimation;
            this.beatsBehind = beatsBehind;
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
            node["trackAnimation"] = trackAnimation.ToString();
            node["beatsAhead"] = beatsAhead;
            node["trackDisappearAnimation"] = trackDisappearAnimation.ToString();
            node["beatsBehind"] = beatsBehind;
            return node;
        }
    }
}
