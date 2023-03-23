using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdofaiMapConverter.Types;
using JSON;

namespace AdofaiMapConverter.Actions
{
    public class UnknownAction : Action
    {
        private JsonNode raw;
        public UnknownAction() : base(LevelEventType.None) { }
        public UnknownAction(JsonNode raw) : this() => this.raw = raw;
        public override JsonNode ToNode() => raw;
    }
}
