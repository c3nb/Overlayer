using Overlayer.Tags.Attributes;

namespace Overlayer.Tags
{
    public static class HitTiming
    {
        [Tag(ProcessingFlags = ValueProcessing.RoundNumber)]
        public static double Timing;
        [Tag(ProcessingFlags = ValueProcessing.RoundNumber)]
        public static double TimingAvg;
        public static void Reset()
        {
            Timing = TimingAvg = 0;
        }
    }
}
