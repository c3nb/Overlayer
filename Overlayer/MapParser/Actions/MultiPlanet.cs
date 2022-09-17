using Overlayer.MapParser.Types;
using JSON;
using static Overlayer.MapParser.Actions.ActionUtils;

namespace Overlayer.MapParser.Actions
{
    public class MultiPlanet : Action
    {
        public int planets
        {
            get => _planets;
            set
            {
                planetsflag = true;
                _planets = value;
            }
        }
        private int _planets = 2;
        private bool planetsflag;
        public MultiPlanet() : base(LevelEventType.MultiPlanet) { }
        public MultiPlanet(int planets, bool active) : base(LevelEventType.MultiPlanet, active)
        {
            this.planets = planets;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (planetsflag)
                node["planets"] = _planets;
            return node;
        }
    }
}
