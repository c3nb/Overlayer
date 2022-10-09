using Overlayer.Core;
using Overlayer.Core.Utils;

namespace Overlayer.Tags.Global
{
    public static class Misc
    {
        [Tag("Radius")]
        public static float Radius(float digits = -1) => scrController.instance.chosenplanet.cosmeticRadius.Round(digits);
        [Tag("Pitch")]
        public static float Pitch(float digits = -1) => GCS.currentSpeedTrial.Round(digits);
        [Tag("EditorPitch")]
        public static float EditorPitch(float digits = -1) => (scnEditor.instance.levelData.pitch / 100.0).Round(digits);
        [Tag("TEHex")]
        public static string TEHex() => "FF0000FF";
        [Tag("VEHex")]
        public static string VEHex() => "FF6F4EFF";
        [Tag("EPHex")]
        public static string EPHex() => "A0FF4EFF";
        [Tag("PHex")]
        public static string PHex() => "60FF4EFF";
        [Tag("LPHex")]
        public static string LPHex() => "A0FF4EFF";
        [Tag("VLHex")]
        public static string VLHex() => "FF6F4EFF";
        [Tag("TLHex")]
        public static string TLHex() => "FF0000FF";
        [Tag("MPHex")]
        public static string MPHex() => "00FFEDFF";
        [Tag("FMHex")]
        public static string FMHex() => "D958FFFF";
        [Tag("FOHex")]
        public static string FOHex() => "D958FFFF";
        [Tag("Title")]
        public static string Title() => scnEditor.instance?.levelData?.song.BreakRichTagWithoutSize();
        [Tag("Author")]
        public static string Author() => scnEditor.instance?.levelData?.author.BreakRichTagWithoutSize();
        [Tag("Artist")]
        public static string Artist() => scnEditor.instance?.levelData?.artist.BreakRichTagWithoutSize();
        [Tag("StartTile")]
        public static float StartTile() => Variables.StartTile;
        [Tag("Difficulty")]
        public static float Difficulty(float digits = -1) => PlayPoint.CurrentDifficulty.Round(digits);
        [Tag("Accuracy")]
        public static float Accuracy(float digits = -1) => (scrController.instance.mistakesManager.percentAcc * 100).Round(digits);
        [Tag("Progress")]
        public static float Progress(float digits = -1) => ((!scrLevelMaker.instance ? 0 : scrController.instance.percentComplete) * 100f).Round(digits);
        [Tag("CheckPoint")]
        public static float CheckPoint() => scrController.checkpointsUsed;
        [Tag("CurCheckPoint")]
        public static float CurCheckPoint() => Variables.CurrentCheckPoint;
        [Tag("TotalCheckPoint")]
        public static float TotalCheckPoints() => Variables.AllCheckPoints.Count;
        [Tag("XAccuracy")]
        public static float XAccuracy(float digits = -1) => (scrController.instance.mistakesManager.percentXAcc * 100).Round(digits);
        [Tag("FailCount")]
        public static float FailCount() => Variables.FailCount;
        [Tag("MissCount")]
        public static float MissCount() => scrMistakesManager.hitMarginsCount[8];
        [Tag("Overloads")]
        public static float Overloads() => scrMistakesManager.hitMarginsCount[9];
        [Tag("CurBpm")]
        public static float CurBpm(float digits = -1) => Variables.CurBpm.Round(digits);
        [Tag("TileBpm")]
        public static float TileBpm(float digits = -1) => Variables.TileBpm.Round(digits);
        [Tag("RecKps")]
        public static float RecKps(float digits = -1) => Variables.RecKPS.Round(digits);
        [Tag("BestProgress")]
        public static float BestProgress(float digits = -1) => Variables.BestProg.Round(digits);
        [Tag("LeastCheckPoint")]
        public static float LeastCheckPoint() => Variables.LeastChkPt;
        [Tag("StartProgress")]
        public static float StartProgress(float digits = -1) => Variables.StartProg.Round(digits);
        [Tag("CurMinute")]
        public static float CurMinute() => Variables.CurMinute;
        [Tag("CurSecond")]
        public static float CurSecond() => Variables.CurSecond;
        [Tag("CurMilliSecond")]
        public static float CurMilliSecond() => Variables.CurMilliSecond;
        [Tag("TotalMinute")]
        public static float TotalMinute() => Variables.TotalMinute;
        [Tag("TotalSecond")]
        public static float TotalSecond() => Variables.TotalSecond;
        [Tag("TotalMilliSecond")]
        public static float TotalMilliSecond() => Variables.TotalMilliSecond;
        [Tag("LeftTile")]
        public static float LeftTile() => Variables.LeftTile;
        [Tag("TotalTile")]
        public static float TotalTile() => Variables.TotalTile;
        [Tag("CurTile")]
        public static float CurTile() => Variables.CurrentTile;
        [Tag("Attempts")]
        public static float Attempts() => Variables.Attempts;
        [Tag("Year")]
        public static float Year() => DT.Now.Year;
        [Tag("Month")]
        public static float Month() => DT.Now.Month;
        [Tag("Day")]
        public static float Day() => DT.Now.Day;
        [Tag("Hour")]
        public static float Hour() => DT.Now.Hour;
        [Tag("Minute")]
        public static float Minute() => DT.Now.Minute;
        [Tag("Second")]
        public static float Second() => DT.Now.Second;
        [Tag("MilliSecond")]
        public static float MilliSecond() => DT.Now.Millisecond;
        [Tag("Multipress")]
        public static float Multipress() => Variables.MultipressCount;
        [Tag("Combo")]
        public static float Combo() => Variables.Combo;
        [Tag("Fps")]
        public static float Fps(float digits = -1) => Variables.Fps.Round(digits);
        [Tag("FrameTime")]
        public static float FrameTime(float digits = -1) => Variables.FrameTime.Round(digits);
    }
}
