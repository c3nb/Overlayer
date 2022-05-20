using HarmonyLib;
using ADOFAI;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(LevelData), "LoadLevel")]
    public static class DataInit
    {
        public static void Postfix(LevelData __instance)
        {
            string hash = MD5Hash.GetHash(__instance.author + __instance.artist + __instance.song);
            int attempts = Persistence.GetCustomWorldAttempts(hash);
            FailCounter.Attempts[hash] = attempts;
            Variables.Attempts = attempts;
            FailCounter.FailId = hash;
        }
    }
}
