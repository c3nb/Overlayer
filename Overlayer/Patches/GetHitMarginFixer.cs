using HarmonyLib;
using Overlayer.Tags.Global;
using System;
using UnityEngine;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrMisc), "GetHitMargin")]
    public static class GetHitMarginFixer
    {
        public static bool Prefix(float hitangle, float refangle, bool isCW, float bpmTimesSpeed, float conductorPitch, double marginScale, ref HitMargin __result)
        {
            var controller = scrController.instance;
            if (controller)
            {
                if (controller.currFloor.freeroam)
                    return true;
            }
            Variables.Lenient = GetHitMargin(Difficulty.Lenient, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
            Variables.Normal = GetHitMargin(Difficulty.Normal, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
            Variables.Strict = GetHitMargin(Difficulty.Strict, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
            __result = CurHitTags.GetCurHitMargin(GCS.difficulty);
            if (!Variables.Lenient.SafeMargin())
                Variables.LenientCounts[Variables.Lenient]++;
            if (!Variables.Normal.SafeMargin())
                Variables.NormalCounts[Variables.Normal]++;
            if (!Variables.Strict.SafeMargin())
                Variables.StrictCounts[Variables.Strict]++;
            if (__result == HitMargin.Perfect)
                Variables.Combo++;
            else Variables.Combo = 0;
            switch (GCS.difficulty)
            {
                case Difficulty.Lenient:
                    CalculateScores(__result, Variables.Normal, Variables.Strict, __result);
                    return false;
                case Difficulty.Normal:
                    CalculateScores(Variables.Lenient, __result, Variables.Strict, __result);
                    return false;
                case Difficulty.Strict:
                    CalculateScores(Variables.Lenient, Variables.Normal, __result, __result);
                    return false;
                default: return false;
            }
        }
        public static bool SafeMargin(this ref HitMargin hitMargin)
        {
            scrController ctrl = scrController.instance;
            if (ctrl.gameworld)
            {
                if (ctrl.noFailInfiniteMargin)
                    hitMargin = HitMargin.FailMiss;
                if (ctrl.midspinInfiniteMargin || (RDC.auto && !RDC.useOldAuto))
                    hitMargin = HitMargin.Perfect;
            }
            return ctrl.currFloor?.isSafe ?? false;
        }
        public static void CalculateScores(HitMargin l, HitMargin n, HitMargin s, HitMargin cur)
        {
            switch (cur)
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
            switch (l)
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
            switch (n)
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
            switch (s)
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
        public static double GetAdjustedAngleBoundaryInDeg(Difficulty diff, HitMarginGeneral marginType, double bpmTimesSpeed, double conductorPitch, double marginMult = 1.0)
        {
            float num = 0.065f;
            if (diff == Difficulty.Lenient)
                num = 0.091f;
            if (diff == Difficulty.Normal)
                num = 0.065f;
            if (diff == Difficulty.Strict)
                num = 0.04f;
            bool isMobile = ADOBase.isMobile;
            num = isMobile ? 0.09f : (num / GCS.currentSpeedTrial);
            float num2 = isMobile ? 0.07f : (0.03f / GCS.currentSpeedTrial);
            float a = isMobile ? 0.05f : (0.02f / GCS.currentSpeedTrial);
            num = Mathf.Max(num, 0.025f);
            num2 = Mathf.Max(num2, 0.025f);
            double num3 = (double)Mathf.Max(a, 0.025f);
            double val = scrMisc.TimeToAngleInRad((double)num, bpmTimesSpeed, conductorPitch, false) * 57.295780181884766;
            double val2 = scrMisc.TimeToAngleInRad((double)num2, bpmTimesSpeed, conductorPitch, false) * 57.295780181884766;
            double val3 = scrMisc.TimeToAngleInRad(num3, bpmTimesSpeed, conductorPitch, false) * 57.295780181884766;
            double result = Math.Max(GCS.HITMARGIN_COUNTED * marginMult, val);
            double result2 = Math.Max(45.0 * marginMult, val2);
            double result3 = Math.Max(30.0 * marginMult, val3);
            if (marginType == HitMarginGeneral.Counted)
                return result;
            if (marginType == HitMarginGeneral.Perfect)
                return result2;
            if (marginType == HitMarginGeneral.Pure)
                return result3;
            return result;
        }
        public static HitMargin GetHitMargin(Difficulty diff, float hitangle, float refangle, bool isCW, float bpmTimesSpeed, float conductorPitch, double marginScale)
        {
            float num = (hitangle - refangle) * (isCW ? 1 : -1);
            HitMargin result = HitMargin.TooEarly;
            float num2 = num;
            num2 = 57.29578f * num2;
            double adjustedAngleBoundaryInDeg = GetAdjustedAngleBoundaryInDeg(diff, HitMarginGeneral.Counted, (double)bpmTimesSpeed, (double)conductorPitch, marginScale);
            double adjustedAngleBoundaryInDeg2 = GetAdjustedAngleBoundaryInDeg(diff, HitMarginGeneral.Perfect, (double)bpmTimesSpeed, (double)conductorPitch, marginScale);
            double adjustedAngleBoundaryInDeg3 = GetAdjustedAngleBoundaryInDeg(diff, HitMarginGeneral.Pure, (double)bpmTimesSpeed, (double)conductorPitch, marginScale);
            if ((double)num2 > -adjustedAngleBoundaryInDeg)
                result = HitMargin.VeryEarly;
            if ((double)num2 > -adjustedAngleBoundaryInDeg2)
                result = HitMargin.EarlyPerfect;
            if ((double)num2 > -adjustedAngleBoundaryInDeg3)
                result = HitMargin.Perfect;
            if ((double)num2 > adjustedAngleBoundaryInDeg3)
                result = HitMargin.LatePerfect;
            if ((double)num2 > adjustedAngleBoundaryInDeg2)
                result = HitMargin.VeryLate;
            if ((double)num2 > adjustedAngleBoundaryInDeg)
                result = HitMargin.TooLate;
            return result;
        }

    }
}
