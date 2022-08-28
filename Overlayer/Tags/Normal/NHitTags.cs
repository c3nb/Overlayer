using TagLib.Tags;

namespace Overlayer.Tags.Normal
{
    public static class NHitTags
    {
        [Tag("NHit", "HitMargin in Normal Difficulty")]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Normal);
        [Tag("NTE", "TooEarly in Normal Difficulty")]
        public static float TE() => Variables.NormalCounts[HitMargin.TooEarly];
        [Tag("NVE", "VeryEarly in Normal Difficulty")]
        public static float VE() => Variables.NormalCounts[HitMargin.VeryEarly];
        [Tag("NEP", "EarlyPerfect in Normal Difficulty")]
        public static float EP() => Variables.NormalCounts[HitMargin.EarlyPerfect];
        [Tag("NP", "Perfect in Normal Difficulty")]
        public static float P() => Variables.NormalCounts[HitMargin.Perfect];
        [Tag("NLP", "LatePerfect in Normal Difficulty")]
        public static float LP() => Variables.NormalCounts[HitMargin.LatePerfect];
        [Tag("NVL", "VeryLate in Normal Difficulty")]
        public static float VL() => Variables.NormalCounts[HitMargin.VeryLate];
        [Tag("NTL", "TooLate in Normal Difficulty")]
        public static float TL() => Variables.NormalCounts[HitMargin.TooLate];
    }
}
