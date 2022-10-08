using Overlayer.Core;

namespace Overlayer.Tags.Strict
{
    public static class SHitTags
    {
        [Tag("SHit", "HitMargin in Strict Difficulty")]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Strict);
        [Tag("STE", "TooEarly in Strict Difficulty")]
        public static float TE() => Variables.StrictCounts[HitMargin.TooEarly];
        [Tag("SVE", "VeryEarly in Strict Difficulty")]
        public static float VE() => Variables.StrictCounts[HitMargin.VeryEarly];
        [Tag("SEP", "EarlyPerfect in Strict Difficulty")]
        public static float EP() => Variables.StrictCounts[HitMargin.EarlyPerfect];
        [Tag("SP", "Perfect in Strict Difficulty")]
        public static float P() => Variables.StrictCounts[HitMargin.Perfect];
        [Tag("SLP", "LatePerfect in Strict Difficulty")]
        public static float LP() => Variables.StrictCounts[HitMargin.LatePerfect];
        [Tag("SVL", "VeryLate in Strict Difficulty")]
        public static float VL() => Variables.StrictCounts[HitMargin.VeryLate];
        [Tag("STL", "TooLate in Strict Difficulty")]
        public static float TL() => Variables.StrictCounts[HitMargin.TooLate];
    }
}
