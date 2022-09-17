using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;

namespace TagLib.Tags.Nodes
{
    public class OperatorNode : Node
    {
        public OperatorNode(char op)
        {
            this.op = op.ToString();
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
        public OperatorNode(string op)
        {
            this.op = op;
            switch (op)
            {
                case "==":
                    opcode = OpCodes.Ceq;
                    return;
                case "!=":
                    opcode = OpCodes.Call;
                    opMeth = ine;
                    return;
                case "&&":
                    opcode = OpCodes.And;
                    return;
                case "||":
                    opcode = OpCodes.Or;
                    return;
                case ">":
                    opcode = OpCodes.Cgt;
                    return;
                case "<":
                    opcode = OpCodes.Clt;
                    return;
                case ">=":
                    opcode = OpCodes.Call;
                    opMeth = cge;
                    return;
                case "<=":
                    opcode = OpCodes.Call;
                    opMeth = cle;
                    return;
            }
        }
        public string op;
        public OpCode opcode;
        public MethodInfo opMeth;
        public override void Emit(ILGenerator il)
        {
            if (opMeth != null)
                il.Emit(opcode, opMeth);
            else il.Emit(opcode);
        }
        public static readonly MethodInfo ine = typeof(OperatorNode).GetMethod(nameof(Ine));
        public static readonly MethodInfo cge = typeof(OperatorNode).GetMethod(nameof(Cge));
        public static readonly MethodInfo cle = typeof(OperatorNode).GetMethod(nameof(Cle));
        public static bool Ine(float a, float b) => a != b;
        public static bool Cge(float a, float b) => a >= b;
        public static bool Cle(float a, float b) => a <= b;
    }
}
