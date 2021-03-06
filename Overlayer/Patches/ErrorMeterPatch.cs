using HarmonyLib;
using UnityEngine;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrHitErrorMeter))]
    public static class ErrorMeterPatch
    {
        static bool ForceCall = false;
        static float QueuedAngle = 0f;
        public static int[] Queued = new int[10];
        static readonly int Multipress = (int)HitMargin.Multipress;
        static readonly int FailMiss = (int)HitMargin.FailMiss;
        static readonly int FailOverload = (int)HitMargin.FailOverload;
        [HarmonyPatch("AddHit")]
        [HarmonyPrefix]
        public static bool AHPrefix(float angleDiff)
        {
            if (ForceCall) return true;
            QueuedAngle = angleDiff;
            return false;
        }
        [HarmonyPatch("CalculateTickColor")]
        [HarmonyPostfix]
        public static void CTCPostfix(ref Color __result)
        {
            if (Settings.Instance.AddAllJudgementsAtErrorMeter)
            {
                if (Queued[Multipress] > 0)
                {
                    __result = RDConstants.data.hitMarginColours.colourMultipress;
                    Queued[Multipress]--;
                }
                else if (Queued[FailMiss] > 0)
                {
                    __result = RDConstants.data.hitMarginColours.colourFail;
                    Queued[FailMiss]--;
                }
                else if (Queued[FailOverload] > 0)
                {
                    __result = RDConstants.data.hitMarginColours.colourFail;
                    Queued[FailOverload]--;
                }
            }
        }
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void APrefix(scrHitErrorMeter __instance)
        {
            __instance.tickLife = Settings.Instance.ErrorMeterTickLife;
            __instance.tickCacheSize = Settings.Instance.ErrorMeterHitImages;
        }
        [HarmonyPatch(typeof(scrController), "ShowHitText")]
        [HarmonyPostfix]
        public static void SHTPostfix(scrController __instance, HitMargin hitMargin)
        {
            if (hitMargin == HitMargin.Multipress)
                Variables.MultipressCount++;
            Queued[(int)hitMargin]++;
            ForceCall = true;
            __instance.errorMeter.AddHit(QueuedAngle);
            ForceCall = false;
        }
    }
}
