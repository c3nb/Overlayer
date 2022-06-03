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
			Variables.LenientCounts[Variables.Lenient]++;
            Variables.NormalCounts[Variables.Normal]++;
            Variables.StrictCounts[Variables.Strict]++;
            __result = Utils.GetCurHitMargin(GCS.difficulty);
            return false;
        }
    }
}
