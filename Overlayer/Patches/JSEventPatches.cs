using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Overlayer.Core.JavaScript.CustomLibrary;

namespace Overlayer.Patches
{
    [HarmonyPatch]
    public static class JSEventPatches
    {
        public static void SceneLoads()
            => Ovlr.SceneLoads.ForEach(a => a());
        [HarmonyPatch(typeof(scrController), "Hit")]
        public static class HitEvent
        {
            public static void Postfix()
                => Ovlr.Hits.ForEach(a => a());
        }
        [HarmonyPatch(typeof(scnEditor), "OpenLevelCo")]
        public static class OpenLevelEvent
        {
            public static void Postfix()
                => Ovlr.OpenLevels.ForEach(a => a());
        }
    }
}
