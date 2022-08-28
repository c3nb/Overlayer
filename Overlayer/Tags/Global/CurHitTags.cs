using TagLib.Tags;

namespace Overlayer.Tags.Global
{
    public static class CurHitTags
    {
        [Tag("CurHit", "HitMargin in Current Difficulty")]
        public static string Hit() => RDString.Get("HitMargin." + GetCurHitMargin(GCS.difficulty));
        [Tag("CurTE", "TooEarly in Current Difficulty")]
        public static float TE() => GetCurDiffCount(HitMargin.TooEarly);
        [Tag("CurVE", "VeryEarly in Current Difficulty")]
        public static float VE() => GetCurDiffCount(HitMargin.VeryEarly);
        [Tag("CurEP", "EarlyPerfect in Current Difficulty")]
        public static float EP() => GetCurDiffCount(HitMargin.EarlyPerfect);
        [Tag("CurP", "Perfect in Current Difficulty")]
        public static float P() => GetCurDiffCount(HitMargin.Perfect);
        [Tag("CurLP", "LatePerfect in Current Difficulty")]
        public static float LP() => GetCurDiffCount(HitMargin.LatePerfect);
        [Tag("CurVL", "VeryLate in Current Difficulty")]
        public static float VL() => GetCurDiffCount(HitMargin.VeryLate);
        [Tag("CurTL", "TooLate in Current Difficulty")]
        public static float TL() => GetCurDiffCount(HitMargin.TooLate);
        [Tag("CurDifficulty", "Current Difficulty")]
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
