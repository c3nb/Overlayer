using Overlayer.Core;

namespace Overlayer.Tags.Normal
{
    public static class NHitTags
    {
        [Tag("NHit")]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Normal);
        [Tag("NTE")]
        public static float TE() => Variables.NormalCounts[HitMargin.TooEarly];
        [Tag("NVE")]
        public static float VE() => Variables.NormalCounts[HitMargin.VeryEarly];
        [Tag("NEP")]
        public static float EP() => Variables.NormalCounts[HitMargin.EarlyPerfect];
        [Tag("NP")]
        public static float P() => Variables.NormalCounts[HitMargin.Perfect];
        [Tag("NLP")]
        public static float LP() => Variables.NormalCounts[HitMargin.LatePerfect];
        [Tag("NVL")]
        public static float VL() => Variables.NormalCounts[HitMargin.VeryLate];
        [Tag("NTL")]
        public static float TL() => Variables.NormalCounts[HitMargin.TooLate];
    }
}
