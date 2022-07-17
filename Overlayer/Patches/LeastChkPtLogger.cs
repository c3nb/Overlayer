using HarmonyLib;
using System;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
    public static class LeastChkPtLogger
    {
        public static void Postfix(scrController __instance)
        {
            if (CustomLevel.instance)
                Variables.LeastChkPt = Math.Min(Variables.LeastChkPt, scrController.checkpointsUsed);
        }
    }
}
