using Overlayer.Tags.Attributes;

namespace Overlayer.Tags
{
    public static class Bpm
    {
        [Tag(ProcessingFlags = ValueProcessing.RoundNumber)]
        public static double TileBpm;
        [Tag(ProcessingFlags = ValueProcessing.RoundNumber)]
        public static double CurBpm;
        [Tag(ProcessingFlags = ValueProcessing.RoundNumber)]
        public static double RecKPS;
        [Tag(ProcessingFlags = ValueProcessing.RoundNumber)]
        public static double TileBpmWithoutPitch;
        [Tag(ProcessingFlags = ValueProcessing.RoundNumber)]
        public static double CurBpmWithoutPitch;
        [Tag(ProcessingFlags = ValueProcessing.RoundNumber)]
        public static double RecKPSWithoutPitch;
        public static void Reset()
        {
            TileBpm = CurBpm = RecKPS = 0;
            TileBpmWithoutPitch = CurBpmWithoutPitch = RecKPSWithoutPitch = 0;
        }
    }
}
