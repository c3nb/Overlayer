using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(PatchInfo), "Add")]
    public static class DistinctHarmonyPatches
    {
        public static bool Prefix(string owner, HarmonyMethod[] add, Patch[] current, ref Patch[] __result)
        {
            if (add.Length == 0)
            {
                __result = current;
                return false;
            }
            int initialIndex = current.Length;
            __result = current.Concat(add.Where((HarmonyMethod method) => method != null)
                .Select((HarmonyMethod method, int i) => new Patch(method, i + initialIndex, owner)))
                .Distinct(Core.PatchInfo.PatchComparer.Instance)
                .ToArray();
            return false;
        }
    }
}
