using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Strict
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Strict];

        [Tag("SHit")]
        public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);

        [Tag("STE")]
        public static double TE() => Diff.Counts[HitMargin.TooEarly];

        [Tag("SVE")]
        public static double VE() => Diff.Counts[HitMargin.VeryEarly];

        [Tag("SEP")]
        public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];

        [Tag("SP")]
        public static double P() => Diff.Counts[HitMargin.Perfect];

        [Tag("SLP")]
        public static double LP() => Diff.Counts[HitMargin.LatePerfect];

        [Tag("SVL")]
        public static double VL() => Diff.Counts[HitMargin.VeryLate];

        [Tag("STL")]
        public static double TL() => Diff.Counts[HitMargin.TooLate];

        [Tag("SScore")]
        public static double Score() => Diff.Score;
    }
}
