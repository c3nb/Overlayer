using System;
using System.Collections.Generic;
using System.Linq;
using Overlayer.Utils;

namespace Overlayer.Tags.Global
{
    [ClassTag("ProgressDeath", "Death Count For Progress")]
    public static class ProgressDeath
    {
        [Tag]
        public static float GetDeaths(string opt)
        {
            if (!rangeDeaths.TryGetValue(opt, out int count))
                return rangeDeaths[opt] = 0;
            return count;
        }
        public static void Increment(float progress)
        {
            var keys = rangeDeaths.Keys.Where(key =>
            {
                var split = key.Split('~');
                if (split.Length <= 1) return false;
                var range = (min: FastParser.ParseFloat(split[0]), max: FastParser.ParseFloat(split[1]));
                return progress >= range.min && progress <= range.max;
            }).ToArray();
            for (int i = 0; i < keys.Length; i++)
                rangeDeaths[keys[i]]++;
        }
        public static void Reset()
            => rangeDeaths.Clear();
        public static readonly Dictionary<string, int> rangeDeaths = new Dictionary<string, int>();
    }
}
