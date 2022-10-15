using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.Tags.Nodes
{
    public class CustomNode : Node
    {
        public CustomNode(Type rType, Action<ILGenerator> emitter = null)
        {
            RType = rType;
            Emitter = emitter;
        }
        public Type RType;
        public Action<ILGenerator> Emitter;
        public override Type ResultType => RType;
        public override void Emit(ILGenerator il) => Emitter(il);
    }
}
