using HarmonyEx;
using Overlayer.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "OnDamage")]
    public static class MultipressDetectPatch
    {
        public static void Postfix(scrController __instance, bool multipress, bool applyMultipressDamage)
        {
            if (multipress)
            {
                if (applyMultipressDamage || __instance.consecMultipressCounter > 5)
                    Variables.MultipressCount++;
            }
        }
    }
}
