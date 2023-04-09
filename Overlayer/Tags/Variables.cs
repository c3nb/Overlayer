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
        [FieldTag("LScore")]
        public static int LenientScore;
        [FieldTag("NScore")]
        public static int NormalScore;
        [FieldTag("SScore")]
        public static int StrictScore;
        [FieldTag("Score")]
        public static int CurrentScore;
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
