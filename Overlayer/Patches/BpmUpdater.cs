using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;

namespace Overlayer.Patches
{
    public static class BpmUpdater
    {
        public static FieldInfo curSpd = typeof(GCS).GetField("currentSpeedRun", AccessTools.all) ?? typeof(GCS).GetField("currentSpeedTrial", AccessTools.all);
        public static float bpm = 0, pitch = 0, playbackSpeed = 1;
        public static bool beforedt = false;
        public static double beforebpm = 0;
        [HarmonyPatch(typeof(CustomLevel), "Play")]
        public static class CustomLevelStart
        {
            public static void Postfix(CustomLevel __instance)
            {
                if (!__instance.controller.gameworld) return;
                if (__instance.controller.customLevel == null) return;
                Init(__instance.controller);
            }
        }
        [HarmonyPatch(typeof(scrPressToStart), "ShowText")]
        public static class BossLevelStart
        {
            public static void Postfix(scrPressToStart __instance)
            {
                if (!__instance.controller.gameworld) return;
                if (__instance.controller.customLevel != null) return;
                Init(__instance.controller);
                Variables.StartProg = __instance.controller.percentComplete * 100;
            }
        }
        [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
        public static class MoveToNextFloor
        {
            public static void Postfix(scrPlanet __instance, scrFloor floor)
            {
                if (!__instance.controller.gameworld) return;
                if (floor.nextfloor == null) return;
                double curBPM = GetRealBpm(floor, bpm) * playbackSpeed * pitch;
                bool isDongta = false;
                Variables.TileBpm = Math.Round(bpm * __instance.controller.speed, Settings.Instance.TileBpmDecimals);
                if (isDongta || beforedt) curBPM = beforebpm;
                Variables.CurBpm = Math.Round(curBPM, Settings.Instance.PerceivedBpmDecimals);
                Variables.RecKPS = Math.Round(curBPM / 60, Settings.Instance.PerceivedKpsDecimals);
                beforedt = isDongta;
                beforebpm = curBPM;
            }
        }
        public static double GetRealBpm(scrFloor floor, float bpm)
        {
            if (floor == null)
                return bpm;
            if (floor.nextfloor == null)
                return floor.controller.speed * bpm;
            return 60.0 / (floor.nextfloor.entryTime - floor.entryTime);
        }
        public static void Init(scrController __instance)
        {
            float kps = 0;
            if (__instance.customLevel != null)
            {
                pitch = (float)__instance.customLevel.levelData.pitch / 100;
                if (GCS.standaloneLevelMode) pitch *= (float)curSpd.GetValue(null);
                playbackSpeed = scnEditor.instance.playbackSpeed;
                bpm = __instance.customLevel.levelData.bpm * playbackSpeed * pitch;
            }
            else
            {
                pitch = __instance.conductor.song.pitch;
                playbackSpeed = 1;
                bpm = __instance.conductor.bpm * pitch;
            }
            float cur = bpm;
            if (__instance.currentSeqID != 0)
            {
                double speed = __instance.controller.speed;
                cur = (float)(bpm * speed);
            }
            Variables.TileBpm = Math.Round(cur, Settings.Instance.TileBpmDecimals);
            Variables.CurBpm = Math.Round(cur, Settings.Instance.PerceivedBpmDecimals);
            Variables.RecKPS = Math.Round(kps, Settings.Instance.PerceivedKpsDecimals);
        }
    }
}

