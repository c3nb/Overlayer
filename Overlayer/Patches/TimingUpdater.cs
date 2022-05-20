using System;
using HarmonyLib;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
    public static class TimingUpdater
    {
        public static void Postfix(scrPlanet __instance)
        {
            if (OText.IsPlaying)
            {
                Variables.Timing = Math.Round((__instance.angle - __instance.targetExitAngle) * (__instance.controller.isCW ? 1.0 : -1.0) * 60000.0 / (3.1415926535897931 * __instance.conductor.bpm * __instance.controller.speed * __instance.conductor.song.pitch), Settings.Instance.TimingDecimals);
                if (!__instance.controller.noFail)
                    Variables.BestProg = Math.Max(Variables.BestProg, __instance.controller.percentComplete * 100);
            }
            else Variables.Timing = 0;
        }
    }
}
