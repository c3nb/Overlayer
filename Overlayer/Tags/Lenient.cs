using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Lenient
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Lenient];

        private static double getCount(HitMargin margin)
            => Diff.Counts.TryGetValue(margin, out var count) ? count : 0;

        [Tag("LHit")]
        public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);

        [Tag("LTE")]
        public static double TE() => getCount(HitMargin.TooEarly);

        [Tag("LVE")]
        public static double VE() => getCount(HitMargin.VeryEarly);

        [Tag("LEP")]
        public static double EP() => getCount(HitMargin.EarlyPerfect);

        [Tag("LP")]
        public static double P() => getCount(HitMargin.Perfect);

        [Tag("LLP")]
        public static double LP() => getCount(HitMargin.LatePerfect);

        [Tag("LVL")]
        public static double VL() => Diff.Counts[HitMargin.VeryLate];

        [Tag("LTL")]
        public static double TL() => Diff.Counts[HitMargin.TooLate];

        [Tag("LScore")]
        public static double Score() => Diff.Score;
    }
}
