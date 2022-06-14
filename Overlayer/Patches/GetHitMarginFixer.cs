using HarmonyLib;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrMisc), "GetHitMargin")]
    public static class GetHitMarginFixer
    {
        public static bool Prefix(float hitangle, float refangle, bool isCW, float bpmTimesSpeed, float conductorPitch, double marginScale, ref HitMargin __result)
        {
			Variables.Lenient = Utils.GetHitMargin(Difficulty.Lenient, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
			Variables.Normal = Utils.GetHitMargin(Difficulty.Normal, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
			Variables.Strict = Utils.GetHitMargin(Difficulty.Strict, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
            __result = Utils.GetCurHitMargin(GCS.difficulty);
            if (!Variables.Lenient.ProcessHitMargin())
                Variables.LenientCounts[Variables.Lenient]++;
            if (!Variables.Normal.ProcessHitMargin())
                Variables.NormalCounts[Variables.Normal]++;
            if (!Variables.Strict.ProcessHitMargin())
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
        public static bool ProcessHitMargin(this ref HitMargin hitMargin)
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
    }
}
