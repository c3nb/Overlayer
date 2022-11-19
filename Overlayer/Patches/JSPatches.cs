using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Overlayer.Core.JavaScript.CustomLibrary;
using UnityEngine;

namespace Overlayer.Patches
{
    [HarmonyPatch]
    public static class JSPatches
    {
        public static void SceneLoads()
            => Ovlr.SceneLoads.ForEach(a => a());
        [HarmonyPatch(typeof(scrController), "Awake")]
        public static class InitEvent
        {
            public static void Postfix()
                => Ovlr.Inits.ForEach(a => a());
        }
        [HarmonyPatch(typeof(scrController), "Hit")]
        public static class HitEvent
        {
            public static void Postfix()
                => Ovlr.Hits.ForEach(a => a());
        }
        [HarmonyPatch(typeof(scrController), "Update")]
        public static class UpdateEvent
        {
            public static void Postfix()
                => Ovlr.Updates.ForEach(a => a());
        }
        [HarmonyPatch(typeof(scnEditor), "OpenLevelCo")]
        public static class OpenLevelEvent
        {
            public static void Postfix()
                => Ovlr.OpenLevels.ForEach(a => a());
        }
        // From PlanetTweaks
        [HarmonyPatch(typeof(scrPlanet), "Destroy")]
        public static class PlanetDestroyPatch
        {
            public static void Postfix(scrPlanet __instance)
            {
                var holder = __instance.GetComponent<SRHolder>();
                if (holder != null) holder.enabled = false;
            }
        }
        [HarmonyPatch(typeof(scrPlanet), "Die")]
        public static class PlanetDiePatch
        {
            public static void Postfix(scrPlanet __instance)
            {
                var holder = __instance.GetComponent<SRHolder>();
                if (holder != null) holder.enabled = false;
            }
        }
    }
}
