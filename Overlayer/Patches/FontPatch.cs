using Overlayer.Core;
using Overlayer.Core.Patches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Overlayer.Patches
{
    public static class FontPatch
    {
        [LazyPatch("Patches.FontPatch.FontChanger", "RDString", "GetFontDataForLanguage")]
        public static class FontChanger
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
        [LazyPatch("Patches.FontPatch.FontAttacher", "scrController", "Update")]
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
