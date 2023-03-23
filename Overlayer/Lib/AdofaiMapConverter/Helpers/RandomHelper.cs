using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdofaiMapConverter.Helpers
{
    public static class RandomHelper
    {
        public static readonly Random random = new Random(DateTime.Now.Millisecond);
        public static double Range(double min, double range) => ((random.NextDouble() - 0.5) * range) + min + 0.5 * range;
        public static double MinMax(double min, double max) => Range(min, max - min);
    }
}
