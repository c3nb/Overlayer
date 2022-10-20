using Overlayer.Core;

namespace Overlayer.Tags.Global
{
    public static class Scores
    {
        [Tag("Score")]
        public static double Score() => Variables.CurrentScore;
        [Tag("LScore")]
        public static double LScore() => Variables.LenientScore;
        [Tag("NScore")]
        public static double NScore() => Variables.NormalScore;
        [Tag("SScore")]
        public static double SScore() => Variables.StrictScore;
    }
}
