using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class CurHitTags
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[GCS.difficulty];

        [Tag("CurHit")]
        public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);
        [Tag("CTE")]
        public static double TE() => Diff.Counts[HitMargin.TooEarly];
        [Tag("CVE")]
        public static double VE() => Diff.Counts[HitMargin.VeryEarly];
        [Tag("CEP")]
        public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];
        [Tag("CP")]
        public static double P() => Diff.Counts[HitMargin.Perfect];
        [Tag("CLP")]
        public static double LP() => Diff.Counts[HitMargin.LatePerfect];
        [Tag("CVL")]
        public static double VL() => Diff.Counts[HitMargin.VeryLate];
        [Tag("CTL")]
        public static double TL() => Diff.Counts[HitMargin.TooLate];
    }
}
