using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Normal
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Normal];

        [HitMarginTag("NHit")]
        public static class NHit
        {
            [Tag]
            public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);
        }

        [HitMarginTag("NTE")]
        public static class NTE
        {
            [Tag]
            public static double TE() => Diff.Counts[HitMargin.TooEarly];
        }
        
        [HitMarginTag("NVE")]
        public static class NVE
        {
            [Tag]
            public static double VE() => Diff.Counts[HitMargin.VeryEarly];
        }
        
        [HitMarginTag("NEP")]
        public static class NEP
        {
            [Tag]
            public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];
        }
        
        [HitMarginTag("NP")]
        public static class NP
        {
            [Tag]
            public static double P() => Diff.Counts[HitMargin.Perfect];
        }
        
        [HitMarginTag("NLP")]
        public static class NLP
        {
            [Tag]
            public static double LP() => Diff.Counts[HitMargin.LatePerfect];
        }
        
        [HitMarginTag("NVL")]
        public static class NVL
        {
            [Tag]
            public static double VL() => Diff.Counts[HitMargin.VeryLate];
        }

        [HitMarginTag("NTL")]
        public static class NTL
        {
            [Tag]
            public static double TL() => Diff.Counts[HitMargin.TooLate];
        }

        [HitMarginTag("NScore")]
        public static class NScore
        {
            [Tag]
            public static double Score() => Diff.Score;
        }
    }
}
