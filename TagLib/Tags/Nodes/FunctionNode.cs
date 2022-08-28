using System.Reflection;
using System.Reflection.Emit;

namespace TagLib.Tags.Nodes
{
    public class FunctionNode : Node
    {
        public MethodInfo method;
        public Node[] arguments;
        public FunctionNode(MethodInfo method, Node[] arguments)
        {
            this.method = method;
            this.arguments = arguments;
        }
        public override void Emit(ILGenerator il)
        {
            for (int i = 0; i < arguments.Length; i++)
                arguments[i].Emit(il);
            il.Emit(OpCodes.Call, method);
        }
    }
}
