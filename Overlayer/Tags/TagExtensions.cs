using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Tags
{
    public static class TagExtensions
    {
        public static float Round(this float value, float digits)
        {
            if (digits == -1)
                return value;
            return (float)Math.Round(value, (int)digits);
        }
        public static float Round(this double value, float digits)
        {
            if (digits == -1)
                return (float)value;
            return (float)Math.Round(value, (int)digits);
        }
    }
}
