using System.Reflection.Emit;

namespace Overlayer.Core.TextReplacing.Parsing
{
    public interface IParsed
    {
        void Emit(ILGenerator il);
    }
}
