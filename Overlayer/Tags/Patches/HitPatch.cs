using Overlayer.Core.Patches;
using System;
using UnityEngine;

namespace Overlayer.Tags.Patches
{
    public class HitPatch : PatchBase<HitPatch>
    {
        [LazyPatch("Tags.Hit.JudgementCalculator", "scrMisc", "GetHitMargin",
        Triggers = new string[]
        {
            nameof(Hit.LHit), nameof(Hit.LTE), nameof(Hit.LVE), nameof(Hit.LEP), nameof(Hit.LP), nameof(Hit.LLP), nameof(Hit.LVL), nameof(Hit.LTL),
            nameof(Hit.NHit), nameof(Hit.NTE), nameof(Hit.NVE), nameof(Hit.NEP), nameof(Hit.NP), nameof(Hit.NLP), nameof(Hit.NVL), nameof(Hit.NTL),
            nameof(Hit.SHit), nameof(Hit.STE), nameof(Hit.SVE), nameof(Hit.SEP), nameof(Hit.SP), nameof(Hit.SLP), nameof(Hit.SVL), nameof(Hit.STL),
            nameof(Hit.CHit), nameof(Hit.CTE), nameof(Hit.CVE), nameof(Hit.CEP), nameof(Hit.CP), nameof(Hit.CLP), nameof(Hit.CVL), nameof(Hit.CTL),
            "LHitRaw", "NHitRaw", "SHitRaw", "CHitRaw",
            nameof(Hit.LFast), nameof(Hit.NFast), nameof(Hit.SFast), nameof(Hit.CFast),
            nameof(Hit.LSlow), nameof(Hit.NSlow), nameof(Hit.SSlow), nameof(Hit.CSlow),
            nameof(Hit.LMarginCombos), nameof(Hit.NMarginCombos), nameof(Hit.SMarginCombos), nameof(Hit.MarginCombos),
            nameof(Hit.LMarginMaxCombos), nameof(Hit.NMarginMaxCombos), nameof(Hit.SMarginMaxCombos), nameof(Hit.MarginMaxCombos),
        })]
        public static class JudgementTagPatch
        {
            public static bool Prefix(float hitangle, float refangle, bool isCW, float bpmTimesSpeed, float conductorPitch, double marginScale, ref HitMargin __result)
            {
                var controller = scrController.instance;
                if (controller && controller.currFloor.freeroam) return true;
                Hit.Lenient = GetHitMargin(Difficulty.Lenient, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
                Hit.Normal = GetHitMargin(Difficulty.Normal, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
                Hit.Strict = GetHitMargin(Difficulty.Strict, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
                FixMargin(controller, ref Hit.Lenient);
                FixMargin(controller, ref Hit.Normal);
                FixMargin(controller, ref Hit.Strict);
                Hit.Current = __result = Hit.GetCHit(GCS.difficulty);
                if (!IsSafe(controller))
                {
                    IncreaseCount(Difficulty.Lenient, Hit.Lenient);
                    IncreaseCount(Difficulty.Normal, Hit.Normal);
                    IncreaseCount(Difficulty.Strict, Hit.Strict);
                    IncreaseCCount(Hit.Current);
                    Hit.SetMarginCombos();
                }
                return false;
            }
            public static bool IsSafe(scrController ctrl) => ctrl.currFloor?.isSafe ?? false;
            public static void FixMargin(scrController ctrl, ref HitMargin hitMargin)
            {
                if (ctrl.gameworld)
                {
                    if (ctrl.noFailInfiniteMargin)
                        hitMargin = HitMargin.FailMiss;
                    if (ctrl.midspinInfiniteMargin || (RDC.auto && !RDC.useOldAuto))
                        hitMargin = HitMargin.Perfect;
                }
            }
            private static void IncreaseCount(Difficulty diff, HitMargin hit)
            {
                switch (hit)
                {
                    case HitMargin.TooEarly:
                        switch (diff)
                        {
                            case Difficulty.Lenient: Hit.LTE++; break;
                            case Difficulty.Normal: Hit.NTE++; break;
                            case Difficulty.Strict: Hit.STE++; break;
                        }
                        break;
                    case HitMargin.VeryEarly:
                        switch (diff)
                        {
                            case Difficulty.Lenient: Hit.LVE++; break;
                            case Difficulty.Normal: Hit.NVE++; break;
                            case Difficulty.Strict: Hit.SVE++; break;
                        }
                        break;
                    case HitMargin.EarlyPerfect:
                        switch (diff)
                        {
                            case Difficulty.Lenient: Hit.LEP++; break;
                            case Difficulty.Normal: Hit.NEP++; break;
                            case Difficulty.Strict: Hit.SEP++; break;
                        }
                        break;
                    case HitMargin.Perfect:
                        switch (diff)
                        {
                            case Difficulty.Lenient: Hit.LP++; break;
                            case Difficulty.Normal: Hit.NP++; break;
                            case Difficulty.Strict: Hit.SP++; break;
                        }
                        break;
                    case HitMargin.LatePerfect:
                        switch (diff)
                        {
                            case Difficulty.Lenient: Hit.LLP++; break;
                            case Difficulty.Normal: Hit.NLP++; break;
                            case Difficulty.Strict: Hit.SLP++; break;
                        }
                        break;
                    case HitMargin.VeryLate:
                        switch (diff)
                        {
                            case Difficulty.Lenient: Hit.LVL++; break;
                            case Difficulty.Normal: Hit.NVL++; break;
                            case Difficulty.Strict: Hit.SVL++; break;
                        }
                        break;
                    case HitMargin.TooLate:
                        switch (diff)
                        {
                            case Difficulty.Lenient: Hit.LTL++; break;
                            case Difficulty.Normal: Hit.NTL++; break;
                            case Difficulty.Strict: Hit.STL++; break;
                        }
                        break;
                }
            }
            private static void IncreaseCCount(HitMargin hit)
            {
                switch (hit)
                {
                    case HitMargin.TooEarly: Hit.CTE++; break;
                    case HitMargin.VeryEarly: Hit.CVE++; break;
                    case HitMargin.EarlyPerfect: Hit.CEP++; break;
                    case HitMargin.Perfect: Hit.CP++; break;
                    case HitMargin.LatePerfect: Hit.CLP++; break;
                    case HitMargin.VeryLate: Hit.CVL++; break;
                    case HitMargin.TooLate: Hit.CTL++; break;
                }
            }
            private static double GetAdjustedAngleBoundaryInDeg(Difficulty diff, HitMarginGeneral marginType, double bpmTimesSpeed, double conductorPitch, double marginMult = 1.0)
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
        [LazyPatch("Tags.Hit.MultipressDetector", "scrController", "OnDamage",
        Triggers = new string[] { nameof(Hit.Multipress) })]
        public static class MultipressDetectPatch
        {
            public static void Postfix(scrController __instance, bool multipress, bool applyMultipressDamage)
            {
                if (multipress)
                {
                    if (applyMultipressDamage || __instance.consecMultipressCounter > 5)
                        Hit.Multipress++;
                }
            }
        }
    }
}
