using Overlayer.Core.Patches;

namespace Overlayer.Tags.Patches
{
    public class BpmPatch : PatchBase<BpmPatch>
    {
        public static float bpm, pitch, bpmwithoutpitch, playbackSpeed = 1;
        [LazyPatch("Tags.Bpm.Initializer_scnGame", "scnGame", "Play", Triggers = new string[]
        {
            nameof(Bpm.TileBpm), nameof(Bpm.CurBpm), nameof(Bpm.RecKPS),
            nameof(Bpm.TileBpmWithoutPitch), nameof(Bpm.CurBpmWithoutPitch), nameof(Bpm.RecKPSWithoutPitch),
        })]
        [LazyPatch("Tags.Bpm.Initializer_scrPressToStart", "scrPressToStart", "ShowText", Triggers = new string[]
        {
            nameof(Bpm.TileBpm), nameof(Bpm.CurBpm), nameof(Bpm.RecKPS),
            nameof(Bpm.TileBpmWithoutPitch), nameof(Bpm.CurBpmWithoutPitch), nameof(Bpm.RecKPSWithoutPitch),
        })]
        public static class Initializer
        {
            public static void Postfix(scrController __instance)
            {
                if (!(scrController.instance?.gameworld ?? false)) return;
                if (scnGame.instance == null && scnEditor.instance == null) return;
                try
                {
                    if (scnGame.instance != null)
                    {
                        pitch = (float)scnGame.instance.levelData.pitch / 100;
                        if (ADOBase.isCLSLevel) pitch *= GCS.currentSpeedTrial;
                        bpm = scnGame.instance.levelData.bpm * playbackSpeed * pitch;
                        bpmwithoutpitch = scnGame.instance.levelData.bpm * playbackSpeed;
                    }
                    else
                    {
                        pitch = scrConductor.instance.song.pitch;
                        bpm = scrConductor.instance.bpm * pitch;
                        bpmwithoutpitch = scrConductor.instance.bpm;
                    }
                    playbackSpeed = scnEditor.instance?.playbackSpeed ?? 1;
                }
                catch
                {
                    pitch = scrConductor.instance.song.pitch;
                    playbackSpeed = 1;
                    bpm = scrConductor.instance.bpm * pitch;
                    bpmwithoutpitch = scrConductor.instance.bpm;
                }
                float cur = bpm;
                if (__instance.currentSeqID != 0)
                {
                    double speed = scrController.instance.speed;
                    cur = (float)(bpm * speed);
                }
                Bpm.TileBpm = cur;
                Bpm.CurBpm = cur;
                Bpm.RecKPS = cur / 60;
            }
        }
        [LazyPatch("Tags.Bpm.BpmGetPatch", "scrPlanet", "MoveToNextFloor", Triggers = new string[]
        {
            nameof(Bpm.TileBpm), nameof(Bpm.CurBpm), nameof(Bpm.RecKPS),
            nameof(Bpm.TileBpmWithoutPitch), nameof(Bpm.CurBpmWithoutPitch), nameof(Bpm.RecKPSWithoutPitch),
        })]
        public static class BpmGetPatch
        {
            public static void Postfix(scrFloor floor)
            {
                if (floor.nextfloor == null) return;
                double curBPM = GetRealBpm(floor, bpm) * playbackSpeed;
                Bpm.TileBpm = bpm * scrController.instance.speed;
                Bpm.CurBpm = curBPM;
                Bpm.RecKPS = curBPM / 60;
                curBPM = GetRealBpm(floor, bpmwithoutpitch) * playbackSpeed;
                Bpm.TileBpmWithoutPitch = bpmwithoutpitch * scrController.instance.speed;
                Bpm.CurBpmWithoutPitch = curBPM;
                Bpm.RecKPSWithoutPitch = curBPM / 60;
            }
            public static double GetRealBpm(scrFloor floor, float bpm)
            {
                if (floor == null) return bpm;
                if (floor.nextfloor == null)
                    return scrController.instance.speed * bpm;
                return 60.0 / (floor.nextfloor.entryTime - floor.entryTime);
            }
        }
    }
}
