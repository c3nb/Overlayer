using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions.Default
{
    public class SetHitsound : Action
    {
        public GameSound gameSound = GameSound.Hitsound;
        public HitSound hitsound = HitSound.Kick;
        public int hitsoundVolume = 100;
        public SetHitsound() : base(LevelEventType.SetHitsound) { }
        public SetHitsound(GameSound gameSound, HitSound hitsound, int hitsoundVolume, bool active) : base(LevelEventType.SetHitsound, active)
        {
            this.gameSound = gameSound;
            this.hitsound = hitsound;
            this.hitsoundVolume = hitsoundVolume;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            node["gameSound"] = gameSound.ToString();
            node["hitsound"] = hitsound.ToString();
            node["hitsoundVolume"] = hitsoundVolume;
            return node;
        }
    }
}
