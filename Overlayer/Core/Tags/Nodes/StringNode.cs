using Overlayer.Core.Nodes;
using System;
using System.Reflection.Emit;

namespace Overlayer.Core.Tags.Nodes
{
    public class StringNode : Node
    {
        public override Type ResultType => typeof(string);
        public StringNode(string str)
            => this.str = str;
        public string str;
        public override void Emit(ILGenerator il)
            => il.Emit(OpCodes.Ldstr, str);
        public override string ToString() => str.ToString();
    }
}
