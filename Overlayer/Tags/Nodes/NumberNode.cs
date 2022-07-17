using System.Reflection.Emit;

namespace Overlayer.Tags.Nodes
{
    public class NumberNode : Node
    {
        public NumberNode(float number)
            => this.number = number;
        public float number;
        public override void Emit(ILGenerator il)
            => il.Emit(OpCodes.Ldc_R4, number);
        public override string ToString() => number.ToString();
    }
}
