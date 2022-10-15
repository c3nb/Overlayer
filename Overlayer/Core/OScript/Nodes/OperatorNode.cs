using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Overlayer.Core.OScript.Nodes
{
    public class OperatorNode : Node
    {
        public override Type ResultType => rt;
        internal Type leftType;
        internal Type rightType;
        internal string op;
        internal Type rt;
        internal Action<ILGenerator> emitter;
        public OperatorNode(Node left, string op, Node right)
        {
            leftType = left.ResultType;
            rightType = right.ResultType;
            this.op = op;
            switch (op)
            {
                case "==":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Ceq);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Ceq);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                }
                            }
                        }
                        rt = typeof(bool);
                        break;
                    }
                case "!=":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Ceq);
                                    il.Emit(OpCodes.Not);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Ceq);
                                    il.Emit(OpCodes.Not);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                        il.Emit(OpCodes.Not);
                                    };
                                }
                            }
                        }
                        rt = typeof(bool);
                        break;
                    }
                case ">":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Cgt);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Cgt);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                }
                            }
                        }
                        rt = typeof(bool);
                        break;
                    }
                case "<":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Clt);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Clt);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                }
                            }
                        }
                        rt = typeof(bool);
                        break;
                    }
                case ">=":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Cgt);
                                    il.Emit(OpCodes.Ceq);
                                    il.Emit(OpCodes.Or);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Cgt);
                                    il.Emit(OpCodes.Ceq);
                                    il.Emit(OpCodes.Or);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                }
                            }
                        }
                        rt = typeof(bool);
                        break;
                    }
                case "<=":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Clt);
                                    il.Emit(OpCodes.Ceq);
                                    il.Emit(OpCodes.Or);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Clt);
                                    il.Emit(OpCodes.Ceq);
                                    il.Emit(OpCodes.Or);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                }
                            }
                        }
                        rt = typeof(bool);
                        break;
                    }
                case "&":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.And);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.And);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "|":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Or);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Or);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "+":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Add);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Add);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "-":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Sub);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Sub);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "/":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Div);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Div);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "%":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Rem);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Rem);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "*":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Mul);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Mul);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "<<":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Shl);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Shl);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case ">>":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Shr);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Shr);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "^":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Xor);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Xor);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "!":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Not);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Not);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                }
                            }
                        }
                        rt = typeof(bool);
                        break;
                    }
                case "~":
                    {
                        if (leftType.IsPrimitive && rightType.IsPrimitive)
                        {
                            if (left != right)
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(Conv(rightType));
                                    il.Emit(OpCodes.Not);
                                };
                            else
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    right.Emit(il);
                                    il.Emit(OpCodes.Not);
                                };
                            }
                        }
                        else
                        {
                            if (!leftType.IsPrimitive)
                            {
                                var meth = GetOperator(leftType, op, leftType, rightType);
                                if (meth != null)
                                {
                                    emitter = il =>
                                    {
                                        left.Emit(il);
                                        right.Emit(il);
                                        il.Emit(OpCodes.Call, meth);
                                    };
                                    rt = meth.ReturnType;
                                    break;
                                }
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "++":
                    {
                        if (leftType.IsPrimitive)
                        {
                            emitter = il =>
                            {
                                left.Emit(il);
                                il.Emit(OpCodes.Ldc_I4_1);
                                il.Emit(Conv(leftType));
                                il.Emit(OpCodes.Add);
                            };
                        }
                        else
                        {
                            var meth = GetOperator(leftType, op, leftType, rightType);
                            if (meth != null)
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    il.Emit(OpCodes.Call, meth);
                                };
                                rt = meth.ReturnType;
                                break;
                            }
                        }
                        rt = leftType;
                        break;
                    }
                case "--":
                    {
                        if (leftType.IsPrimitive)
                        {
                            emitter = il =>
                            {
                                left.Emit(il);
                                il.Emit(OpCodes.Ldc_I4_1);
                                il.Emit(Conv(leftType));
                                il.Emit(OpCodes.Sub);
                            };
                        }
                        else
                        {
                            var meth = GetOperator(leftType, op, leftType, rightType);
                            if (meth != null)
                            {
                                emitter = il =>
                                {
                                    left.Emit(il);
                                    il.Emit(OpCodes.Call, meth);
                                };
                                rt = meth.ReturnType;
                                break;
                            }
                        }
                        rt = leftType;
                        break;
                    }
                default:
                    break;
            }
        }
        public override void Emit(ILGenerator il)
        {
            if (emitter == null)
                throw new InvalidOperationException($"Invalid Operator ({leftType} {op} {rightType})");
            emitter(il);
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
        public static bool IsPrimitiveEx(Type type)
            => type.IsPrimitive || type == typeof(string);
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
