using System.Reflection.Emit;

namespace TagLib.Tags.Nodes
{
    public class UnaryNode : Node
    {
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
