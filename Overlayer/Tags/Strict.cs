using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Strict
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Strict];

        [ClassTag("SHit")]
        public static class SHit
        {
            [Tag]
            public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);
        }

        [ClassTag("STE")]
        public static class STE
        {
            [Tag]
            public static double TE() => Diff.Counts[HitMargin.TooEarly];
        }

        [ClassTag("SVE")]
        public static class SVE
        {
            [Tag]
            public static double VE() => Diff.Counts[HitMargin.VeryEarly];
        }
        
        [ClassTag("SEP")]
        public static class SEP
        {
            [Tag]
            public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];
        }
        
        [ClassTag("SP")]
        public static class SP
        {
            [Tag]
            public static double P() => Diff.Counts[HitMargin.Perfect];
        }
        
        [ClassTag("SLP")]
        public static class SLP
        {
            [Tag]
            public static double LP() => Diff.Counts[HitMargin.LatePerfect];
        }
        
        [ClassTag("SVL")]
        public static class SVL
        {
            [Tag]
            public static double VL() => Diff.Counts[HitMargin.VeryLate];
        }
        
        [ClassTag("STL")]
        public static class STL
        {
            [Tag]
            public static double TL() => Diff.Counts[HitMargin.TooLate];
        }

        [ClassTag("SScore")]
        public static class SScore
        {
            [Tag]
            public static double Score() => Diff.Score;
        }
    }
}
