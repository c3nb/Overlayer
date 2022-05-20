using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace Overlayer.KeyViewer
{
    internal static class KeyViewerPatches
    {
        [HarmonyPatch(typeof(scrController), "CountValidKeysPressed")]
        public static class CountValidKeysPressedPatch
        { 
            public static bool IsListening = false;
            public static bool Prefix(ref int __result)
            {
                if (Settings.Instance.IsListening)
                {
                    __result = 0;
                    return false;
                }
                return true;
            }
        }
    }
}
