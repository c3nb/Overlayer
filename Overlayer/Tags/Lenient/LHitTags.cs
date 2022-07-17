namespace Overlayer.Tags.Lenient
{
    public static class LHitTags
    {
        [Tag("LHit", "HitMargin in Lenient Difficulty")]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Lenient);
        [Tag("LTE", "TooEarly in Lenient Difficulty")]
        public static float TE() => Variables.LenientCounts[HitMargin.TooEarly];
        [Tag("LVE", "VeryEarly in Lenient Difficulty")]
        public static float VE() => Variables.LenientCounts[HitMargin.VeryEarly];
        [Tag("LEP", "EarlyPerfect in Lenient Difficulty")]
        public static float EP() => Variables.LenientCounts[HitMargin.EarlyPerfect];
        [Tag("LP", "Perfect in Lenient Difficulty")]
        public static float P() => Variables.LenientCounts[HitMargin.Perfect];
        [Tag("LLP", "LatePerfect in Lenient Difficulty")]
        public static float LP() => Variables.LenientCounts[HitMargin.LatePerfect];
        [Tag("LVL", "VeryLate in Lenient Difficulty")]
        public static float VL() => Variables.LenientCounts[HitMargin.VeryLate];
        [Tag("LTL", "TooLate in Lenient Difficulty")]
        public static float TL() => Variables.LenientCounts[HitMargin.TooLate];
    }
}
