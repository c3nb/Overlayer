using HarmonyLib;
using JSEngine.CustomLibrary;
using Overlayer.Tags.Global;
using System;
using System.Collections.Generic;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
    public static class TimingUpdater
    {
        public static void Prefix(scrPlanet __instance)
        {
            if (OText.IsPlaying)
            {
                Variables.Timing = (__instance.angle - __instance.targetExitAngle) * (scrController.instance.isCW ? 1.0 : -1.0) * 60000.0 / (Math.PI * __instance.conductor.bpm * scrController.instance.speed * __instance.conductor.song.pitch);
                Timings.TimingList.Add(Variables.Timing);
                if (!scrController.instance.noFail)
                    Variables.BestProg = Math.Max(Variables.BestProg, scrController.instance.percentComplete * 100);

                var xacc = double.IsNaN(Misc.XAccuracy()) ? 100 : Misc.XAccuracy();
                Ovlr.tiles.Add(new Core.TileData(__instance.controller.currentSeqID, Timings.Timing(), xacc, Misc.Accuracy(), (int)CurHitTags.GetCurHitMargin(GCS.difficulty)));
            }
            else Variables.Timing = 0;
        }
    }
}
