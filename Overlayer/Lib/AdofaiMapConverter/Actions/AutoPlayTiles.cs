using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class AutoPlayTiles : Action
    {
        public Toggle enabled
        {
            get => _enabled;
            set
            {
                enabledflag = true;
                _enabled = value;
            }
        }
        private Toggle _enabled = Toggle.Enabled;
        private bool enabledflag;
        public Toggle safetyTiles
        {
            get => _safetyTiles;
            set
            {
                safetyTilesflag = true;
                _safetyTiles = value;
            }
        }
        private Toggle _safetyTiles = Toggle.Disabled;
        private bool safetyTilesflag;
        public AutoPlayTiles() : base(LevelEventType.AutoPlayTiles) { }
        public AutoPlayTiles(Toggle enabled, Toggle safetyTiles, bool active) : base(LevelEventType.AutoPlayTiles, active)
        {
            this.enabled = enabled;
            this.safetyTiles = safetyTiles;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (enabledflag)
                node["enabled"] = _enabled.ToString();
            if (safetyTilesflag)
                node["safetyTiles"] = _safetyTiles.ToString();
            return node;
        }
    }
}
