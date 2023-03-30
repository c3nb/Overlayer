using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public class Lenient
    {
        private static GetHitMarginFixer.PerDiff Diff =>
            GetHitMarginFixer.diff[Difficulty.Lenient];

        [Tag("LHit")]
        public static string Hit() => RDString.Get("HitMargin." + Diff.NowMargin);
        [Tag("LTE")]
        public static double TE() => Diff.Counts[HitMargin.TooEarly];
        [Tag("LVE")]
        public static double VE() => Diff.Counts[HitMargin.VeryEarly];
        [Tag("LEP")]
        public static double EP() => Diff.Counts[HitMargin.EarlyPerfect];
        [Tag("LP")]
        public static double P() => Diff.Counts[HitMargin.Perfect];
        [Tag("LLP")]
        public static double LP() => Diff.Counts[HitMargin.LatePerfect];
        [Tag("LVL")]
        public static double VL() => Diff.Counts[HitMargin.VeryLate];
        [Tag("LTL")]
        public static double TL() => Diff.Counts[HitMargin.TooLate];
    }
}
