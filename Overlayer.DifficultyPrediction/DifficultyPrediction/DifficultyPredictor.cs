using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.DifficultyPrediction
{
    public partial class DifficultyPredictor
    {
        public static double Predict(int tiles, float bpmAvg, float bpmVariance, float bpmStdDev)
        {
            var sampleData = new ModelInput()
            {
                타일_수 = tiles,
                평균_BPM = bpmAvg,
                BPM_분산 = bpmVariance,
                BPM_표준편차 = bpmStdDev,
            };
            return Predict(sampleData).Score;
        }
    }
}
