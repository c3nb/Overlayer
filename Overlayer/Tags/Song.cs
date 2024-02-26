using Overlayer.Tags.Attributes;

namespace Overlayer.Tags
{
    public static class Song
    {
        [Tag]
        public static int CurMinute;
        [Tag]
        public static int CurSecond;
        [Tag]
        public static int CurMilliSecond;
        [Tag]
        public static int TotalMinute;
        [Tag]
        public static int TotalSecond;
        [Tag]
        public static int TotalMilliSecond;
        public static void Reset()
        {
            CurMinute = CurSecond = CurMilliSecond = 0;
            TotalMinute = TotalSecond = TotalMilliSecond = 0;
        }
    }
}
