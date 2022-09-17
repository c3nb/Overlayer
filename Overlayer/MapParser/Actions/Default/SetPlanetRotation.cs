using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions.Default
{
    public class SetPlanetRotation : Action
    {
        public Ease ease = Ease.Linear;
        public int easeParts = 1;
        public EasePartBehavior easePartBehavior = EasePartBehavior.Mirror;
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
            node["ease"] = ease.ToString();
            node["easeParts"] = easeParts;
            node["easePartBehavior"] = easePartBehavior.ToString();
            return node;
        }
    }
}
