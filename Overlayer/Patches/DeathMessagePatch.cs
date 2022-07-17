using HarmonyLib;
using Overlayer.Tags;
using UnityEngine.UI;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrCountdown), "ShowOverload")]
    public static class DeathMessagePatch
    {
        public static TagCompiler compiler;
        public static void Postfix(Text ___text)
        {
            if (compiler.getter != null)
            {
                ___text.text = compiler.Result;
            }
        }
    }
}
