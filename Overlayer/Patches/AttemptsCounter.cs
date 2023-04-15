using HarmonyExLib;
using System;
using System.Collections.Generic;
using Overlayer.Core.Tags;
using Overlayer.Tags;

namespace Overlayer.Patches
{
    [HarmonyExPatch]
    public static class AttemptsCounter
    {
        public static Dictionary<string, int> Attempts = new Dictionary<string, int>();
        [HarmonyExPatch(typeof(CustomLevel), "FinishCustomLevelLoading")]
        [HarmonyExPostfix]
        public static void FCLLPostfix()
        {
            if (!GCS.useNoFail)
            {
                if (PlaytimeCounter.MapID == null)
                    PlaytimeCounter.MapID = DataInit.MakeHash(RDString.Get("editor.author"), RDString.Get("editor.artist"), RDString.Get("editor.song"));
                if (!Attempts.TryGetValue(PlaytimeCounter.MapID, out _))
                    Attempts[PlaytimeCounter.MapID] = Persistence.GetCustomWorldAttempts(PlaytimeCounter.MapID);
                else Attempts[PlaytimeCounter.MapID]++;
                Variables.AttemptsCount = Attempts[PlaytimeCounter.MapID];
                Persistence.IncrementCustomWorldAttempts(PlaytimeCounter.MapID);
            }
        }
        [HarmonyExPatch(typeof(scrController), "Start")]
        public static void Postfix(scrController __instance)
        {
            if (ADOBase.sceneName.Contains("-") && !__instance.noFail)
                Variables.AttemptsCount = Persistence.GetWorldAttempts(scrController.currentWorld);
        }
        [HarmonyExPatch(typeof(scrController), "FailAction")]
        [HarmonyExPostfix]
        public static void FAPostfix(scrController __instance)
        {
            Variables.FailCount++;
            Variables.BestProg = Math.Max(Variables.BestProg, __instance.percentComplete * 100);
        }
    }
}
