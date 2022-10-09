using Overlayer.Core;

namespace Overlayer.Tags.Global
{
    public static class Scores
    {
        [Tag("Score")]
        public static float Score() => Variables.CurrentScore;
        [Tag("LScore")]
        public static float LScore() => Variables.LenientScore;
        [Tag("NScore")]
        public static float NScore() => Variables.NormalScore;
        [Tag("SScore")]
        public static float SScore() => Variables.StrictScore;
    }
}
