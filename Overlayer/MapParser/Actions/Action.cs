using Overlayer.MapParser.Types;
using JSON;

namespace Overlayer.MapParser.Actions
{
    public abstract class Action : System.ICloneable
    {
        public Action(LevelEventType eventType) : this(eventType, true) { }
        public Action(LevelEventType eventType, bool active)
        {
            this.eventType = eventType;
            this.active = active;
        }
        public LevelEventType eventType;
        public bool active = true;
        public abstract JsonNode ToNode();
        public Action Copy() => ActionUtils.ParseAction(ToNode());
        public object Clone() => Copy();
    }
}
