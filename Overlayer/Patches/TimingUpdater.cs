using HarmonyLib;
using Overlayer.Tags.Global;
using System;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
    public static class TimingUpdater
    {
        public static void Postfix(scrPlanet __instance)
        {
            if (OText.IsPlaying)
            {
                Variables.Timing = Math.Round((__instance.angle - __instance.targetExitAngle) * (scrController.instance.isCW ? 1.0 : -1.0) * 60000.0 / (3.1415926535897931 * __instance.conductor.bpm * scrController.instance.speed * __instance.conductor.song.pitch), Settings.Instance.TimingDecimals);
                Timings.TimingList.Add(Variables.Timing);
                if (!scrController.instance.noFail)
                    Variables.BestProg = Math.Max(Variables.BestProg, scrController.instance.percentComplete * 100);
            }
            else Variables.Timing = 0;
        }
    }
}
