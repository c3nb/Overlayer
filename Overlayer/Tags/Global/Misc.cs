using Overlayer.Core;
using Overlayer.Core.Utils;

namespace Overlayer.Tags.Global
{
    public static class Misc
    {
        [Tag("Radius")]
        public static double Radius(double digits = -1) => scrController.instance.chosenplanet.cosmeticRadius.Round(digits);
        [Tag("Pitch")]
        public static double Pitch(double digits = -1) => GCS.currentSpeedTrial.Round(digits);
        [Tag("EditorPitch")]
        public static double EditorPitch(double digits = -1) => (scnEditor.instance.levelData.pitch / 100.0).Round(digits);
        [Tag("ShortcutPitch")]
        public static double ShortcutPitch(double digits = -1) => scnEditor.instance.speedIndicator.percent.text.RemoveLast(1).ToDouble().Round(digits);
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
        public static double StartTile() => Variables.StartTile;
        [Tag("IntegratedDifficulty")]
        public static double IntegratedDifficulty(double digits = -1) => PlayPoint.IntegratedDifficulty.Round(digits);
        [Tag("PredictedDifficulty")]
        public static double PredictedDifficulty(double digits = -1) => PlayPoint.PredictedDifficulty.Round(digits);
        [Tag("ForumDifficulty")]
        public static double ForumDifficulty(double digits = -1) => PlayPoint.ForumDifficulty.Round(digits);
        [Tag("Accuracy")]
        public static double Accuracy(double digits = -1) => (scrController.instance.mistakesManager.percentAcc * 100).Round(digits);
        [Tag("Progress")]
        public static double Progress(double digits = -1) => ((!scrLevelMaker.instance ? 0 : scrController.instance.percentComplete) * 100f).Round(digits);
        [Tag("CheckPoint")]
        public static double CheckPoint() => scrController.checkpointsUsed;
        [Tag("CurCheckPoint")]
        public static double CurCheckPoint() => Variables.CurrentCheckPoint;
        [Tag("TotalCheckPoint")]
        public static double TotalCheckPoints() => Variables.AllCheckPoints.Count;
        [Tag("XAccuracy")]
        public static double XAccuracy(double digits = -1) => (scrController.instance.mistakesManager.percentXAcc * 100).Round(digits);
        [Tag("FailCount")]
        public static double FailCount() => Variables.FailCount;
        [Tag("MissCount")]
        public static double MissCount() => scrMistakesManager.hitMarginsCount[8];
        [Tag("Overloads")]
        public static double Overloads() => scrMistakesManager.hitMarginsCount[9];
        [Tag("CurBpm")]
        public static double CurBpm(double digits = -1) => Variables.CurBpm.Round(digits);
        [Tag("TileBpm")]
        public static double TileBpm(double digits = -1) => Variables.TileBpm.Round(digits);
        [Tag("RecKps")]
        public static double RecKps(double digits = -1) => Variables.RecKPS.Round(digits);
        [Tag("BestProgress")]
        public static double BestProgress(double digits = -1) => Variables.BestProg.Round(digits);
        [Tag("LeastCheckPoint")]
        public static double LeastCheckPoint() => Variables.LeastChkPt;
        [Tag("StartProgress")]
        public static double StartProgress(double digits = -1) => Variables.StartProg.Round(digits);
        [Tag("CurMinute")]
        public static double CurMinute() => Variables.CurMinute;
        [Tag("CurSecond")]
        public static double CurSecond() => Variables.CurSecond;
        [Tag("CurMilliSecond")]
        public static double CurMilliSecond() => Variables.CurMilliSecond;
        [Tag("TotalMinute")]
        public static double TotalMinute() => Variables.TotalMinute;
        [Tag("TotalSecond")]
        public static double TotalSecond() => Variables.TotalSecond;
        [Tag("TotalMilliSecond")]
        public static double TotalMilliSecond() => Variables.TotalMilliSecond;
        [Tag("LeftTile")]
        public static double LeftTile() => Variables.LeftTile;
        [Tag("TotalTile")]
        public static double TotalTile() => Variables.TotalTile;
        [Tag("CurTile")]
        public static double CurTile() => Variables.CurrentTile;
        [Tag("Attempts")]
        public static double Attempts() => Variables.Attempts;
        [Tag("Year")]
        public static double Year() => DT.Now.Year;
        [Tag("Month")]
        public static double Month() => DT.Now.Month;
        [Tag("Day")]
        public static double Day() => DT.Now.Day;
        [Tag("Hour")]
        public static double Hour() => DT.Now.Hour;
        [Tag("Minute")]
        public static double Minute() => DT.Now.Minute;
        [Tag("Second")]
        public static double Second() => DT.Now.Second;
        [Tag("MilliSecond")]
        public static double MilliSecond() => DT.Now.Millisecond;
        [Tag("Multipress")]
        public static double Multipress() => Variables.MultipressCount;
        [Tag("Combo")]
        public static double Combo() => Variables.Combo;
        [Tag("Fps")]
        public static double Fps(double digits = -1) => Variables.Fps.Round(digits);
        [Tag("FrameTime")]
        public static double FrameTime(double digits = -1) => Variables.FrameTime.Round(digits);
    }
}
