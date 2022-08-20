using HarmonyLib;
using Overlayer.Tags;
using Overlayer.Tags.Global;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "FailAction")]
    public static class DeathMessagePatch
    {
        public static TagCompiler compiler;
        public static void Prefix(scrController __instance)
            => ProgressDeath.Increment(__instance.percentComplete * 100);
        public static void Postfix(scrController __instance)
        {
            if (compiler.getter != null)
                __instance.txtTryCalibrating.text = compiler.Result;
        }
    }
}
