using Overlayer.Core;

namespace Overlayer.Tags.Lenient
{
    public static class LHitTags
    {
        [Tag("LHit")]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Lenient);
        [Tag("LTE")]
        public static double TE() => Variables.LenientCounts[HitMargin.TooEarly];
        [Tag("LVE")]
        public static double VE() => Variables.LenientCounts[HitMargin.VeryEarly];
        [Tag("LEP")]
        public static double EP() => Variables.LenientCounts[HitMargin.EarlyPerfect];
        [Tag("LP")]
        public static double P() => Variables.LenientCounts[HitMargin.Perfect];
        [Tag("LLP")]
        public static double LP() => Variables.LenientCounts[HitMargin.LatePerfect];
        [Tag("LVL")]
        public static double VL() => Variables.LenientCounts[HitMargin.VeryLate];
        [Tag("LTL")]
        public static double TL() => Variables.LenientCounts[HitMargin.TooLate];
    }
}
