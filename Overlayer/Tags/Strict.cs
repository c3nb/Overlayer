using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Strict
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Strict];

        [HitMarginTag("SHit")]
        public static class SHit
        {
            [Tag]
            public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);
        }

        [HitMarginTag("STE")]
        public static class STE
        {
            [Tag]
            public static double TE() => Diff.Counts[HitMargin.TooEarly];
        }

        [HitMarginTag("SVE")]
        public static class SVE
        {
            [Tag]
            public static double VE() => Diff.Counts[HitMargin.VeryEarly];
        }
        
        [HitMarginTag("SEP")]
        public static class SEP
        {
            [Tag]
            public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];
        }
        
        [HitMarginTag("SP")]
        public static class SP
        {
            [Tag]
            public static double P() => Diff.Counts[HitMargin.Perfect];
        }
        
        [HitMarginTag("SLP")]
        public static class SLP
        {
            [Tag]
            public static double LP() => Diff.Counts[HitMargin.LatePerfect];
        }
        
        [HitMarginTag("SVL")]
        public static class SVL
        {
            [Tag]
            public static double VL() => Diff.Counts[HitMargin.VeryLate];
        }
        
        [HitMarginTag("STL")]
        public static class STL
        {
            [Tag]
            public static double TL() => Diff.Counts[HitMargin.TooLate];
        }

        [HitMarginTag("SScore")]
        public static class SScore
        {
            [Tag]
            public static double Score() => Diff.Score;
        }
    }
}
