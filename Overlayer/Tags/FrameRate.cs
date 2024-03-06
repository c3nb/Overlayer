using Overlayer.Tags.Attributes;

namespace Overlayer.Tags
{
    public static class FrameRate
    {
        [Tag(NotPlaying = true, ProcessingFlags = ValueProcessing.RoundNumber)]
        public static double Fps;
        [Tag(NotPlaying = true, ProcessingFlags = ValueProcessing.RoundNumber)]
        public static double FrameTime;
        public static void Reset()
        {
            Fps = FrameTime = 0;
        }
    }
}
