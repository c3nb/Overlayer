using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Core.Tags.Nodes
{
    public class ArgumentNode : Node
    {
        public ArgumentNode(int index, Type type)
        {
            argIndex = index;
            argType = type;
        }
        public Type argType;
        public int argIndex;
        public override Type ResultType => argType;
        public override void Emit(ILGenerator il) => il.Emit(OpCodes.Ldarg, argIndex);
    }
}
