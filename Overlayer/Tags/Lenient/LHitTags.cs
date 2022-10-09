using Overlayer.Core;

namespace Overlayer.Tags.Lenient
{
    public static class LHitTags
    {
        [Tag("LHit")]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Lenient);
        [Tag("LTE")]
        public static float TE() => Variables.LenientCounts[HitMargin.TooEarly];
        [Tag("LVE")]
        public static float VE() => Variables.LenientCounts[HitMargin.VeryEarly];
        [Tag("LEP")]
        public static float EP() => Variables.LenientCounts[HitMargin.EarlyPerfect];
        [Tag("LP")]
        public static float P() => Variables.LenientCounts[HitMargin.Perfect];
        [Tag("LLP")]
        public static float LP() => Variables.LenientCounts[HitMargin.LatePerfect];
        [Tag("LVL")]
        public static float VL() => Variables.LenientCounts[HitMargin.VeryLate];
        [Tag("LTL")]
        public static float TL() => Variables.LenientCounts[HitMargin.TooLate];
    }
}
