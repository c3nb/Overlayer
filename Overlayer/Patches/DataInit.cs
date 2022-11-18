﻿using ADOFAI;
using HarmonyLib;
using Overlayer.Core;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(LevelData), "LoadLevel")]
    public static class DataInit
    {
        public static void Postfix(LevelData __instance)
        {
            if (!TagManager.AllTags["Attempts"].Referenced)
                return;
            string hash = MakeHash(__instance.author, __instance.artist, __instance.song);
            int attempts = Persistence.GetCustomWorldAttempts(hash);
            AttemptsCounter.Attempts[hash] = attempts;
            Variables.Attempts = attempts;
            AttemptsCounter.FailId = hash;
        }
        public static string MakeHash(string author, string artist, string song)
            => MD5Hash.GetHash(author + artist + song);
    }
}
