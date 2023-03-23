using AdofaiMapConverter.Types;
using JSON;
using static AdofaiMapConverter.Actions.ActionUtils;

namespace AdofaiMapConverter.Actions
{
    public class FreeRoamRemove : Action
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
        public Vector2 size
        {
            get => _size;
            set
            {
                sizeflag = true;
                _size = value;
            }
        }
        private Vector2 _size = Vector2.One;
        private bool sizeflag;
        public FreeRoamRemove() : base(LevelEventType.FreeRoamRemove) { }
        public FreeRoamRemove(Vector2 position, Vector2 size, bool active) : base(LevelEventType.FreeRoamRemove, active)
        {
            this.position = position;
            this.size = size;
        }
        public override JsonNode ToNode()
        {
            JsonNode node = InitNode(eventType, active);
            if (positionflag)
                node["position"] = _position.ToNode();
            if (sizeflag)
                node["size"] = _size.ToNode();
            return node;
        }
    }
}
