using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrFloor))]
    public static class TilePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        public static void AwakePostfix(scrFloor __instance)
        {
            if (ADOBase.sceneName.Contains("-") || ADOBase.sceneName == "scnNewIntro" || ADOBase.sceneName == "scnCLS") return;
            var spr = __instance.floorRenderer.renderer as SpriteRenderer;
            spr.drawMode = SpriteDrawMode.Sliced;
        }
        [HarmonyPostfix]
        [HarmonyPatch("LateUpdate")]
        public static void LateUpdatePostfix(scrFloor __instance)
        {
            if (!OText.IsPlaying) return;
            var spr = __instance.floorRenderer.renderer as SpriteRenderer;
            var size = spr.size;
            spr.size = new Vector2(Settings.Instance.TileLength * 1.33333333333333333333333333333333333333f, size.y);
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrController), "startRadius", MethodType.Getter)]
        public static bool RadiusPrefix(ref float __result)
        {
            if (Settings.Instance.TileLength == -1) return true;
            __result = Settings.Instance.TileLength;
            return false;
        }
    }
}
