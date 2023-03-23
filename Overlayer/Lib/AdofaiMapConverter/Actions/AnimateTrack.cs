using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class AnimateTrack : Action
    {
        public TrackAnimationType trackAnimation
        {
            get => _trackAnimation;
            set
            {
                trackAnimationflag = true;
                _trackAnimation = value;
            }
        }
        private TrackAnimationType _trackAnimation = TrackAnimationType.None;
        private bool trackAnimationflag;
        public double beatsAhead
        {
            get => _beatsAhead;
            set
            {
                beatsAheadflag = true;
                _beatsAhead = value;
            }
        }
        private double _beatsAhead = 3;
        private bool beatsAheadflag;
        public TrackAnimationType2 trackDisappearAnimation
        {
            get => _trackDisappearAnimation;
            set
            {
                trackDisappearAnimationflag = true;
                _trackDisappearAnimation = value;
            }
        }
        private TrackAnimationType2 _trackDisappearAnimation = TrackAnimationType2.None;
        private bool trackDisappearAnimationflag;
        public double beatsBehind
        {
            get => _beatsBehind;
            set
            {
                beatsBehindflag = true;
                _beatsBehind = value;
            }
        }
        private double _beatsBehind = 4;
        private bool beatsBehindflag;
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
            if (trackAnimationflag)
                node["trackAnimation"] = _trackAnimation.ToString();
            if (beatsAheadflag)
                node["beatsAhead"] = _beatsAhead;
            if (trackDisappearAnimationflag)
                node["trackDisappearAnimation"] = _trackDisappearAnimation.ToString();
            if (beatsBehindflag)
                node["beatsBehind"] = _beatsBehind;
            return node;
        }
    }
}
