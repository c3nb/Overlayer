using HarmonyLib;
using UnityModManagerNet;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(UnityModManager.Logger), "Log", new[] { typeof(string) })]
    public static class RemoveFuckingUMMDummyLogPatch
    {
        public static bool Prefix(string str) => str != "Cancel start. Already started.";
    }
}