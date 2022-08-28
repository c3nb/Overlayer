using System.Reflection.Emit;

namespace TagLib.Tags.Nodes
{
    public abstract class Node
    {
        public abstract void Emit(ILGenerator il);
    }
}
