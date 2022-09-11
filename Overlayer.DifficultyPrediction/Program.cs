using System;
using System.IO;
using Overlayer.Utils;

namespace Overlayer.DifficultyPrediction
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(DifficultyPredictor.Predict(new DifficultyPredictor.ModelInput
                {
                    타일_수 = args[0].ToInt(),
                    평균_BPM = args[1].ToFloat(),
                    BPM_분산 = args[2].ToFloat(),
                    BPM_표준편차 = args[3].ToFloat()
                }).Score);
            }
            catch (Exception e)
            {
                File.WriteAllText("Error.txt", $"Error On Predicting Difficulty. {e}");
            }
        }
    }
}
