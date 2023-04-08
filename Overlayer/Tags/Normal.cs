using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Normal
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Normal];

        [ClassTag("NHit")]
        public static class NHit
        {
            [Tag]
            public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);
        }

        [ClassTag("NTE")]
        public static class NTE
        {
            [Tag]
            public static double TE() => Diff.Counts[HitMargin.TooEarly];
        }
        
        [ClassTag("NVE")]
        public static class NVE
        {
            [Tag]
            public static double VE() => Diff.Counts[HitMargin.VeryEarly];
        }
        
        [ClassTag("NEP")]
        public static class NEP
        {
            [Tag]
            public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];
        }
        
        [ClassTag("NP")]
        public static class NP
        {
            [Tag]
            public static double P() => Diff.Counts[HitMargin.Perfect];
        }
        
        [ClassTag("NLP")]
        public static class NLP
        {
            [Tag]
            public static double LP() => Diff.Counts[HitMargin.LatePerfect];
        }
        
        [ClassTag("NVL")]
        public static class NVL
        {
            [Tag]
            public static double VL() => Diff.Counts[HitMargin.VeryLate];
        }

        [ClassTag("NTL")]
        public static class NTL
        {
            [Tag]
            public static double TL() => Diff.Counts[HitMargin.TooLate];
        }

        [ClassTag("NScore")]
        public static class NScore
        {
            [Tag]
            public static double Score() => Diff.Score;
        }
    }
}
