using HarmonyEx;
using System;
using System.Collections.Generic;
using Overlayer.Core.Tags;

namespace Overlayer.Patches
{
    [HarmonyPatch]
    public static class AttemptsCounter
    {
        public static Dictionary<string, int> Attempts = new Dictionary<string, int>();
        [HarmonyPatch(typeof(CustomLevel), "FinishCustomLevelLoading")]
        [HarmonyPostfix]
        public static void FCLLPostfix()
        {
            if (!GCS.standaloneLevelMode && !GCS.useNoFail)
            {
                if (PlaytimeCounter.MapID == null)
                    PlaytimeCounter.MapID = DataInit.MakeHash(RDString.Get("editor.author"), RDString.Get("editor.artist"), RDString.Get("editor.song"));
                if (PlaytimeCounter.MapID == null || !Attempts.TryGetValue(PlaytimeCounter.MapID, out _))
                    Attempts[PlaytimeCounter.MapID] = Persistence.GetCustomWorldAttempts(PlaytimeCounter.MapID);
                else Attempts[PlaytimeCounter.MapID]++;
                AttemptsCount = Attempts[PlaytimeCounter.MapID];
                Persistence.IncrementCustomWorldAttempts(PlaytimeCounter.MapID);
            }
        }
        [HarmonyPatch(typeof(scrController), "Start")]
        public static void Postfix(scrController __instance)
        {
            if (ADOBase.sceneName.Contains("-") && !__instance.noFail)
                AttemptsCount = Persistence.GetWorldAttempts(scrController.currentWorld);
        }
        [HarmonyPatch(typeof(scrController), "FailAction")]
        [HarmonyPostfix]
        public static void FAPostfix(scrController __instance)
        {
            FailCount++;
            BestProg = Math.Max(BestProg, __instance.percentComplete * 100);
        }
    }
}
