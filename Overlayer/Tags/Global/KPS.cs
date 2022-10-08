using System;
using Overlayer.Core;
using System.Collections.Generic;
using System.Diagnostics;

namespace Overlayer.Tags.Global
{
    [ClassTag("CurKps", "Current KPS", Threads = new[] { "Calculate" })]
    public static class KPS
    {
        [Tag]
        public static float GetKps() => Kps;
        public static int Kps;
        public static double KpsAvg;
        public static int KpsMax;
        public static int Total;
        public static void Calculate()
        {
            LinkedList<int> timePoints = new LinkedList<int>();
            long n = 0;
            Stopwatch watch = Stopwatch.StartNew();
            while (true)
            {
                if (watch.ElapsedMilliseconds >= Settings.Instance.KPSUpdateRate)
                {
                    int temp = Variables.KpsTemp;
                    Variables.KpsTemp = 0;
                    int num = temp;
                    foreach (int i in timePoints)
                        num += i;
                    KpsMax = Math.Max(num, KpsMax);
                    if (num != 0)
                    {
                        KpsAvg = (KpsAvg * n + num) / (n + 1.0);
                        n += 1L;
                        Total += temp;
                    }
                    timePoints.AddFirst(temp);
                    if (timePoints.Count >= 1000 / Settings.Instance.KPSUpdateRate)
                        timePoints.RemoveLast();
                    Kps = num;
                    watch.Restart();
                }
            }
        }
    }
}
