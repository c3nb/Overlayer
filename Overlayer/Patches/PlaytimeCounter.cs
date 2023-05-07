using HarmonyLib;
using Overlayer.Core;
using Overlayer.Core.Tags;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Overlayer.Patches
{
    [HarmonyPatch]
    public static class PlaytimeCounter
    {
        public static double PlayTime = 0;
        [Tag("PlayTime", RelatedPatches = "Overlayer.Patches.PlaytimeCounter:UpdatePostfix|Overlayer.Patches.PlaytimeCounter+SetIDPatch", Category = Category.Play)]
        public static double PlayTimeTag(string opt = "M_1")
        {
            if (string.IsNullOrEmpty(opt))
                opt = "M_1";
            string[] split = opt.Split2('_');
            if (split.Length < 2) return 0;
            int digits = split.Length > 1 ? StringConverter.ToInt32(split[1]) : 1;
            switch (split[0])
            {
                case "H": return H().Round(digits);
                case "M": return M().Round(digits);
                case "S": return S().Round(digits);
                case "MS": return MS().Round(digits);
                default: return 0;
            }
            double H() => PlayTime / 60 / 60;
            double M() => PlayTime / 60;
            double S() => PlayTime;
            double MS() => PlayTime * 1000;
        }
        public static Dictionary<string, float> PlayTimes = new Dictionary<string, float>();
        public static string MapID = string.Empty;
        public static string ID(string id) => id + "_PlayTime";
        [HarmonyPatch(typeof(scrController), "Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(scrController __instance)
        {
            if (__instance.gameworld && __instance.state == States.PlayerControl)
                if (PlayTimes.TryGetValue(ID(MapID), out _))
                    PlayTime = PlayTimes[ID(MapID)] += Time.deltaTime;
                else PlayTime = 0;
        }
        [HarmonyPatch]
        public static class SetIDPatch
        {
            public static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(scrController), "Awake_Rewind");
                yield return AccessTools.Method(typeof(scnEditor), "Play");
            }
            public static void Postfix()
            {
                if (ADOBase.sceneName.Contains("-"))
                {
                    if (!PlayTimes.TryGetValue(ID(ADOBase.sceneName), out _))
                        PlayTimes[ID(ADOBase.sceneName)] = Persistence.generalPrefs.GetFloat(ID(ADOBase.sceneName));
                    PlayTime = PlayTimes[ID(ADOBase.sceneName)];
                }
                else
                {
                    if (scnEditor.instance?.levelData == null)
                        return;
                    var levelData = scnEditor.instance.levelData;
                    MapID = DataInit.MakeHash(levelData.author, levelData.artist, levelData.song);
                    if (!PlayTimes.TryGetValue(ID(MapID), out _))
                        PlayTimes[ID(MapID)] = Persistence.generalPrefs.GetFloat(ID(MapID));
                    PlayTime = PlayTimes[ID(MapID)];
                }
            }
        }
    }
}
