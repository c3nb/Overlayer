using System;
using System.Reflection.Emit;
using Overlayer.Core.Nodes;

namespace Overlayer.Core.Nodes
{
    public class BinaryNode : Node
    {
        public Node left;
        public OperatorNode op;
        public Node right;
        public override Type ResultType => op.ResultType;
        public BinaryNode(Node left, OperatorNode op, Node right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
        public override void Emit(ILGenerator il)
        {
            left.Emit(il);
            right.Emit(il);
            op.Emit(il);
        }
    }
}
