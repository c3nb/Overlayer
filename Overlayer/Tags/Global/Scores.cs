using Overlayer.Core;

namespace Overlayer.Core.Global
{
    public static class Scores
    {
        [Tag("Score", "Score in Current Difficulty")]
        public static float Score() => Variables.CurrentScore;
        [Tag("LScore", "Score in Lenient Difficulty")]
        public static float LScore() => Variables.LenientScore;
        [Tag("NScore", "Score in Normal Difficulty")]
        public static float NScore() => Variables.NormalScore;
        [Tag("SScore", "Score in Strict Difficulty")]
        public static float SScore() => Variables.StrictScore;
    }
}
