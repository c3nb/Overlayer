using System.Reflection.Emit;

namespace Overlayer.Tags.Nodes
{
    public abstract class Node
    {
        public abstract void Emit(ILGenerator il);
    }
}
