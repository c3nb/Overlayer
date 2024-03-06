using ADOFAI;
using Overlayer.Tags.Attributes;
using Overlayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overlayer.Tags
{
    public static class Status
    {
        public static LevelData LevelData => scnGame.instance?.levelData ?? scnEditor.instance?.levelData;
        [Tag]
        public static double Progress(int digits = -1) => (scrController.instance?.percentComplete * 100 ?? 0).Round(digits);
        [Tag]
        public static double Accuracy(int digits = -1) => (scrController.instance?.mistakesManager?.percentAcc * 100 ?? 0).Round(digits);
        [Tag]
        public static double XAccuracy(int digits = -1) => (scrController.instance?.mistakesManager?.percentXAcc * 100 ?? 0).Round(digits);
        [Tag]
        public static double Pitch(int digits = -1) => GCS.currentSpeedTrial.Round(digits);
        [Tag]
        public static double EditorPitch(int digits = -1) => (LevelData.pitch / 100.0).Round(digits);
        [Tag]
        public static int CheckPointUsed() => scrController.checkpointsUsed;
        [Tag]
        public static int CurCheckPoint;
        [Tag]
        public static int TotalCheckPoints;
        [Tag]
        public static int Combo;
        [Tag]
        public static int MaxCombo;
        [Tag]
        public static int LScore;
        [Tag]
        public static int NScore;
        [Tag]
        public static int SScore;
        [Tag]
        public static int Score;
        [Tag]
        public static double BestProgress;
        #region MarginCombo
        [Tag]
        public static int LMarginCombo(HitMargin margin) => Combos[(int)Difficulty.Lenient][(int)margin];
        [Tag]
        public static int NMarginCombo(HitMargin margin) => Combos[(int)Difficulty.Normal][(int)margin];
        [Tag]
        public static int SMarginCombo(HitMargin margin) => Combos[(int)Difficulty.Strict][(int)margin];
        [Tag]
        public static int MarginCombo(HitMargin margin) => Combos[(int)GCS.difficulty][(int)margin];
        #endregion
        #region MarginMaxCombo
        [Tag]
        public static int LMarginMaxCombo(HitMargin margin) => MaxCombos[(int)Difficulty.Lenient][(int)margin];
        [Tag]
        public static int NMarginMaxCombo(HitMargin margin) => MaxCombos[(int)Difficulty.Normal][(int)margin];
        [Tag]
        public static int SMarginMaxCombo(HitMargin margin) => MaxCombos[(int)Difficulty.Strict][(int)margin];
        [Tag]
        public static int MarginMaxCombo(HitMargin margin) => MaxCombos[(int)GCS.difficulty][(int)margin];
        #endregion
        public static int[][] Combos = new int[EnumHelper<Difficulty>.GetValues().Length][];
        public static int[][] MaxCombos = new int[EnumHelper<Difficulty>.GetValues().Length][];
        public static void Reset()
        {
            CurCheckPoint = TotalCheckPoints = Combo = MaxCombo = LScore = NScore = SScore = Score = 0;
            //BestProgress = 0;
            int margins = EnumHelper<HitMargin>.GetValues().Length;
            for (int i = 0; i < Combos.Length; i++)
                Combos[i] = new int[margins];
            for (int i = 0; i < MaxCombos.Length; i++)
                MaxCombos[i] = new int[margins];
        }
    }
}
