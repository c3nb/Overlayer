using System;

namespace Overlayer.Tags.Global
{
    public static class Misc
    {
        [Tag("Pitch", "Current Pitch")]
        public static float Pitch() => GCS.currentSpeedTrial;
        [Tag("StartTile", "Start Tile")]
        public static float StartTile() => Variables.StartTile;
        [Tag("Accuracy", "Accuracy")]
        public static float Accuracy() => (float)Math.Round(scrController.instance.mistakesManager.percentAcc * 100, Settings.Instance.AccuracyDecimals);
        [Tag("Progress", "Progress")]
        public static float Progress() => (float)Math.Round((!scrLevelMaker.instance ? 0 : scrController.instance.percentComplete) * 100f, Settings.Instance.ProgressDecimals);
        [Tag("CheckPoint", "Check Point Used Count")]
        public static float CheckPoint() => scrController.checkpointsUsed;
        [Tag("CurCheckPoint", "Current Check Point Index")]
        public static float CurCheckPoint() => Variables.CurrentCheckPoint;
        [Tag("TotalCheckPoint", "Total Check Points Count")]
        public static float TotalCheckPoints() => Variables.AllCheckPoints.Count;
        [Tag("XAccuracy", "XAccuracy")]
        public static float XAccuracy()
        {
            float value = (float)Math.Round(scrController.instance.mistakesManager.percentXAcc * 100, Settings.Instance.XAccuracyDecimals);
            if (float.IsNaN(value))
                return 0;
            return value;
        }
        [Tag("FailCount", "Fail Count")]
        public static float FailCount() => Variables.FailCount;
        [Tag("MissCount", "Miss Count")]
        public static float MissCount() => scrMistakesManager.hitMarginsCount[8];
        [Tag("Overloads", "Overload Count")]
        public static float Overloads() => scrMistakesManager.hitMarginsCount[9];
        [Tag("CurBpm", "Perceived Bpm")]
        public static float CurBpm() => (float)Variables.CurBpm;
        [Tag("TileBpm", "Tile Bpm")]
        public static float TileBpm() => (float)Variables.TileBpm;
        [Tag("RecKps", "Perceived KPS")]
        public static float RecKps() => (float)Variables.RecKPS;
        [Tag("BestProgress", "Best Progress")]
        public static float BestProgress() => (float)Variables.BestProg;
        [Tag("LeastCheckPoint", "Least Check Point Used Count")]
        public static float LeastCheckPoint() => Variables.LeastChkPt;
        [Tag("StartProgress", "Start Progress")]
        public static float StartProgress() => (float)Math.Round(Variables.StartProg, Settings.Instance.StartProgDecimals);
        [Tag("CurMinute", "Now Minute Of Music")]
        public static float CurMinute() => Variables.CurMinute;
        [Tag("CurSecond", "Now Second Of Music", NumberFormat = "00")]
        public static float CurSecond() => Variables.CurSecond;
        [Tag("CurMilliSecond", "Now MilliSecond Of Music")]
        public static float CurMilliSecond() => Variables.CurMilliSecond;
        [Tag("TotalMinute", "Total Minute Of Music")]
        public static float TotalMinute() => Variables.TotalMinute;
        [Tag("TotalSecond", "Total Second Of Music", NumberFormat = "00")]
        public static float TotalSecond() => Variables.TotalSecond;
        [Tag("TotalMilliSecond", "Total MilliSecond Of Music")]
        public static float TotalMilliSecond() => Variables.TotalMilliSecond;
        [Tag("LeftTile", "Left Tile Count")]
        public static float LeftTile() => Variables.LeftTile;
        [Tag("TotalTile", "Total Tile Count")]
        public static float TotalTile() => Variables.TotalTile;
        [Tag("CurTile", "Current TIle Count")]
        public static float CurTile() => Variables.CurrentTile;
        [Tag("Attempts", "Current Level Try Count")]
        public static float Attempts() => Variables.Attempts;
        [Tag("Year", "Year Of System Time")]
        public static float Year() => DT.Now.Year;
        [Tag("Month", "Month Of System Time")]
        public static float Month() => DT.Now.Month;
        [Tag("Day", "Day Of System Time")]
        public static float Day() => DT.Now.Day;
        [Tag("Hour", "Hour Of System Time")]
        public static float Hour() => DT.Now.Hour;
        [Tag("Minute", "Minute Of System Time")]
        public static float Minute() => DT.Now.Minute;
        [Tag("Second", "Second Of System Time")]
        public static float Second() => DT.Now.Second;
        [Tag("MilliSecond", "MilliSecond Of System Time")]
        public static float MilliSecond() => DT.Now.Millisecond;
        [Tag("Multipress", "Multipress Count")]
        public static float Multipress() => Variables.MultipressCount;
        [Tag("Fps", "Frame Rate")]
        public static float Fps() => (float)Math.Round(Variables.Fps, Settings.Instance.FPSDecimals);
        [Tag("FrameTime", "FrameTime")]
        public static float FrameTime() => (float)Math.Round(Variables.FrameTime, Settings.Instance.FrametimeDecimals);
    }
}
