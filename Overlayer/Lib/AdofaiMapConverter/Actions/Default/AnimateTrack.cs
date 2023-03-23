using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class AnimateTrack : Action
    {
        public TrackAnimationType trackAnimation = TrackAnimationType.None;
        public int beatsAhead = 3;
        public TrackAnimationType2 trackDisappearAnimation = TrackAnimationType2.None;
        public int beatsBehind = 4;
        public AnimateTrack() : base(LevelEventType.AnimateTrack) { }
        public AnimateTrack(TrackAnimationType trackAnimation, int beatsAhead, TrackAnimationType2 trackDisappearAnimation, int beatsBehind, bool active) : base(LevelEventType.AnimateTrack, active)
        {
            this.trackAnimation = trackAnimation;
            this.beatsAhead = beatsAhead;
            this.trackDisappearAnimation = trackDisappearAnimation;
            this.beatsBehind = beatsBehind;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["trackAnimation"] = trackAnimation.ToString();
            node["beatsAhead"] = beatsAhead;
            node["trackDisappearAnimation"] = trackDisappearAnimation.ToString();
            node["beatsBehind"] = beatsBehind;
            return node;
        }
    }
}
