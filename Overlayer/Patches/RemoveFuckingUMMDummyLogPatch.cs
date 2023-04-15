using HarmonyExLib;
using Overlayer.Core.Tags;
using UnityModManagerNet;

namespace Overlayer.Patches
{
    [HarmonyExPatch(typeof(UnityModManager.Logger), "Log", new[] { typeof(string) })]
    public static class RemoveFuckingUMMDummyLogPatch
    {
        public static bool Prefix(string str) => str != "Cancel start. Already started.";
    }
}