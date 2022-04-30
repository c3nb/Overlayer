using System;
using System.Reflection;
using HarmonyLib;

namespace Overlayer
{
    public class Patches
    {
        [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
        public static class TimingUpdater
        {
            public static void Postfix(scrPlanet __instance)
            {
                if (Text.IsPlaying)
                {
                    Variables.Timing = Math.Round((__instance.angle - __instance.targetExitAngle) * (__instance.controller.isCW ? 1.0 : -1.0) * 60000.0 / (3.1415926535897931 * __instance.conductor.bpm * __instance.controller.speed * __instance.conductor.song.pitch), Settings.Instance.TimingDecimals);
                    if (!__instance.controller.noFail)
                        Variables.BestProg = Math.Max(Variables.BestProg, __instance.controller.percentComplete * 100);
                }
                else Variables.Timing = 0;
            }
        }
        [HarmonyPatch(typeof(scrMistakesManager), "AddHit")]
        public static class ScoreCalculator
        {
            public static void Postfix(HitMargin hit)
            {
                if (hit == HitMargin.Perfect) Variables.Combo++;
                else Variables.Combo = 0;
                switch (hit)
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Variables.CurrentScore += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Variables.CurrentScore += 150;
                        break;
                    case HitMargin.Perfect:
                        Variables.CurrentScore += 300;
                        break;
                }
                if (GCS.difficulty != Difficulty.Lenient)
                switch (Utils.GetHitMarginForDifficulty(Variables.Angle, Difficulty.Lenient))
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Variables.LenientScore += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Variables.LenientScore += 150;
                        break;
                    case HitMargin.Perfect:
                        Variables.LenientScore += 300;
                        break;
                }
                if (GCS.difficulty != Difficulty.Normal)
                switch (Utils.GetHitMarginForDifficulty(Variables.Angle, Difficulty.Normal))
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Variables.NormalScore += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Variables.NormalScore += 150;
                        break;
                    case HitMargin.Perfect:
                        Variables.NormalScore += 300;
                        break;
                }
                if (GCS.difficulty != Difficulty.Strict)
                switch (Utils.GetHitMarginForDifficulty(Variables.Angle, Difficulty.Strict))
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Variables.StrictScore += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Variables.StrictScore += 150;
                        break;
                    case HitMargin.Perfect:
                        Variables.StrictScore += 300;
                        break;
                }
            }
        }
        [HarmonyPatch(typeof(scrMisc), "GetHitMargin")]
        public static class GetHitMarginFixer
        {
            public static bool Prefix(float hitangle, float refangle, bool isCW, float bpmTimesSpeed, float conductorPitch, ref HitMargin __result)
            {
                float num = (hitangle - refangle) * (isCW ? 1 : -1);
                HitMargin result = HitMargin.TooEarly;
                float num2 = num;
                num2 = 57.29578f * num2;
                double adjustedAngleBoundaryInDeg = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Counted, bpmTimesSpeed, conductorPitch);
                double adjustedAngleBoundaryInDeg2 = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Perfect, bpmTimesSpeed, conductorPitch);
                double adjustedAngleBoundaryInDeg3 = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Pure, bpmTimesSpeed, conductorPitch);
                if (num2 > -adjustedAngleBoundaryInDeg) result = HitMargin.VeryEarly;
                if (num2 > -adjustedAngleBoundaryInDeg2) result = HitMargin.EarlyPerfect;
                if (num2 > -adjustedAngleBoundaryInDeg3) result = HitMargin.Perfect;
                if (num2 > adjustedAngleBoundaryInDeg3) result = HitMargin.LatePerfect;
                if (num2 > adjustedAngleBoundaryInDeg2) result = HitMargin.VeryLate;
                if (num2 > adjustedAngleBoundaryInDeg) result = HitMargin.TooLate;
                Variables.Lenient = Utils.GetHitMarginForDifficulty(num2, Difficulty.Lenient);
                Variables.Normal = Utils.GetHitMarginForDifficulty(num2, Difficulty.Normal);
                Variables.Strict = Utils.GetHitMarginForDifficulty(num2, Difficulty.Strict);
                Variables.LenientCounts[Variables.Lenient]++;
                Variables.NormalCounts[Variables.Normal]++;
                Variables.StrictCounts[Variables.Strict]++;
                Variables.Angle = num2;
                __result = result;
                return false;
            }
        }
        [HarmonyPatch(typeof(scrController), "FailAction")]
        public static class FailCounter
        {
            public static string MapId = string.Empty;
            public static void Postfix(scrController __instance)
            {
                Variables.FailCount++;
                if (!__instance.noFail)
                    Variables.BestProg = Math.Max(Variables.BestProg, __instance.controller.percentComplete * 100);
            }
        }
        [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
        public static class LeastChkPtLogger
        {
            public static void Postfix(scrController __instance) => Variables.LeastChkPt = Math.Min(Variables.LeastChkPt, __instance.customLevel.checkpointsUsed);
        }
        [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
        public static class Resetter
        {
            public static FieldInfo speedTrial;
            static Resetter() => speedTrial = typeof(GCS).GetField("currentSpeedRun", AccessTools.all) ?? typeof(GCS).GetField("currentSpeedTrial", AccessTools.all);
            public static void Postfix(scrController __instance) => Reset(__instance);
            public static void Reset(scrController __instance)
            {
                var caption = __instance.txtCaption?.text;
                if (caption != FailCounter.MapId)
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
                FailCounter.MapId = caption;
            }
        }
        [HarmonyPatch(typeof(scrController), "Start")]
        public static class Resetter2
        {
            public static void Postfix(scrController __instance) => Resetter.Reset(__instance);
        }
        [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
        public static class BpmUpdater
        {
            public static float pitch, bpm;
            public static void Postfix(scrPlanet __instance, scrFloor floor)
            {
                if (!Text.IsPlaying) return;
                double speed, curBPM;
                speed = __instance.controller.speed;
                curBPM = GetRealBpm(floor, bpm);
                Settings setting = Settings.Instance;
                Variables.TileBpm = Math.Round(bpm * speed, setting.TileBpmDecimals);
                Variables.CurBpm = Math.Round(curBPM, setting.PerceivedBpmDecimals);
                Variables.RecKPS = Math.Round(curBPM / 60, setting.PerceivedKpsDecimals);
            }
            public static bool DoubleEqual(double f1, double f2) => Math.Abs(f1 - f2) < 0.0001;
            public static double GetRealBpm(scrFloor floor, float bpm)
            {
                double val = scrMisc.GetAngleMoved(floor.entryangle, floor.exitangle, !floor.isCCW) / 3.1415927410125732 * 180;
                double angle = Math.Round(val);
                double speed = floor.controller.speed;
                if (angle == 0) angle = 360;
                return 180 / angle * (speed * bpm);
            }
        }
        [HarmonyPatch(typeof(scrPlanet), "ScrubToFloorNumber")]
        public static class StartProgUpdater
        {
            public static void Postfix(scrPlanet __instance)
            {
                if (__instance.controller?.gameworld ?? false)
                    Variables.StartProg = __instance.controller.percentComplete * 100;
            }
        }
    }
}
