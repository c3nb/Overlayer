using System.Reflection.Emit;

namespace Overlayer.Tags.Nodes
{
    public class OperatorNode : Node
    {
        public OperatorNode(char op)
        {
            this.op = op;
            switch (op)
            {
                case '+':
                    opcode = OpCodes.Add;
                    return;
                case '-':
                    opcode = OpCodes.Sub;
                    return;
                case '*':
                    opcode = OpCodes.Mul;
                    return;
                case '/':
                    opcode = OpCodes.Div;
                    return;
                case '%':
                    opcode = OpCodes.Rem;
                    return;
                case '\0':
                    opcode = OpCodes.Pop;
                    return;
            }
        }
        public char op;
        public OpCode opcode;
        public override void Emit(ILGenerator il)
            => il.Emit(opcode);
    }
}
