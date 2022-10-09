using Overlayer.Core;

namespace Overlayer.Tags.Global
{
    public static class CurHitTags
    {
        [Tag("CurHit")]
        public static string Hit() => RDString.Get("HitMargin." + GetCurHitMargin(GCS.difficulty));
        [Tag("CurTE")]
        public static float TE() => GetCurDiffCount(HitMargin.TooEarly);
        [Tag("CurVE")]
        public static float VE() => GetCurDiffCount(HitMargin.VeryEarly);
        [Tag("CurEP")]
        public static float EP() => GetCurDiffCount(HitMargin.EarlyPerfect);
        [Tag("CurP")]
        public static float P() => GetCurDiffCount(HitMargin.Perfect);
        [Tag("CurLP")]
        public static float LP() => GetCurDiffCount(HitMargin.LatePerfect);
        [Tag("CurVL")]
        public static float VL() => GetCurDiffCount(HitMargin.VeryLate);
        [Tag("CurTL")]
        public static float TL() => GetCurDiffCount(HitMargin.TooLate);
        [Tag("CurDifficulty")]
        public static string Difficulty() => RDString.Get("enum.Difficulty." + GCS.difficulty);
        public static int GetCurDiffCount(HitMargin hit)
        {
            switch (GCS.difficulty)
            {
                case global::Difficulty.Lenient: return Variables.LenientCounts[hit];
                case global::Difficulty.Normal: return Variables.NormalCounts[hit];
                case global::Difficulty.Strict: return Variables.StrictCounts[hit];
                default: return 0;
            }
        }
        public static HitMargin GetCurHitMargin(Difficulty diff)
        {
            switch (diff)
            {
                case global::Difficulty.Lenient: return Variables.Lenient;
                case global::Difficulty.Normal: return Variables.Normal;
                case global::Difficulty.Strict: return Variables.Strict;
                default: return 0;
            }
        }
    }
}
