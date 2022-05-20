using System;
using HarmonyLib;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
    public static class LeastChkPtLogger
    {
        public static void Postfix(scrController __instance)
        {
            if (__instance.customLevel)
                Variables.LeastChkPt = Math.Min(Variables.LeastChkPt, __instance.customLevel.checkpointsUsed);
        }
    }
}
