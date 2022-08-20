using System;
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
            if (!Settings.Instance.AddAllJudgementsAtErrorMeter) return true;
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
            Queued = new int[10];
        }
        [HarmonyPatch(typeof(scrController), "ShowHitText")]
        [HarmonyPostfix]
        public static void SHTPostfix(scrController __instance, HitMargin hitMargin)
        {
            if (hitMargin == HitMargin.Multipress)
                Variables.MultipressCount++;
            if (!Settings.Instance.AddAllJudgementsAtErrorMeter)
                return;
            Queued[(int)hitMargin]++;
            ForceCall = true;
            if (r92 != null)
                r92(__instance.errorMeter, QueuedAngle);
            else if (r94 != null)
                r94(__instance.errorMeter, QueuedAngle, 1);
            ForceCall = false;
        }
        public static readonly Action<scrHitErrorMeter, float, float> r94 = (Action<scrHitErrorMeter, float, float>)typeof(scrHitErrorMeter).GetMethod("AddHit", AccessTools.all, null, new[] { typeof(float), typeof(float) }, null)?.CreateDelegate(typeof(Action<scrHitErrorMeter, float, float>));
        public static readonly Action<scrHitErrorMeter, float> r92 = (Action<scrHitErrorMeter, float>)typeof(scrHitErrorMeter).GetMethod("AddHit", AccessTools.all, null, new[] { typeof(float) }, null)?.CreateDelegate(typeof(Action<scrHitErrorMeter, float>));
    }
}
