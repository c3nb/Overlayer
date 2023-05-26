using HarmonyLib;
using System;
using Overlayer.Tags;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Overlayer.Core.Tags;
using Overlayer.Core;

namespace Overlayer.Patches
{
    public static class BpmUpdater
    {
        #region Tags
        public static List<scrFloor> AllCheckPoints = new List<scrFloor>();
        public static List<double> TimingList = new List<double>();
        [Tag("TimingAvg", RelatedPatches = "Overlayer.Patches.BpmUpdater+CustomLevelStart:Postfix|Overlayer.Patches.BpmUpdater+BossLevelStart:Postfix", Category = Category.Misc)]
        public static double TimingAvg(double digits = -1)
        {
            if (TimingList.Any())
            {
                var avg = (double)TimingList.Average();
                if (digits != -1)
                    return (double)Math.Round(avg, (int)digits);
                return avg;
            }
            return 0;
        }
        #endregion
        public static FieldInfo curSpd = typeof(GCS).GetField("currentSpeedTrial", AccessTools.all);
        public static float bpm = 0, pitch = 0, playbackSpeed = 1;
        public static bool beforedt = false;
        public static double beforebpm = 0;
        [HarmonyPatch(typeof(scnGame), "Play")]
        public static class CustomLevelStart
        {
            public static void Postfix(scnGame __instance)
            {
                if (!(scrController.instance?.gameworld ?? false)) return;
                if (scnGame.instance == null) return;
                Init(scrController.instance);
            }
        }
        [HarmonyPatch(typeof(scrPressToStart), "ShowText")]
        public static class BossLevelStart
        {
            public static void Postfix(scrPressToStart __instance)
            {
                if (!scrController.instance.gameworld) return;
                if (scnGame.instance != null) return;
                Init(scrController.instance);
                Variables.StartProg = scrController.instance.percentComplete * 100;
            }
        }
        [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
        public static class MoveToNextFloorPatch
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
                foreach (var chkPt in AllCheckPoints)
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
            if (scnGame.instance)
                Adofaigg.Setup(scnGame.instance);
            else Adofaigg.Setup(scnEditor.instance);
            TimingList.Clear();
            Variables.IsStarted = true;
            AllCheckPoints = scrLevelMaker.instance.listFloors.FindAll(f => f.GetComponent<ffxCheckpoint>() != null);
            float kps = 0;
            try
            {
                if (scnGame.instance != null)
                {
                    pitch = (float)scnGame.instance.levelData.pitch / 100;
                    if (ADOBase.isCLSLevel) pitch *= (float)curSpd.GetValue(null);
                    bpm = scnGame.instance.levelData.bpm * playbackSpeed * pitch;
                }
                else
                {
                    pitch = scrConductor.instance.song.pitch;
                    bpm = scrConductor.instance.bpm * pitch;
                }
                playbackSpeed = scnEditor.instance?.playbackSpeed ?? 1;
            }
            catch (Exception e)
            {
                OverlayerDebug.Exception(e, "Exception At BpmUpdater.Init!");
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
            Variables.TileBpm = cur;
            Variables.CurBpm = cur;
            Variables.RecKPS = kps;
            Scripting.Api.ClearTileInfo();
        }
    }
}

