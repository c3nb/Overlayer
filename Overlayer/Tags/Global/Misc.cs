using TagLib.Tags;
using TagLib.Utils;

namespace Overlayer.Tags.Global
{
    public static class Misc
    {
        [Tag("Pitch", "Current Pitch")]
        public static float Pitch(float digits = -1) => GCS.currentSpeedTrial.Round(digits);
        [Tag("EditorPitch", "Pitch In Editor")]
        public static string EditorPitch() => (scnEditor.instance?.levelData?.pitch / 100.0).ToString();
        [Tag("Title", "Title")]
        public static string Title() => scnEditor.instance?.levelData?.song.BreakRichTag();
        [Tag("Author", "Author")]
        public static string Author() => scnEditor.instance?.levelData?.author.BreakRichTag();
        [Tag("Artist", "Artist")]
        public static string Artist() => scnEditor.instance?.levelData?.artist.BreakRichTag();
        [Tag("StartTile", "Start Tile")]
        public static float StartTile() => Variables.StartTile;
        [Tag("Accuracy", "Accuracy")]
        public static float Accuracy(float digits = -1) => (scrController.instance.mistakesManager.percentAcc * 100).Round(digits);
        [Tag("Progress", "Progress")]
        public static float Progress(float digits = -1) => ((!scrLevelMaker.instance ? 0 : scrController.instance.percentComplete) * 100f).Round(digits);
        [Tag("CheckPoint", "Check Point Used Count")]
        public static float CheckPoint() => scrController.checkpointsUsed;
        [Tag("CurCheckPoint", "Current Check Point Index")]
        public static float CurCheckPoint() => Variables.CurrentCheckPoint;
        [Tag("TotalCheckPoint", "Total Check Points Count")]
        public static float TotalCheckPoints() => Variables.AllCheckPoints.Count;
        [Tag("XAccuracy", "XAccuracy")]
        public static float XAccuracy(float digits = -1) => (scrController.instance.mistakesManager.percentXAcc * 100).Round(digits);
        [Tag("FailCount", "Fail Count")]
        public static float FailCount() => Variables.FailCount;
        [Tag("MissCount", "Miss Count")]
        public static float MissCount() => scrMistakesManager.hitMarginsCount[8];
        [Tag("Overloads", "Overload Count")]
        public static float Overloads() => scrMistakesManager.hitMarginsCount[9];
        [Tag("CurBpm", "Perceived Bpm")]
        public static float CurBpm(float digits = -1) => Variables.CurBpm.Round(digits);
        [Tag("TileBpm", "Tile Bpm")]
        public static float TileBpm(float digits = -1) => Variables.TileBpm.Round(digits);
        [Tag("RecKps", "Perceived KPS")]
        public static float RecKps(float digits = -1) => Variables.RecKPS.Round(digits);
        [Tag("BestProgress", "Best Progress")]
        public static float BestProgress(float digits = -1) => Variables.BestProg.Round(digits);
        [Tag("LeastCheckPoint", "Least Check Point Used Count")]
        public static float LeastCheckPoint() => Variables.LeastChkPt;
        [Tag("StartProgress", "Start Progress")]
        public static float StartProgress(float digits = -1) => Variables.StartProg.Round(digits);
        [Tag("CurMinute", "Now Minute Of Music")]
        public static float CurMinute() => Variables.CurMinute;
        [Tag("CurSecond", "Now Second Of Music")]
        public static float CurSecond() => Variables.CurSecond;
        [Tag("CurMilliSecond", "Now MilliSecond Of Music")]
        public static float CurMilliSecond() => Variables.CurMilliSecond;
        [Tag("TotalMinute", "Total Minute Of Music")]
        public static float TotalMinute() => Variables.TotalMinute;
        [Tag("TotalSecond", "Total Second Of Music")]
        public static float TotalSecond() => Variables.TotalSecond;
        [Tag("TotalMilliSecond", "Total MilliSecond Of Music")]
        public static float TotalMilliSecond() => Variables.TotalMilliSecond;
        [Tag("LeftTile", "Left Tile Count")]
        public static float LeftTile() => Variables.LeftTile;
        [Tag("TotalTile", "Total Tile Count")]
        public static float TotalTile() => Variables.TotalTile;
        [Tag("CurTile", "Current Tile Count")]
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
        [Tag("Combo", "Combo")]
        public static float Combo() => Variables.Combo;
        [Tag("Fps", "Frame Rate")]
        public static float Fps(float digits = -1) => Variables.Fps.Round(digits);
        [Tag("FrameTime", "FrameTime")]
        public static float FrameTime(float digits = -1) => Variables.FrameTime.Round(digits);
    }
}
