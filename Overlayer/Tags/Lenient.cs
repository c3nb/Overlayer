using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Lenient
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Lenient];

        [ClassTag("LHit")]
        public static class LHit
        {
            [Tag]
            public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);
        }

        [ClassTag("LTE")]
        public static class LTE
        {
            [Tag]
            public static double TE() => Diff.Counts[HitMargin.TooEarly];
        }

        [ClassTag("LVE")]
        public static class LVE
        {
            [Tag]
            public static double VE() => Diff.Counts[HitMargin.VeryEarly];
        }

        [ClassTag("LEP")]
        public static class LEP
        {
            [Tag]
            public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];
        }

        [ClassTag("LP")]
        public static class LP
        {
            [Tag]
            public static double P() => Diff.Counts[HitMargin.Perfect];
        }

        [ClassTag("LLP")]
        public static class LLP
        {
            [Tag]
            public static double LP() => Diff.Counts[HitMargin.LatePerfect];
        }

        [ClassTag("LVL")]
        public static class LVL
        {
            [Tag]
            public static double VL() => Diff.Counts[HitMargin.VeryLate];
        }

        [ClassTag("LTL")]
        public static class LTL
        {
            [Tag]
            public static double TL() => Diff.Counts[HitMargin.TooLate];
        }

        [ClassTag("LScore")]
        public static class LScore
        {
            [Tag]
            public static double Score() => Diff.Score;
        }
    }
}
