using System;
using HarmonyLib;
using System.Collections.Generic;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController))]
    public static class AttemptsCounter
    {
        public static Dictionary<string, int> Attempts = new Dictionary<string, int>();
        public static string FailId = string.Empty;
        [HarmonyPatch("FailAction")]
        public static void Postfix(scrController __instance)
        {
            if (!ADOBase.sceneName.Contains("-") && !__instance.noFail && !RDC.auto)
            {
                Attempts[FailId]++;
                Variables.FailCount++;
                Variables.BestProg = Math.Max(Variables.BestProg, __instance.controller.percentComplete * 100);
                Variables.Attempts = Attempts[FailId];
                if (!GCS.standaloneLevelMode)
                    Persistence.IncrementCustomWorldAttempts(FailId);
            }
        }
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(scrController __instance)
        {
            if (ADOBase.sceneName.Contains("-") && !__instance.noFail && !RDC.auto)
                Variables.Attempts = Persistence.GetWorldAttempts(scrController.currentWorld);
        }
    }
}
