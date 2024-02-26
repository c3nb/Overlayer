using Overlayer.Core.Patches;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overlayer.Tags.Patches
{
    public class HitTimingPatch : PatchBase<HitTimingPatch>
    {
        [LazyPatch("Tags.HitTiming.TimingUpdater", "scrPlanet", "SwitchChosen", Triggers = new string[] { nameof(HitTiming.Timing) })]
        public static class TimingUpdater
        {
            public static List<double> Timings = new List<double>();
            public static void Prefix(scrPlanet __instance)
            {
                if (Main.IsPlaying)
                {
                    HitTiming.Timing = (__instance.angle - __instance.targetExitAngle) * (scrController.instance.isCW ? 1.0 : -1.0) * 60000.0 / (Math.PI * __instance.conductor.bpm * scrController.instance.speed * __instance.conductor.song.pitch);
                    Timings.Add(HitTiming.Timing);
                    HitTiming.TimingAvg = Timings.Average();
                }
                else HitTiming.Timing = 0;
            }
        }
        [LazyPatch("Tags.HitTiming.TimingResetter", "scrController", "Awake_Rewind", Triggers = new string[] { nameof(HitTiming.Timing) })]
        public static class TimingResetter
        {
            public static void Postfix()
            {
                HitTiming.Timing = 0;
                TimingUpdater.Timings = new List<double>();
            }
        }
    }
}
