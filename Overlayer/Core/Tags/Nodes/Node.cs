using System.Reflection.Emit;
using System;

namespace Overlayer.Core.Tags.Nodes
{
    public abstract class Node
    {
        public abstract Type ResultType { get; }
        public abstract void Emit(ILGenerator il);
    }
}
