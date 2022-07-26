using HarmonyLib;
using Overlayer.Tags;
using UnityEngine.UI;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "FailAction")]
    public static class DeathMessagePatch
    {
        public static TagCompiler compiler;
        public static void Postfix(scrController __instance)
        {
            if (compiler.getter != null)
            {
                __instance.txtTryCalibrating.text = compiler.Result;
            }
        }
    }
}
