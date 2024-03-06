using System.Reflection.Emit;

namespace Overlayer.Core.TextReplacing.Parsing
{
    public class ParsedString : IParsed
    {
        public string str;
        public ParsedString(string str)
        {
            this.str = str;
        }
        public void Emit(ILGenerator il)
        {
            il.Emit(OpCodes.Ldstr, str);
        }
    }
}
