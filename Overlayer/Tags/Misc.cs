using Overlayer.Core;
using Overlayer.Core.Tags;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Misc
    {
        [Tag("Radius")]
        public static double Radius(int digits = -1) => scrController.instance.chosenplanet.cosmeticRadius.Round(digits);
        [Tag("Pitch")]
        public static double Pitch(int digits = -1) => GCS.currentSpeedTrial.Round(digits);
        [Tag("EditorPitch")]
        public static double EditorPitch(int digits = -1) => (scnEditor.instance.levelData.pitch / 100.0).Round(digits);
        [Tag("ShortcutPitch")]
        public static double ShortcutPitch(int digits = -1) => StringConverter.ToDouble(scnEditor.instance.speedIndicator.percent.text.RemoveLast(1)).Round(digits);
        [Tag("Title")]
        public static string Title() => scnEditor.instance?.levelData?.song.BreakRichTagWithoutSize();
        [Tag("Author")]
        public static string Author() => scnEditor.instance?.levelData?.author.BreakRichTagWithoutSize();
        [Tag("Artist")]
        public static string Artist() => scnEditor.instance?.levelData?.artist.BreakRichTagWithoutSize();
        [Tag("Accuracy")]
        public static double Accuracy(int digits = -1) => (scrController.instance.mistakesManager.percentAcc * 100d).Round(digits);
        [Tag("Progress")]
        public static double Progress(int digits = -1) => ((!scrLevelMaker.instance ? 0 : scrController.instance.percentComplete) * 100d).Round(digits);
        [Tag("CheckPoint")]
        public static double CheckPoint() => scrController.checkpointsUsed;
        [Tag("TotalCheckPoint")]
        public static double TotalCheckPoints() => BpmUpdater.AllCheckPoints.Count;
        [Tag("XAccuracy")]
        public static double XAccuracy(int digits = -1) => (scrController.instance.mistakesManager.percentXAcc * 100d).Round(digits);
        [Tag("Year")]
        public static int Year() => FastDateTime.Now.Year;
        [Tag("Month")]
        public static int Month() => FastDateTime.Now.Month;
        [Tag("Day")]
        public static int Day() => FastDateTime.Now.Day;
        [Tag("Hour")]
        public static int Hour() => FastDateTime.Now.Hour;
        [Tag("Minute")]
        public static int Minute() => FastDateTime.Now.Minute;
        [Tag("Second")]
        public static int Second() => FastDateTime.Now.Second;
        [Tag("MilliSecond")]
        public static int MilliSecond() => FastDateTime.Now.Millisecond;
        [Tag("DifficultyStr")]
        public static string DifficultyStr() => GCS.difficulty.ToString();
    }
}
