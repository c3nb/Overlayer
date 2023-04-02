using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Lenient
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Lenient];

        [HitMarginTag("LHit")]
        public static class LHit
        {
            [Tag]
            public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);
        }

        [HitMarginTag("LTE")]
        public static class LTE
        {
            [Tag]
            public static double TE() => Diff.Counts[HitMargin.TooEarly];
        }

        [HitMarginTag("LVE")]
        public static class LVE
        {
            [Tag]
            public static double VE() => Diff.Counts[HitMargin.VeryEarly];
        }

        [HitMarginTag("LEP")]
        public static class LEP
        {
            [Tag]
            public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];
        }

        [HitMarginTag("LP")]
        public static class LP
        {
            [Tag]
            public static double P() => Diff.Counts[HitMargin.Perfect];
        }

        [HitMarginTag("LLP")]
        public static class LLP
        {
            [Tag]
            public static double LP() => Diff.Counts[HitMargin.LatePerfect];
        }

        [HitMarginTag("LVL")]
        public static class LVL
        {
            [Tag]
            public static double VL() => Diff.Counts[HitMargin.VeryLate];
        }

        [HitMarginTag("LTL")]
        public static class LTL
        {
            [Tag]
            public static double TL() => Diff.Counts[HitMargin.TooLate];
        }

        [HitMarginTag("LScore")]
        public static class LScore
        {
            [Tag]
            public static double Score() => Diff.Score;
        }
    }
}
