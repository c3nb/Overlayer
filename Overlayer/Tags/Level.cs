using ADOFAI;
using Overlayer.Tags.Attributes;
using Overlayer.Utils;

namespace Overlayer.Tags
{
    public static class Level
    {
        public static LevelData LevelData => scnGame.instance?.levelData ?? scnEditor.instance?.levelData;
        [Tag]
        public static string Title(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => ADOBase.isOfficialLevel ? ADOBase.sceneName.Trim(maxLength, afterTrimStr) : TitleRaw(maxLength, afterTrimStr)?.BreakRichTag();
        [Tag]
        public static string Author(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => AuthorRaw(maxLength, afterTrimStr)?.BreakRichTag();
        [Tag]
        public static string Artist(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => ArtistRaw(maxLength, afterTrimStr)?.BreakRichTag();
        [Tag]
        public static string TitleRaw(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => LevelData?.song?.Trim(maxLength, afterTrimStr);
        [Tag]
        public static string AuthorRaw(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => LevelData?.author?.Trim(maxLength, afterTrimStr);
        [Tag]
        public static string ArtistRaw(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => LevelData?.artist?.Trim(maxLength, afterTrimStr);
        public static void Reset() { }
    }
}
