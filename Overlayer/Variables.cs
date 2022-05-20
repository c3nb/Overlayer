using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer
{
    public static class Variables
    {
        public static readonly HitMargin[] HitMargins = (HitMargin[])Enum.GetValues(typeof(HitMargin));
        public static double Timing;
        public static float Angle;
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
        public static int FailCount;
        public static int Combo;

        public static double TileBpm;
        public static double CurBpm;
        public static double RecKPS;

        public static double StartProg;
        public static double BestProg;
        public static int LeastChkPt;

        public static int CurMinute;
        public static int CurSecond;
        public static int TotalMinute;
        public static int TotalSecond;

        public static int CurrentTile;
        public static int LeftTile;
        public static int TotalTile;

        public static int Attempts;
        public static float Fps;

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
            FailCount = 0;
            TileBpm = 0;
            CurBpm = 0;
            RecKPS = 0;
            Combo = 0;
        }
    }
}
