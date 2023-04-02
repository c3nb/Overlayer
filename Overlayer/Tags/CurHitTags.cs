using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class CurHitTags
    {
        public static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[GCS.difficulty];

        [HitMarginTag("CurHit")]
        public static class CurHit
        {
            [Tag]
            public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);
        }

        [HitMarginTag("CTE")]
        public static class CTE
        {
            [Tag]
            public static double TE() => Diff.Counts[HitMargin.TooEarly];
        }

        [HitMarginTag("CVE")]
        public static class CVE
        {
            [Tag]
            public static double VE() => Diff.Counts[HitMargin.VeryEarly];
        }

        [HitMarginTag("CEP")]
        public static class CEP
        {
            [Tag]
            public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];
        }

        [HitMarginTag("CP")]
        public static class CP
        {
            [Tag]
            public static double P() => Diff.Counts[HitMargin.Perfect];
        }

        [HitMarginTag("CLP")]
        public static class CLP
        {
            [Tag]
            public static double LP() => Diff.Counts[HitMargin.LatePerfect];
        }

        [HitMarginTag("CVL")]
        public static class CVL
        {
            [Tag]
            public static double VL() => Diff.Counts[HitMargin.VeryLate];
        }

        [HitMarginTag("CTL")]
        public static class CTL
        {
            [Tag]
            public static double TL() => Diff.Counts[HitMargin.TooLate];
        }

        [HitMarginTag("Score")]
        public static class CScore
        {
            [Tag]
            public static double Score() => Diff.Score;
        }
    }
}
