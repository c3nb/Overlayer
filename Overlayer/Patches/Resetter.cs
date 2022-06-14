using System;
using System.Reflection;
using HarmonyLib;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
    public static class Resetter
    {
        public static string MapId = string.Empty;
        public static void Prefix(scrController __instance) => Reset(__instance);
        public static void Reset(scrController __instance)
        {
            var caption = __instance.txtCaption?.text;
            if (caption != MapId)
            {
                Variables.BestProg = 0;
                Variables.LeastChkPt = 0;
            }
            if (Settings.Instance.Reset)
                Variables.Reset();
            MapId = caption;
        }
    }
    [HarmonyPatch(typeof(scrController), "Start")]
    public static class Resetter2
    {
        public static void Postfix(scrController __instance) => Resetter.Reset(__instance);
    }
}
