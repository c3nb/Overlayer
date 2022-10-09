using Overlayer.Core;

namespace Overlayer.Tags.Strict
{
    public static class SHitTags
    {
        [Tag("SHit")]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Strict);
        [Tag("STE")]
        public static float TE() => Variables.StrictCounts[HitMargin.TooEarly];
        [Tag("SVE")]
        public static float VE() => Variables.StrictCounts[HitMargin.VeryEarly];
        [Tag("SEP")]
        public static float EP() => Variables.StrictCounts[HitMargin.EarlyPerfect];
        [Tag("SP")]
        public static float P() => Variables.StrictCounts[HitMargin.Perfect];
        [Tag("SLP")]
        public static float LP() => Variables.StrictCounts[HitMargin.LatePerfect];
        [Tag("SVL")]
        public static float VL() => Variables.StrictCounts[HitMargin.VeryLate];
        [Tag("STL")]
        public static float TL() => Variables.StrictCounts[HitMargin.TooLate];
    }
}
