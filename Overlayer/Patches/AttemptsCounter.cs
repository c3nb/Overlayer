using HarmonyLib;
using System;
using TagLib.Tags;
using System.Collections.Generic;
using Overlayer.Tags;

namespace Overlayer.Patches
{
    [HarmonyPatch]
    public static class AttemptsCounter
    {
        public static Dictionary<string, int> Attempts = new Dictionary<string, int>();
        public static string FailId = string.Empty;
        [HarmonyPatch(typeof(CustomLevel), "FinishCustomLevelLoading")]
        [HarmonyPostfix]
        public static void FCLLPostfix()
        {
            if (!TextCompiler.IsReferencing("Attempts"))
                return;
            if (!GCS.standaloneLevelMode && !GCS.useNoFail)
            {
                if (FailId == null || !Attempts.TryGetValue(FailId, out _))
                {
                    FailId = DataInit.MakeHash(RDString.Get("editor.author"), RDString.Get("editor.artist"), RDString.Get("editor.song"));
                    Attempts[FailId] = Persistence.GetCustomWorldAttempts(FailId);
                }
                else Attempts[FailId]++;
                Variables.Attempts = Attempts[FailId];
                Persistence.IncrementCustomWorldAttempts(FailId);
            }
        }
        [HarmonyPatch(typeof(scrController), "Start")]
        public static void Postfix(scrController __instance)
        {
            if (!TextCompiler.IsReferencing("Attempts"))
                return;
            if (ADOBase.sceneName.Contains("-") && !__instance.noFail)
                Variables.Attempts = Persistence.GetWorldAttempts(scrController.currentWorld);
        }
        [HarmonyPatch(typeof(scrController), "FailAction")]
        [HarmonyPostfix]
        public static void FAPostfix(scrController __instance)
        {
            Variables.FailCount++;
            Variables.BestProg = Math.Max(Variables.BestProg, __instance.percentComplete * 100);
        }
    }
}
