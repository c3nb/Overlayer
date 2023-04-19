using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdofaiMapConverter.Actions;
using AdofaiMapConverter.Types;
using Action = AdofaiMapConverter.Actions.Action;

namespace AdofaiMapConverter.Decorations
{
    public abstract class Decoration : Action
    {
        public bool visible = true;
        public int floor;
        public Decoration(LevelEventType eventType, bool visible) : base(eventType, visible)
            => this.visible = visible;
        public Decoration(LevelEventType eventType) : base(eventType, true) 
            => visible = true;
        public new Decoration Copy() => DecorationUtils.ParseDecoration(ToNode());
        public new object Clone() => Copy();
    }
}
