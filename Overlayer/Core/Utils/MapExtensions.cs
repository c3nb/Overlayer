using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Overlayer.Core.Utils
{
    public static class MapExtensions
    {
        public static int Map(this int value, int fromMin, int fromMax, int toMin, int toMax) => toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        public static double Map(this double value, double fromMin, double fromMax, double toMin, double toMax) => toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        public static float Map(this float value, float fromMin, float fromMax, float toMin, float toMax) => toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }
}
