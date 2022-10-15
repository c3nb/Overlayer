using HarmonyLib;
using Overlayer.Core;
using Overlayer.Tags.Global;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
    public static class ClearMessagePatch
    {
        public static TextCompiler compiler;
        public static void Postfix(scrController __instance)
        {
            if (!__instance.noFail && compiler.getter != null)
                __instance.txtCongrats.text = compiler.Result;
        }
    }
}
