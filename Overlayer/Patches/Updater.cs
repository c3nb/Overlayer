using HarmonyEx;
using System;
using UnityEngine;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "PlayerControl_Update")]
    public static class Updater
    {
        public static void Prefix(scrController __instance)
        {
            if (__instance.paused || !scrConductor.instance.isGameWorld) return;
            Variables.CurrentTile = __instance.currentSeqID + 1;
            Variables.TotalTile = scrLevelMaker.instance.listFloors.Count;
            Variables.LeftTile = Variables.TotalTile - Variables.CurrentTile;
            AudioSource song = scrConductor.instance.song;
            if (!song.clip) return;
            TimeSpan nowt = TimeSpan.FromSeconds(song.time);
            TimeSpan tott = TimeSpan.FromSeconds(song.clip.length);
            Variables.CurMinute = nowt.Minutes;
            Variables.CurSecond = nowt.Seconds;
            Variables.CurMilliSecond = nowt.Milliseconds;
            Variables.TotalMinute = tott.Minutes;
            Variables.TotalSecond = tott.Seconds;
            Variables.TotalMilliSecond = tott.Milliseconds;
        }
    }
}
