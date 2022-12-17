#if TOURNAMENT
using HarmonyLib;
using Overlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer
{
    public static class Tournament
    {
        public static double HMRange = 0;
        [Tag("HitMarginRange")]
        public static double HitMarginRange(double digits = -1) => HMRange.Round(digits);
        [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
        public static class DetectHMRange
        {
            public static void Postfix(scrPlanet __instance)
            {
                HMRange = __instance.currfloor.nextfloor == null ? 1.0 : __instance.currfloor.nextfloor.marginScale;
            }
        }
    }
}
#endif
