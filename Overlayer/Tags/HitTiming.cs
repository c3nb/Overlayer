using Overlayer.Tags.Attributes;

namespace Overlayer.Tags
{
    public static class HitTiming
    {
        [Tag(FieldFlags = FieldValueProcessing.RoundNumber)]
        public static double Timing;
        [Tag(FieldFlags = FieldValueProcessing.RoundNumber)]
        public static double TimingAvg;
        public static void Reset()
        {
            Timing = TimingAvg = 0;
        }
    }
}
