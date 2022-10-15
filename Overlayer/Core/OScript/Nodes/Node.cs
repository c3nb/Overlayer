using System;
using System.Reflection.Emit;

namespace Overlayer.Core.OScript.Nodes
{
    public abstract class Node
    {
        public abstract Type ResultType { get; }
        public abstract void Emit(ILGenerator il);
    }
}
