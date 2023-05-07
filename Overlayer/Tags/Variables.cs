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
        public static bool IsStarted;
        public static int Combo;
        [FieldTag("LScore", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static int LenientScore;
        [FieldTag("NScore", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static int NormalScore;
        [FieldTag("SScore", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static int StrictScore;
        [FieldTag("Score", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static int CurrentScore;
        [FieldTag("Timing", Round = true, RelatedPatches = "Overlayer.Patches.TimingUpdater:Prefix", Category = Category.Play)]
        public static double Timing = 0;
        [FieldTag("Attempts", RelatedPatches = "Overlayer.Patches.AttemptsCounter:FCLLPostfix|Overlayer.Patches.AttemptsCounter:Postfix|Overlayer.Patches.DataInit:Postfix", Category = Category.Play)]
        public static int AttemptsCount = 0;
        [FieldTag("FailCount", RelatedPatches = "Overlayer.Patches.AttemptsCounter:FAPostfix", Category = Category.HitMargin)]
        public static int FailCount = 0;
        [FieldTag("StartProgress", Round = true, RelatedPatches = "Overlayer.Patches.BpmUpdater+BossLevelStart:Postfix|Overlayer.Patches.StartProgUpdater:Prefix", Category = Category.AccProg)]
        public static double StartProg = 0;
        [FieldTag("BestProgress", Round = true, RelatedPatches = "Overlayer.Patches.AttemptsCounter:FAPostfix|Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.AccProg)]
        public static double BestProg = 0;
        [FieldTag("CurMinute", RelatedPatches = "Overlayer.Patches.Updater:Prefix", Category = Category.Song)]
        public static int CurMinute;
        [FieldTag("CurSecond", RelatedPatches = "Overlayer.Patches.Updater:Prefix", Category = Category.Song)]
        public static int CurSecond;
        [FieldTag("CurMilliSecond", RelatedPatches = "Overlayer.Patches.Updater:Prefix", Category = Category.Song)]
        public static int CurMilliSecond;
        [FieldTag("TotalMinute", RelatedPatches = "Overlayer.Patches.Updater:Prefix", Category = Category.Song)]
        public static int TotalMinute;
        [FieldTag("TotalSecond", RelatedPatches = "Overlayer.Patches.Updater:Prefix", Category = Category.Song)]
        public static int TotalSecond;
        [FieldTag("TotalMilliSecond", RelatedPatches = "Overlayer.Patches.Updater:Prefix", Category = Category.Song)]
        public static int TotalMilliSecond;
        [FieldTag("CurCheckPoint", RelatedPatches = "Overlayer.Patches.BpmUpdater+MoveToNextFloorPatch:Postfix", Category = Category.CheckPoint)]
        public static int CurrentCheckPoint;
        [FieldTag("StartTile", RelatedPatches = "Overlayer.Patches.StartProgUpdater:Prefix", Category = Category.Tile)]
        public static int StartTile;
        [FieldTag("LeftTile", RelatedPatches = "Overlayer.Patches.Updater:Prefix", Category = Category.Tile)]
        public static int LeftTile;
        [FieldTag("CurTile", RelatedPatches = "Overlayer.Patches.Updater:Prefix", Category = Category.Tile)]
        public static int CurrentTile;
        [FieldTag("TotalTile", RelatedPatches = "Overlayer.Patches.Updater:Prefix", Category = Category.Tile)]
        public static int TotalTile;
        [FieldTag("TileBpm", Round = true, RelatedPatches = "Overlayer.Patches.BpmUpdater+CustomLevelStart:Postfix|Overlayer.Patches.BpmUpdater+BossLevelStart:Postfix|Overlayer.Patches.BpmUpdater+MoveToNextFloorPatch:Postfix", Category = Category.BpmKps)]
        public static double TileBpm;
        [FieldTag("CurBpm", Round = true, RelatedPatches = "Overlayer.Patches.BpmUpdater+CustomLevelStart:Postfix|Overlayer.Patches.BpmUpdater+BossLevelStart:Postfix|Overlayer.Patches.BpmUpdater+MoveToNextFloorPatch:Postfix", Category = Category.BpmKps)]
        public static double CurBpm;
        [FieldTag("RecKPS", Round = true, RelatedPatches = "Overlayer.Patches.BpmUpdater+CustomLevelStart:Postfix|Overlayer.Patches.BpmUpdater+BossLevelStart:Postfix|Overlayer.Patches.BpmUpdater+MoveToNextFloorPatch:Postfix", Category = Category.BpmKps)]
        public static double RecKPS;
        [FieldTag(Round = true, NotPlaying = true, Category = Category.Misc)]
        public static double FrameTime;
        [FieldTag(Round = true, NotPlaying = true, Category = Category.Misc)]
        public static double Fps;
        [FieldTag("Multipress", Category = Category.HitMargin)]
        public static int MultipressCount;
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
            FailCount = 0;
            StartTile = 0;
            TileBpm = CurBpm = RecKPS = 0;
            FrameTime = Fps = 0;
            MultipressCount = 0;
        }
    }
}
