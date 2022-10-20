using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.Tags.Nodes
{
    public class AccessorNode : Node
    {
        public MemberInfo member;
        public AccessorNode(Node left, string identifier)
        {
            member = left.ResultType.GetMember(identifier, BindingFlags.Public | BindingFlags.Static).FirstOrDefault();
        }
        public override Type ResultType => member is MethodInfo m ? m.ReturnType : member is FieldInfo f ? f.FieldType : member is PropertyInfo p ? p.PropertyType : null;
        public override void Emit(ILGenerator il)
        {
            if (member is MethodInfo m)
                il.Emit(OpCodes.Call, m);
            else if (member is FieldInfo f)
                il.Emit(OpCodes.Ldsfld, f);
            else if (member is PropertyInfo p)
                il.Emit(OpCodes.Call, p.GetGetMethod(true));
            else
            {
                if (ResultType == typeof(string))
                    il.Emit(OpCodes.Ldstr, "");
                else il.Emit(OpCodes.Ldc_R8, 0);
            }
        }
        public static MethodInfo GetOperator(Type t, string op, params Type[] parameters)
            => t.GetMethod("op_" + GetOperatorCode(op), (BindingFlags)15420, null, parameters, null);
        public static string GetOperatorCode(string op)
        {
            switch (op)
            {
                case "==":
                    return "Equality";
                case "!=":
                    return "Inequality";
                case ">":
                    return "GreaterThan";
                case "<":
                    return "LessThan";
                case ">=":
                    return "GreaterThanOrEqual";
                case "<=":
                    return "LessThanOrEqual";
                case "&":
                    return "BitwiseAnd";
                case "|":
                    return "BitwiseOr";
                case "+":
                    return "Addition";
                case "-":
                    return "Subtraction";
                case "/":
                    return "Division";
                case "%":
                    return "Modulus";
                case "*":
                    return "Multiply";
                case "<<":
                    return "LeftShift";
                case ">>":
                    return "RightShift";
                case "^":
                    return "ExclusiveOr";
                case "!":
                    return "LogicalNot";
                case "~":
                    return "OnesComplement";
                case "++":
                    return "Increment";
                case "--":
                    return "Decrement";
                default:
                    return null;
            }
        }
        public static OpCode Conv(Type t)
        {
            if (t == typeof(sbyte))
                return OpCodes.Conv_I1;
            else if (t == typeof(byte))
                return OpCodes.Conv_U1;
            else if (t == typeof(short))
                return OpCodes.Conv_I2;
            else if (t == typeof(ushort))
                return OpCodes.Conv_U2;
            else if (t == typeof(int))
                return OpCodes.Conv_I4;
            else if (t == typeof(uint))
                return OpCodes.Conv_U4;
            else if (t == typeof(long))
                return OpCodes.Conv_I8;
            else if (t == typeof(ulong))
                return OpCodes.Conv_U8;
            else if (t == typeof(float))
                return OpCodes.Conv_R4;
            else if (t == typeof(double))
                return OpCodes.Conv_R8;
            else if (t == typeof(IntPtr))
                return OpCodes.Conv_I;
            else if (t == typeof(UIntPtr))
                return OpCodes.Conv_U;
            return OpCodes.Pop;
        }
    }
}
