using Overlayer.Core.Tags;

namespace Overlayer.Tags
{
    public static class SHitTags
    {
        [Tag("SHit")]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Strict);
        [Tag("STE")]
        public static double TE() => Variables.StrictCounts[HitMargin.TooEarly];
        [Tag("SVE")]
        public static double VE() => Variables.StrictCounts[HitMargin.VeryEarly];
        [Tag("SEP")]
        public static double EP() => Variables.StrictCounts[HitMargin.EarlyPerfect];
        [Tag("SP")]
        public static double P() => Variables.StrictCounts[HitMargin.Perfect];
        [Tag("SLP")]
        public static double LP() => Variables.StrictCounts[HitMargin.LatePerfect];
        [Tag("SVL")]
        public static double VL() => Variables.StrictCounts[HitMargin.VeryLate];
        [Tag("STL")]
        public static double TL() => Variables.StrictCounts[HitMargin.TooLate];
    }
}
