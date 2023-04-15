using Overlayer.Core.Tags;
using System;
using System.Collections.Generic;

namespace Overlayer.Tags
{
    public static class Variables
    {
        public static readonly HitMargin[] HitMargins = (HitMargin[])Enum.GetValues(typeof(HitMargin));
        public static readonly Dictionary<HitMargin, int> LenientCounts = new Dictionary<HitMargin, int>();
        public static readonly Dictionary<HitMargin, int> NormalCounts = new Dictionary<HitMargin, int>();
        public static readonly Dictionary<HitMargin, int> StrictCounts = new Dictionary<HitMargin, int>();
        public static HitMargin Lenient;
        public static HitMargin Normal;
        public static HitMargin Strict;
        public static int Combo;
        [FieldTag("LScore", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static int LenientScore;
        [FieldTag("NScore", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static int NormalScore;
        [FieldTag("SScore", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static int StrictScore;
        [FieldTag("Score", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static int CurrentScore;
        [FieldTag("Timing", Round = true,
            RelatedPatches = "Overlayer.Patches.TimingUpdater:Prefix")]
        public static double Timing = 0;
        [FieldTag("Attempts", RelatedPatches = "Overlayer.Patches.AttemptsCounter:FCLLPostfix|Overlayer.Patches.AttemptsCounter:Postfix|Overlayer.Patches.DataInit:Postfix")]
        public static int AttemptsCount = 0;
        [FieldTag("FailCount", RelatedPatches = "Overlayer.Patches.AttemptsCounter:FAPostfix")]
        public static int FailCount = 0;
        [FieldTag("BestProgress", Round = true, RelatedPatches = "Overlayer.Patches.AttemptsCounter:FAPostfix")]
        public static double BestProg = 0;
        public static void Reset()
        {
            foreach (HitMargin h in HitMargins)
            {
                LenientCounts[h] = 0;
                NormalCounts[h] = 0;
                StrictCounts[h] = 0;
            }
            LenientScore = 0;
            NormalScore = 0;
            StrictScore = 0;
            CurrentScore = 0;
            Combo = 0;
        }
    }
}
