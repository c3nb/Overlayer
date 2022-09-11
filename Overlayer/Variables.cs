using System;
using System.Collections.Generic;
using Overlayer.Tags.Global;
using UnityEngine;

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

        public static readonly Color tooEarlyColor = new Color(1.000f, 0.000f, 0.000f, 1.000f); //FF0000FF
        public static readonly Color veryEarlyColor = new Color(1.000f, 0.436f, 0.306f, 1.000f); //FF6F4EFF
        public static readonly Color earlyPerfectColor = new Color(0.627f, 1.000f, 0.306f, 1.000f); //A0FF4EFF
        public static readonly Color perfectColor = new Color(0.376f, 1.000f, 0.307f, 1.000f); //60FF4EFF
        public static readonly Color latePerfectColor = new Color(0.627f, 1.000f, 0.306f, 1.000f); //A0FF4EFF
        public static readonly Color veryLateColor = new Color(1.000f, 0.435f, 0.306f, 1.000f); //FF6F4EFF
        public static readonly Color tooLateColor = new Color(1.000f, 0.000f, 0.000f, 1.000f); //FF0000FF
        public static readonly Color multipressColor = new Color(0.000f, 1.000f, 0.930f, 1.000f); //00FFEDFF
        public static readonly Color failMissColor = new Color(0.851f, 0.346f, 1.000f, 1.000f); //D958FFFF
        public static readonly Color failOverloadColor = new Color(0.851f, 0.346f, 1.000f, 1.000f); //D958FFFF

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
            KeyJudgeTag.keyJudges = new KeyJudge[KeyJudgeTag.lineCount];
        }
    }
}
