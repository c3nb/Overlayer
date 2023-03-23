using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class FreeRoamTwirl : Action
    {
        public Vector2 position
        {
            get => _position;
            set
            {
                positionflag = true;
                _position = value;
            }
        }
        private Vector2 _position = new Vector2(1, 0);
        private bool positionflag;
        public FreeRoamTwirl() : base(LevelEventType.FreeRoamTwirl) { }
        public FreeRoamTwirl(Vector2 position, bool active) : base(LevelEventType.FreeRoamTwirl, active)
        {
            this.position = position;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (positionflag)
                node["position"] = _position.ToNode();
            return node;
        }
    }
}
