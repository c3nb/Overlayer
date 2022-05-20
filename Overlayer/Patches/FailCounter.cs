using System;
using HarmonyLib;
using System.Collections.Generic;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "FailAction")]
    public static class FailCounter
    {
        public static Dictionary<string, int> Attempts = new Dictionary<string, int>();
        public static string FailId = string.Empty;
        public static void Postfix(scrController __instance)
        {
            if (!__instance.noFail && !RDC.auto)
            {
                Attempts[FailId]++;
                Variables.FailCount++;
                Variables.BestProg = Math.Max(Variables.BestProg, __instance.controller.percentComplete * 100);
                Variables.Attempts = Attempts[FailId];
                if (!GCS.standaloneLevelMode)
                    Persistence.IncrementCustomWorldAttempts(FailId);
            }
        }
    }
}
