using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class SetPlanetRotation : Action
    {
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
        public int easeParts
        {
            get => _easeParts;
            set
            {
                easePartsflag = true;
                _easeParts = value;
            }
        }
        private int _easeParts = 1;
        private bool easePartsflag;
        public EasePartBehavior easePartBehavior
        {
            get => _easePartBehavior;
            set
            {
                easePartBehaviorflag = true;
                _easePartBehavior = value;
            }
        }
        private EasePartBehavior _easePartBehavior = EasePartBehavior.Mirror;
        private bool easePartBehaviorflag;
        public SetPlanetRotation() : base(LevelEventType.SetPlanetRotation) { }
        public SetPlanetRotation(Ease ease, int easeParts, EasePartBehavior easePartBehavior, bool active) : base(LevelEventType.SetPlanetRotation, active)
        {
            this.ease = ease;
            this.easeParts = easeParts;
            this.easePartBehavior = easePartBehavior;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (easeflag)
                node["ease"] = _ease.ToString();
            if (easePartsflag)
                node["easeParts"] = _easeParts;
            if (easePartBehaviorflag)
                node["easePartBehavior"] = _easePartBehavior.ToString();
            return node;
        }
    }
}
