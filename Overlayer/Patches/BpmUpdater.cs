using HarmonyLib;
using Overlayer.Tags.Global;
using System;
using Overlayer.Core;
using System.Reflection;

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
                if (!scrController.instance.gameworld) return;
                if (CustomLevel.instance == null) return;
                Init(scrController.instance);
            }
        }
        [HarmonyPatch(typeof(scrPressToStart), "ShowText")]
        public static class BossLevelStart
        {
            public static void Postfix(scrPressToStart __instance)
            {
                if (!scrController.instance.gameworld) return;
                if (CustomLevel.instance != null) return;
                Init(scrController.instance);
                Variables.StartProg = scrController.instance.percentComplete * 100;
            }
        }
        [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
        public static class MoveToNextFloor
        {
            public static void Postfix(scrPlanet __instance, scrFloor floor)
            {
                if (!scrController.instance.gameworld) return;
                Variables.CurrentCheckPoint = GetCheckPointIndex(floor);
                if (floor.nextfloor == null) return;
                double curBPM = GetRealBpm(floor, bpm) * playbackSpeed * pitch;
                bool isDongta = false;
                Variables.TileBpm = bpm * scrController.instance.speed;
                if (isDongta || beforedt) curBPM = beforebpm;
                Variables.CurBpm = curBPM;
                Variables.RecKPS = curBPM / 60;
                beforedt = isDongta;
                beforebpm = curBPM;
            }
            public static int GetCheckPointIndex(scrFloor floor)
            {
                int i = 0;
                foreach (var chkPt in Variables.AllCheckPoints)
                {
                    if (floor.seqID + 1 <= chkPt.seqID)
                        return i;
                    i++;
                }
                return i;
            }
        }
        public static double GetRealBpm(scrFloor floor, float bpm)
        {
            if (floor == null)
                return bpm;
            if (floor.nextfloor == null)
                return scrController.instance.speed * bpm;
            return 60.0 / (floor.nextfloor.entryTime - floor.entryTime);
        }
        public static string mapHash = "";
        public static void Init(scrController __instance)
        {
            if (mapHash != __instance.caption)
            {
                ProgressDeath.Reset();
                mapHash = __instance.caption;
            }
            PlayPoint.Setup(scnEditor.instance);
            Timings.TimingList.Clear();
            Variables.IsStarted = true;
            Variables.AllCheckPoints = scrLevelMaker.instance.listFloors.FindAll(f => f.GetComponent<ffxCheckpoint>() != null);
            float kps = 0;
            if (CustomLevel.instance != null)
            {
                pitch = (float)CustomLevel.instance.levelData.pitch / 100;
                if (GCS.standaloneLevelMode) pitch *= (float)curSpd.GetValue(null);
                playbackSpeed = scnEditor.instance.playbackSpeed;
                bpm = CustomLevel.instance.levelData.bpm * playbackSpeed * pitch;
            }
            else
            {
                pitch = scrConductor.instance.song.pitch;
                playbackSpeed = 1;
                bpm = scrConductor.instance.bpm * pitch;
            }
            float cur = bpm;
            if (__instance.currentSeqID != 0)
            {
                double speed = scrController.instance.speed;
                cur = (float)(bpm * speed);
            }
            if (!Settings.Instance.ApplyPitchAtBpmTags)
                pitch = 1;
            Variables.TileBpm = cur;
            Variables.CurBpm = cur;
            Variables.RecKPS = kps;
        }
    }
}

