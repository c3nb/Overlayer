using ADOFAI;
using HarmonyEx;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(LevelData), "LoadLevel")]
    public static class DataInit
    {
        public static void Postfix(LevelData __instance)
        {
            string hash = MakeHash(__instance.author, __instance.artist, __instance.song);
            int attempts = Persistence.GetCustomWorldAttempts(hash);
            AttemptsCounter.Attempts[hash] = attempts;
            AttemptsCounter.AttemptsCount = attempts;
            PlaytimeCounter.MapID = hash;
        }
        public static string MakeHash(string author, string artist, string song)
            => MD5Hash.GetHash(author + artist + song);
    }
}
