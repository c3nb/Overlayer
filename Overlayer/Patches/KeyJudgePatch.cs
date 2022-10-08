using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Overlayer.Tags.Global;
using Overlayer.Core.Utils;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController))]
    public static class KeyJudgePatch
    {
        public static readonly KeyCode[] KeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
        public static readonly int KeyCodesCount = KeyCodes.Length;
        public static KeyCode[] codes = new KeyCode[0];
        public static KeyCode[] prevCodes = new KeyCode[0];
        [HarmonyPatch("Hit")]
        [HarmonyPrefix]
        public static void HitPrefix()
        {
            codes = new KeyCode[0];
            for (int i = 0; i < KeyCodesCount; i++)
            {
                KeyCode code = KeyCodes[i];
                if (Input.GetKeyDown(code) || Input.GetKeyUp(code))
                    ArrayUtils.Add(ref codes, code);
            }
        }
        [HarmonyPatch("ShowHitText")]
        [HarmonyPrefix]
        public static void SHTPrefix(scrController __instance, HitMargin hitMargin)
        {
            if (__instance.midspinInfiniteMargin)
                return;
            if (hitMargin == HitMargin.Multipress)
            {
                for (int i = 0; i < prevCodes.Length; i++)
                    KeyJudgeTag.Add(new KeyJudge(prevCodes[i], hitMargin));
            }
            else
            {
                for (int i = 0; i < codes.Length; i++)
                    KeyJudgeTag.Add(new KeyJudge(codes[i], hitMargin));
                prevCodes = codes;
            }
        }
    }
}
