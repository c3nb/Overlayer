using HarmonyLib;
using Overlayer.Core.Tags;
using UnityModManagerNet;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(UnityModManager.Logger), "Log", new[] { typeof(string) })]
    public static class RemoveFuckingUMMDummyLogPatch
    {
        public static bool Prefix(string str) => str != "Cancel start. Already started.";
    }
    [HarmonyPatch(typeof(scrController), "Hit")]
    public static class TestPatch
    {
        [FieldTag("HitCount", NotPlaying = true, RelatedPatches = "Overlayer.Patches.TestPatch:Postfix")]
        public static int Count = 0;
        public static void Postfix()
        {
            Count++;
            Main.Logger.Log("Count!!");
        }
    }
}