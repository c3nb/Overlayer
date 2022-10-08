using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Overlayer.Core.Tags.Nodes
{
    public class OperatorNode : Node
    {
        public override Type ResultType { get => rt; }
        internal Type rt;
        bool _(char c, bool isString)
        {
            if (isString)
                rt = typeof(string);
            else rt = typeof(float);
            switch (c)
            {
                case '+':
                    if (isString)
                    {
                        opcode = OpCodes.Call;
                        opMeth = sAdd;
                        return true;
                    }
                    opcode = OpCodes.Add;
                    return true;
                case '-':
                    if (isString)
                    {
                        opcode = OpCodes.Call;
                        opMeth = sSub;
                        return true;
                    }
                    opcode = OpCodes.Sub;
                    return true;
                case '*':
                    if (isString)
                    {
                        opcode = OpCodes.Pop;
                        return true;
                    }
                    opcode = OpCodes.Mul;
                    return true;
                case '/':
                    if (isString)
                    {
                        opcode = OpCodes.Pop;
                        return true;
                    }
                    opcode = OpCodes.Div;
                    return true;
                case '%':
                    if (isString)
                    {
                        opcode = OpCodes.Pop;
                        return true;
                    }
                    opcode = OpCodes.Rem;
                    return true;
                case '^':
                    if (isString)
                    {
                        opcode = OpCodes.Pop;
                        return true;
                    }
                    opcode = OpCodes.Call;
                    opMeth = CustomTag.functions["pow"][0];
                    return true;
                case '\0':
                    opcode = OpCodes.Pop;
                    return true;
            }
            return false;
        }
        public OperatorNode(char op, bool isString)
        {
            this.op = op.ToString();
            _(op, isString);
        }
        public OperatorNode(string op, bool isString)
        {
            if (_(op[0], isString))
                return;
            this.op = op;
            rt = typeof(bool);
            switch (op)
            {
                case "==":
                    if (isString)
                    {
                        opcode = OpCodes.Call;
                        opMeth = sEq;
                        return;
                    }
                    opcode = OpCodes.Ceq;
                    return;
                case "!=":
                    if (isString)
                    {
                        opcode = OpCodes.Call;
                        opMeth = sIne;
                        return;
                    }
                    opcode = OpCodes.Call;
                    opMeth = ine;
                    return;
                case "&&":
                    if (isString)
                    {
                        opcode = OpCodes.Call;
                        opMeth = sAnd;
                        return;
                    }
                    opcode = OpCodes.And;
                    return;
                case "||":
                    if (isString)
                    {
                        opcode = OpCodes.Call;
                        opMeth = sOr;
                        return;
                    }
                    opcode = OpCodes.Or;
                    return;
                case ">":
                    if (isString)
                    {
                        opcode = OpCodes.Call;
                        opMeth = sGt;
                        return;
                    }
                    opcode = OpCodes.Cgt;
                    return;
                case "<":
                    if (isString)
                    {
                        opcode = OpCodes.Call;
                        opMeth = sLt;
                        return;
                    }
                    opcode = OpCodes.Clt;
                    return;
                case ">=":
                    if (isString)
                    {
                        opcode = OpCodes.Call;
                        opMeth = sGe;
                        return;
                    }
                    opcode = OpCodes.Call;
                    opMeth = cge;
                    return;
                case "<=":
                    if (isString)
                    {
                        opcode = OpCodes.Call;
                        opMeth = sLe;
                        return;
                    }
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

        public static readonly MethodInfo sEq = typeof(OperatorNode).GetMethod(nameof(Seq));
        public static readonly MethodInfo sIne = typeof(OperatorNode).GetMethod(nameof(Sine));
        public static readonly MethodInfo sLt = typeof(OperatorNode).GetMethod(nameof(Slt));
        public static readonly MethodInfo sGt = typeof(OperatorNode).GetMethod(nameof(Sgt));
        public static readonly MethodInfo sLe = typeof(OperatorNode).GetMethod(nameof(Sle));
        public static readonly MethodInfo sGe = typeof(OperatorNode).GetMethod(nameof(Sge));
        public static readonly MethodInfo sAnd = typeof(OperatorNode).GetMethod(nameof(Sand));
        public static readonly MethodInfo sOr = typeof(OperatorNode).GetMethod(nameof(Sor));

        public static readonly MethodInfo sAdd = typeof(OperatorNode).GetMethod(nameof(Sadd));
        public static readonly MethodInfo sSub = typeof(OperatorNode).GetMethod(nameof(Ssub));

        public static bool Seq(string a, string b) => a == b;
        public static bool Sine(string a, string b) => a != b;
        public static bool Slt(string a, string b) => a.Length < b.Length;
        public static bool Sgt(string a, string b) => a.Length > b.Length;
        public static bool Sle(string a, string b) => a.Length <= b.Length;
        public static bool Sge(string a, string b) => a.Length >= b.Length;
        public static bool Sand(string a, string b) => !string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b);
        public static bool Sor(string a, string b) => !string.IsNullOrEmpty(a) || !string.IsNullOrEmpty(b);

        public static string Sadd(string a, string b) => a + b;
        public static string Ssub(string a, string b) => a.Replace(b, string.Empty);
    }
}
