using HarmonyLib;

namespace Overlayer.Patches
{
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
}
