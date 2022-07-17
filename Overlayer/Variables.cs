using System;
using System.Collections.Generic;

namespace Overlayer
{
    public static class Variables
    {
        public static bool IsStarted = true;
        public static readonly HitMargin[] HitMargins = (HitMargin[])Enum.GetValues(typeof(HitMargin));
        public static double Timing;
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

        public static int MultipressCount;

        public static List<scrFloor> AllCheckPoints;
        public static int CurrentCheckPoint;

        public static int FailCount;
        public static int Combo;

        public static double TileBpm;
        public static double CurBpm;
        public static double RecKPS;

        public static double StartProg;
        public static int StartTile;
        public static double BestProg;
        public static int LeastChkPt;

        public static int CurMinute;
        public static int CurSecond;
        public static int CurMilliSecond;
        public static int TotalMinute;
        public static int TotalSecond;
        public static int TotalMilliSecond;

        public static int CurrentTile;
        public static int LeftTile;
        public static int TotalTile;

        public static int Attempts;
        public static int KpsTemp;
        public static float Fps;
        public static float FrameTime;

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
            MultipressCount = 0;
            TileBpm = 0;
            CurBpm = 0;
            RecKPS = 0;
            Combo = 0;
            AllCheckPoints = new List<scrFloor>();
            CurrentCheckPoint = 0;
        }
    }
}
