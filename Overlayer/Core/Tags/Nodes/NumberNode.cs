using System;
using System.Reflection.Emit;

namespace Overlayer.Core.Tags.Nodes
{
    public class NumberNode : Node
    {
        public override Type ResultType => typeof(double);
        public NumberNode(double number)
            => this.number = number;
        public double number;
        public override void Emit(ILGenerator il)
            => il.Emit(OpCodes.Ldc_R8, number);
        public override string ToString() => number.ToString();
    }
}
