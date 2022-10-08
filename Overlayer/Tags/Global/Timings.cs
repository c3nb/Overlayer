using System;
using Overlayer.Core;
using System.Collections.Generic;
using System.Linq;

namespace Overlayer.Tags.Global
{
    public static class Timings
    {
        [Tag("Timing", "Hit Timing")]
        public static float Timing(float digits = -1) => Variables.Timing.Round(digits);
        [Tag("TimingAvg", "Average Hit Timing")]
        public static float TimingAvg(float digits = -1)
        {
            if (TimingList.Any())
            {
                var avg = (float)TimingList.Average();
                if (digits != -1)
                    return (float)Math.Round(avg, (int)digits);
                return avg;
            }
            return 0;
        }
        public static List<double> TimingList = new List<double>();
    }
}
