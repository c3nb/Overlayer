using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class CurHitTags
    {
        public static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[GCS.difficulty];

        private static double getCount(HitMargin margin)
            => Diff.Counts.TryGetValue(margin, out var count) ? count : 0;

        [Tag("CurHit")]
        public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);

        [Tag("CTE")]
        public static double TE() => getCount(HitMargin.TooEarly);

        [Tag("CVE")]
        public static double VE() => getCount(HitMargin.VeryEarly);

        [Tag("CEP")]
        public static double EP() => getCount(HitMargin.EarlyPerfect);

        [Tag("CP")]
        public static double P() => getCount(HitMargin.Perfect);

        [Tag("CLP")]
        public static double LP() => getCount(HitMargin.LatePerfect);

        [Tag("CVL")]
        public static double VL() => getCount(HitMargin.VeryLate);

        [Tag("CTL")]
        public static double TL() => getCount(HitMargin.TooLate);

        [Tag("CScore")]
        public static double Score() => Diff.Score;
    }
}
