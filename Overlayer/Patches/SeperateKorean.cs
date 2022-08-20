using HarmonyLib;
using System.Reflection;
using System.Text;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace Overlayer.Patches
{
    public static class SeperateKorean
    {
        public static IEnumerable<CodeInstruction> Seperator(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> insts = new List<CodeInstruction>(instructions);
            for (int i = 0; i < insts.Count; i++)
            {
                CodeInstruction inst = insts[i];
                if (inst.opcode == OpCodes.Ldstr || 
                    ((inst.opcode == OpCodes.Ldfld || inst.opcode == OpCodes.Ldsfld) && inst.operand is FieldInfo fi && fi.FieldType == typeof(string)) ||
                    ((inst.opcode == OpCodes.Call || inst.opcode == OpCodes.Callvirt) && inst.operand is MethodInfo m && m.ReturnType == typeof(string)))
                {
                    insts.Insert(i + 1, new CodeInstruction(OpCodes.Call, sp));
                    i++;
                    continue;
                }
            }
            return insts;
        }
        public static readonly int[] ChoSung = { 0x3131, 0x3132, 0x3134, 0x3137, 0x3138, 0x3139, 0x3141, 0x3142, 0x3143, 0x3145, 0x3146, 0x3147, 0x3148, 0x3149, 0x314a, 0x314b, 0x314c, 0x314d, 0x314e };
        public static readonly int[] JungSung = { 0x314f, 0x3150, 0x3151, 0x3152, 0x3153, 0x3154, 0x3155, 0x3156, 0x3157, 0x3158, 0x3159, 0x315a, 0x315b, 0x315c, 0x315d, 0x315e, 0x315f, 0x3160, 0x3161, 0x3162, 0x3163 };
        public static readonly int[] JongSung = { 0, 0x3131, 0x3132, 0x3133, 0x3134, 0x3135, 0x3136, 0x3137, 0x3139, 0x313a, 0x313b, 0x313c, 0x313d, 0x313e, 0x313f, 0x3140, 0x3141, 0x3142, 0x3144, 0x3145, 0x3146, 0x3147, 0x3148, 0x314a, 0x314b, 0x314c, 0x314d, 0x314e };
        public static string Seperate(this string data)
        {
            if (data == null)
                return null;
            int a, b, c, x;
            StringBuilder sb = new StringBuilder();
            for (int cnt = 0; cnt < data.Length; cnt++)
            {
                x = data[cnt];
                if (x >= 0xAC00 && x <= 0xD7A3)
                {
                    c = x - 0xAC00;
                    a = c / (21 * 28);
                    c %= 21 * 28;
                    b = c / 28;
                    c %= 28;
                    sb.Append($"{(char)ChoSung[a]}{(char)JungSung[b]}");
                    if (c != 0)
                        sb.Append((char)JongSung[c]);
                }
                else sb.Append((char)x);
            }
            return sb.ToString();
        }
        public static readonly MethodInfo sp = typeof(SeperateKorean).GetMethod(nameof(Seperate));
        public static readonly MethodInfo sptr = typeof(SeperateKorean).GetMethod(nameof(Seperator));
    }
}
