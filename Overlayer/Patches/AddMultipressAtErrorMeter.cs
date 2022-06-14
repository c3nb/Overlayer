using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrHitErrorMeter), "CalculateTickColor")]
    public static class AddMultipressAtErrorMeter
    {
        public static int QueuedMP = 0;
        public static void Postfix(ref Color __result)
        {
			if (Settings.Instance.AddMultipressAtErrorMeter)
            {
                if (QueuedMP > 0)
                {
                    __result = RDConstants.data.hitMarginColours.colourMultipress;
                    QueuedMP--;
                }
            }
		}
        [HarmonyPatch(typeof(scrController), "ShowHitText")]
        [HarmonyPostfix]
        public static void SHTPostfix(HitMargin hitMargin)
        {
            if (hitMargin == HitMargin.Multipress)
                QueuedMP++;
        }
    }
}
