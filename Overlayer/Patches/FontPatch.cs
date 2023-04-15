using HarmonyEx;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Overlayer.Core;
using System.Collections.Generic;

namespace Overlayer.Patches
{
    public static class FontPatch
    {
        [HarmonyPatch(typeof(RDString), "GetFontDataForLanguage")]
        public static class ChangeFontPatch
        {
            public static bool Prefix(ref FontData __result)
            {
                if (!FontManager.Initialized)
                    return true;
                if (!Main.Settings.ChangeFont)
                    return true;
                if (!FontManager.TryGetFont(Main.Settings.AdofaiFont.name, out var font))
                    return true;
                if (!(font.font?.dynamic ?? false)) 
                    return true;
                __result = font;
                return false;
            }
        }
        [HarmonyPatch(typeof(scrController), "Update")]
        public static class FontAttacher
        {
            public static void Postfix(scrController __instance)
            {
                if (!Main.Settings.ChangeFont) return;
                __instance.StartCoroutine(UpdateFontCo());
            }
            static IEnumerator UpdateFontCo()
            {
                if (!Main.Settings.ChangeFont) yield break;
                List<GameObject> list = new List<GameObject>();
                try { Main.ActiveScene.GetRootGameObjects(list); }
                catch { yield break; }
                foreach (var i in list)
                {
                    if (!i.activeSelf) continue;
                    if (i.GetComponentInChildren(typeof(scrEnableIfBeta)))
                        continue;
                    foreach (var j in i.GetComponentsInChildren<Text>(false))
                        j.SetLocalizedFont();
                    yield return null;
                }
            }
        }
    }
}
