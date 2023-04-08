using Overlayer.Core.Tags;

namespace Overlayer.Tags
{
    public static class NHitTags
    {
        [Tag("NHit")]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Normal);
        [Tag("NTE")]
        public static double TE() => Variables.NormalCounts[HitMargin.TooEarly];
        [Tag("NVE")]
        public static double VE() => Variables.NormalCounts[HitMargin.VeryEarly];
        [Tag("NEP")]
        public static double EP() => Variables.NormalCounts[HitMargin.EarlyPerfect];
        [Tag("NP")]
        public static double P() => Variables.NormalCounts[HitMargin.Perfect];
        [Tag("NLP")]
        public static double LP() => Variables.NormalCounts[HitMargin.LatePerfect];
        [Tag("NVL")]
        public static double VL() => Variables.NormalCounts[HitMargin.VeryLate];
        [Tag("NTL")]
        public static double TL() => Variables.NormalCounts[HitMargin.TooLate];
    }
}
