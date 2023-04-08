using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Strict
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Strict];

        private static double getCount(HitMargin margin)
            => Diff.Counts.TryGetValue(margin, out var count) ? count : 0;

        [Tag("SHit")]
        public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);

        [Tag("STE")]
        public static double TE() => getCount(HitMargin.TooEarly);

        [Tag("SVE")]
        public static double VE() => getCount(HitMargin.VeryEarly);

        [Tag("SEP")]
        public static double EP() => getCount(HitMargin.EarlyPerfect);

        [Tag("SP")]
        public static double P() => getCount(HitMargin.Perfect);

        [Tag("SLP")]
        public static double LP() => getCount(HitMargin.LatePerfect);

        [Tag("SVL")]
        public static double VL() => getCount(HitMargin.VeryLate);

        [Tag("STL")]
        public static double TL() => getCount(HitMargin.TooLate);

        [Tag("SScore")]
        public static double Score() => Diff.Score;
    }
}
