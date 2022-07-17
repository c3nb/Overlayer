using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace Overlayer.Tags.Nodes
{
    public class Operator
    {
        public static readonly Operator Add = new Operator('+');
        public static readonly Operator Sub = new Operator('-');
        public static readonly Operator Mul = new Operator('*');
        public static readonly Operator Div = new Operator('/');
        public static readonly Operator Rem = new Operator('%');
        public readonly OpCode OpCode;
        public readonly char OpChar;
        Operator(char opChar)
        {
            OpChar = opChar;
            switch (opChar)
            {
                case '+':
                    OpCode = OpCodes.Add;
                    return;
                case '-':
                    OpCode = OpCodes.Sub;
                    return;
                case '*':
                    OpCode = OpCodes.Mul;
                    return;
                case '/':
                    OpCode = OpCodes.Div;
                    return;
                case '%':
                    OpCode = OpCodes.Rem;
                    return;
            }
        }
        public int UnaryPrecedence
        {
            get
            {
                switch (OpChar)
                {
                    case '+':
                    case '-':
                        return 3;
                    default:
                        return 0;
                }
            }
        }
        public int BinaryPrecedence
        {
            get
            {
                switch (OpChar)
                {
                    case '*':
                    case '/':
                    case '%':
                        return 2;
                    case '+':
                    case '-':
                        return 1;
                    default:
                        return 0;
                }
            }
        }
    }
}
