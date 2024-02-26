using Overlayer.Tags.Attributes;

namespace Overlayer.Tags
{
    public static class Bpm
    {
        [Tag(FieldFlags = FieldValueProcessing.RoundNumber)]
        public static double TileBpm;
        [Tag(FieldFlags = FieldValueProcessing.RoundNumber)]
        public static double CurBpm;
        [Tag(FieldFlags = FieldValueProcessing.RoundNumber)]
        public static double RecKPS;
        [Tag(FieldFlags = FieldValueProcessing.RoundNumber)]
        public static double TileBpmWithoutPitch;
        [Tag(FieldFlags = FieldValueProcessing.RoundNumber)]
        public static double CurBpmWithoutPitch;
        [Tag(FieldFlags = FieldValueProcessing.RoundNumber)]
        public static double RecKPSWithoutPitch;
        public static void Reset()
        {
            TileBpm = CurBpm = RecKPS = 0;
            TileBpmWithoutPitch = CurBpmWithoutPitch = RecKPSWithoutPitch = 0;
        }
    }
}
