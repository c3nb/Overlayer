using Overlayer.Core;
using Overlayer.Core.Tags;
using Overlayer.Core.Utils;
using Overlayer.Patches;

namespace Overlayer.Tags
{
    public static class Misc
    {
        [Tag("Radius", Category = Category.Play)]
        public static double Radius(int digits = -1) => scrController.instance.chosenplanet.cosmeticRadius.Round(digits);
        [Tag("Pitch", Category = Category.Play)]
        public static double Pitch(int digits = -1) => GCS.currentSpeedTrial.Round(digits);
        [Tag("EditorPitch", Category = Category.Play)]
        public static double EditorPitch(int digits = -1) => (scnEditor.instance.levelData.pitch / 100.0).Round(digits);
        [Tag("ShortcutPitch", Category = Category.Play)]
        public static double ShortcutPitch(int digits = -1) => StringConverter.ToDouble(ExtUtils.RemoveLast(scnEditor.instance.speedIndicator.percent.text, 1)).Round(digits);
        [Tag("Title", Category = Category.Song)]
        public static string Title() => scnEditor.instance?.levelData?.fullCaption.BreakRichTagWithoutSize();
        [Tag("Author", Category = Category.Song)]
        public static string Author() => scnEditor.instance?.levelData?.author.BreakRichTagWithoutSize();
        [Tag("Artist", Category = Category.Song)]
        public static string Artist() => scnEditor.instance?.levelData?.artist.BreakRichTagWithoutSize();
        [Tag("Accuracy", Category = Category.AccProg)]
        public static double Accuracy(int digits = -1) => (scrController.instance.mistakesManager.percentAcc * 100d).Round(digits);
        [Tag("Progress", Category = Category.AccProg)]
        public static double Progress(int digits = -1) => ((!scrLevelMaker.instance ? 0 : scrController.instance.percentComplete) * 100d).Round(digits);
        [Tag("CheckPoint", Category = Category.CheckPoint)]
        public static double CheckPoint() => scrController.checkpointsUsed;
        [Tag("TotalCheckPoint", Category = Category.CheckPoint)]
        public static double TotalCheckPoints() => BpmUpdater.AllCheckPoints.Count;
        [Tag("XAccuracy", Category = Category.AccProg)]
        public static double XAccuracy(int digits = -1) => (scrController.instance.mistakesManager.percentXAcc * 100d).Round(digits);
        [Tag("Year", Category = Category.Misc)]
        public static int Year() => FastDateTime.Now.Year;
        [Tag("Month", Category = Category.Misc)]
        public static int Month() => FastDateTime.Now.Month;
        [Tag("Day", Category = Category.Misc)]
        public static int Day() => FastDateTime.Now.Day;
        [Tag("Hour", Category = Category.Misc)]
        public static int Hour() => FastDateTime.Now.Hour;
        [Tag("Minute", Category = Category.Misc)]
        public static int Minute() => FastDateTime.Now.Minute;
        [Tag("Second", Category = Category.Misc)]
        public static int Second() => FastDateTime.Now.Second;
        [Tag("MilliSecond", Category = Category.Misc)]
        public static int MilliSecond() => FastDateTime.Now.Millisecond;
        [Tag("DifficultyStr", Category = Category.Play)]
        public static string DifficultyStr() => GCS.difficulty.ToString();
    }
}
