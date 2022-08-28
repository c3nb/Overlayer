using System.Reflection.Emit;
using System.Reflection;

namespace TagLib.Tags.Nodes
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
                case '^':
                    opcode = OpCodes.Call;
                    opMeth = CustomTag.functions["pow"][0];
                    return;
                case '\0':
                    opcode = OpCodes.Pop;
                    return;
            }
        }
        public char op;
        public OpCode opcode;
        public MethodInfo opMeth;
        public override void Emit(ILGenerator il)
        {
            if (opMeth != null)
                il.Emit(opcode, opMeth);
            else il.Emit(opcode);
        }
    }
}
