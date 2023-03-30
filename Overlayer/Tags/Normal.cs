using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public class Normal
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Normal];

        [Tag("NHit")]
        public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);
        [Tag("NTE")]
        public static double TE() => Diff.Counts[HitMargin.TooEarly];
        [Tag("NVE")]
        public static double VE() => Diff.Counts[HitMargin.VeryEarly];
        [Tag("NEP")]
        public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];
        [Tag("NP")]
        public static double P() => Diff.Counts[HitMargin.Perfect];
        [Tag("NLP")]
        public static double LP() => Diff.Counts[HitMargin.LatePerfect];
        [Tag("NVL")]
        public static double VL() => Diff.Counts[HitMargin.VeryLate];
        [Tag("NTL")]
        public static double TL() => Diff.Counts[HitMargin.TooLate];
    }
}
