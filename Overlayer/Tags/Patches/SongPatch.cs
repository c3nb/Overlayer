using Overlayer.Core.Patches;
using System;
using UnityEngine;

namespace Overlayer.Tags.Patches
{
    public class SongPatch : PatchBase<SongPatch>
    {
        [LazyPatch("Tags.Song.SongTimePatch", "scrConductor", "Update", Triggers = new string[]
        {
            nameof(Song.CurMinute), nameof(Song.CurSecond),nameof(Song.CurMilliSecond),
            nameof(Song.TotalMinute), nameof(Song.TotalSecond),nameof(Song.TotalMilliSecond),
        })]
        public static class SongTimePatch
        {
            public static void Postfix(scrConductor __instance)
            {
                if (scrController.instance.paused || !__instance.isGameWorld) return;
                AudioSource song = __instance.song;
                if (!song.clip) return;
                TimeSpan nowt = TimeSpan.FromSeconds(song.time);
                TimeSpan tott = TimeSpan.FromSeconds(song.clip.length);
                Song.CurMinute = nowt.Minutes;
                Song.CurSecond = nowt.Seconds;
                Song.CurMilliSecond = nowt.Milliseconds;
                Song.TotalMinute = tott.Minutes;
                Song.TotalSecond = tott.Seconds;
                Song.TotalMilliSecond = tott.Milliseconds;
            }
        }
    }
}
