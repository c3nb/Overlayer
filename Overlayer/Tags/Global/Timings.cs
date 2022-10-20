using System;
using Overlayer.Core;
using System.Collections.Generic;
using System.Linq;

namespace Overlayer.Tags.Global
{
    public static class Timings
    {
        [Tag("Timing")]
        public static double Timing(double digits = -1) => Variables.Timing.Round(digits);
        [Tag("TimingAvg")]
        public static double TimingAvg(double digits = -1)
        {
            if (TimingList.Any())
            {
                var avg = (double)TimingList.Average();
                if (digits != -1)
                    return (double)Math.Round(avg, (int)digits);
                return avg;
            }
            return 0;
        }
        public static List<double> TimingList = new List<double>();
    }
}
