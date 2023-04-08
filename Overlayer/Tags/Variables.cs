using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static int LenientScore;
        public static int NormalScore;
        public static int StrictScore;
        public static int CurrentScore;
        public static int Combo;
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
