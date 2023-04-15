using HarmonyLib;
using Overlayer.Core;
using Overlayer.Tags;
using System.Collections.Generic;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
    public static class Resetter
    {
        public static void Prefix(scrController __instance) => Reset(__instance);
        public static void Reset(scrController __instance)
        {
            Variables.Reset();
        }
    }
    [HarmonyPatch(typeof(scrController), "Start")]
    public static class Resetter2
    {
        public static void Postfix(scrController __instance) => Resetter.Reset(__instance);
    }
}
