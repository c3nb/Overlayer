using System;
using System.Reflection;
using System.Reflection.Emit;
using Overlayer.Core.Utils;

namespace Overlayer.Core.Tags.Nodes
{
    public class FunctionNode : Node
    {
        public override Type ResultType => method.ReturnType;
        public MethodInfo method;
        public Node[] arguments;
        public FunctionNode(MethodInfo method, Node[] arguments)
        {
            this.method = method;
            this.arguments = arguments;
        }
        public override void Emit(ILGenerator il)
        {
            if (method is DynamicMethod)
            {
                var name = method.Name;
                if (IntPtr.Size == 4)
                {
                    int addr = name.Split('_')[1].ToInt();
                    il.Emit(OpCodes.Ldc_I4, addr);
                }
                else
                {
                    long addr = name.Split('_')[1].ToLong();
                    il.Emit(OpCodes.Ldc_I8, addr);
                }
                il.Emit(OpCodes.Ldobj, typeof(Tag[]));
            }
            for (int i = 0; i < arguments.Length; i++)
                arguments[i].Emit(il);
            il.Emit(OpCodes.Call, method);
        }
    }
}
