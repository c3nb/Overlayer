using System;
using HarmonyLib;
using UnityEngine;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "PlayerControl_Update")]
    public static class Updater
    {
        public static void Prefix(scrController __instance)
        {
            if (__instance.paused || !__instance.conductor.isGameWorld) return;
            AudioSource song = __instance.conductor.song;
            if (!song.clip) return;
            TimeSpan nowt = TimeSpan.FromSeconds(song.time);
            TimeSpan tott = TimeSpan.FromSeconds(song.clip.length);
            Variables.CurMinute = nowt.Minutes;
            Variables.CurSecond = nowt.Seconds;
            Variables.TotalMinute = tott.Minutes;
            Variables.TotalSecond = tott.Seconds;
            Variables.CurrentTile = __instance.currentSeqID;
            Variables.TotalTile = __instance.lm.listFloors.Count - 1;
            Variables.LeftTile = Variables.TotalTile - Variables.CurrentTile;
        }
    }
}
