using System;
using System.Reflection;
using HarmonyLib;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
    public static class Resetter
    {
        public static string MapId = string.Empty;
        public static FieldInfo speedTrial;
        static Resetter() => speedTrial = typeof(GCS).GetField("currentSpeedRun", AccessTools.all) ?? typeof(GCS).GetField("currentSpeedTrial", AccessTools.all);
        public static void Postfix(scrController __instance) => Reset(__instance);
        public static void Reset(scrController __instance)
        {
            var caption = __instance.txtCaption?.text;
            if (caption != MapId)
            {
                Variables.BestProg = 0;
                Variables.LeastChkPt = 0;
            }
            if (Settings.Instance.Reset)
                Variables.Reset();
            if (__instance.customLevel != null)
            {
                float speed = (float)speedTrial.GetValue(null);
                BpmUpdater.pitch = (float)__instance.customLevel.levelData.pitch / 100;
                if (GCS.standaloneLevelMode) BpmUpdater.pitch *= speed;
                BpmUpdater.bpm = __instance.customLevel.levelData.bpm * BpmUpdater.pitch;
            }
            else
            {
                BpmUpdater.pitch = __instance.conductor.song.pitch;
                BpmUpdater.bpm = __instance.conductor.bpm * BpmUpdater.pitch;
            }
            Variables.CurBpm = BpmUpdater.bpm;
            if (__instance.currentSeqID != 0)
            {
                double speed = __instance.controller.speed;
                Variables.CurBpm = (float)(BpmUpdater.bpm * speed);
            }
            Variables.TileBpm = Variables.CurBpm;
            Variables.RecKPS = Math.Round(Variables.CurBpm / 60, Settings.Instance.PerceivedKpsDecimals);
            MapId = caption;
        }
    }
    [HarmonyPatch(typeof(scrController), "Start")]
    public static class Resetter2
    {
        public static void Postfix(scrController __instance) => Resetter.Reset(__instance);
    }
}
