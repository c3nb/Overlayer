using System;
using System.Reflection.Emit;

namespace Overlayer.Core.Nodes
{
    public class UnaryNode : Node
    {
        public override Type ResultType => typeof(float);
        public Node node;
        public UnaryNode(Node node)
        {
            this.node = node;
        }
        public override void Emit(ILGenerator il)
        {
            node.Emit(il);
            il.Emit(OpCodes.Neg);
        }
    }
}
