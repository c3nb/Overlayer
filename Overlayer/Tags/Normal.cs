using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Normal
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Normal];

        private static double getCount(HitMargin margin)
            => Diff.Counts.TryGetValue(margin, out var count) ? count : 0;

        [Tag("NHit")]
        public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);

        [Tag("NTE")]
        public static double TE() => getCount(HitMargin.TooEarly);

        [Tag("NVE")]
        public static double VE() => getCount(HitMargin.VeryEarly);

        [Tag("NEP")]
        public static double EP() => getCount(HitMargin.EarlyPerfect);

        [Tag("NP")]
        public static double P() => getCount(HitMargin.Perfect);

        [Tag("NLP")]
        public static double LP() => getCount(HitMargin.LatePerfect);

        [Tag("NVL")]
        public static double VL() => getCount(HitMargin.VeryLate);

        [Tag("NTL")]
        public static double TL() => getCount(HitMargin.TooLate);

        [Tag("NScore")]
        public static double Score() => Diff.Score;
    }
}
