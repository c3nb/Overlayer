using Overlayer.Core.Patches;
using UnityEngine;
using UnityEngine.UI;

namespace Overlayer.Tags.Patches
{
    public class TilePatch : PatchBase<TilePatch>
    {
        [LazyPatch("Tags.Tile.TileCountPatch_Update", "scnGame", "Update", Triggers = new string[]
        {
            nameof(Tile.LeftTile), nameof(Tile.CurTile), nameof(Tile.TotalTile)
        })]
        public static class TileCountPatch
        {
            public static void Postfix()
            {
                Tile.CurTile = scrController.instance.currentSeqID + 1;
                Tile.TotalTile = ADOBase.lm.listFloors.Count;
                Tile.LeftTile = Tile.TotalTile - Tile.CurTile;
            }
        }
        [LazyPatch("Tags.Tile.StartTile&ProgressPatch", "scrCountdown", "Update", Triggers = new string[]
        {
            nameof(Tile.StartTile), nameof(Tile.StartProgress)
        })]
        public static class StartTileAndProgressPatch
        {
            public static bool Prefix(scrCountdown __instance, ref Text ___text, ref float ___timeGoTween)
            {
                if ((ADOBase.isLevelEditor || __instance.controller.currentState == States.PlayerControl) && (__instance.controller.goShown || __instance.conductor.fastTakeoff))
                {
                    double num = AudioSettings.dspTime - (double)scrConductor.calibration_i;
                    if (__instance.controller.curCountdown < __instance.conductor.extraTicksCountdown.Count && num > __instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].time)
                    {
                        if (num < __instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].time + 1.0)
                        {
                            if (__instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].count > 0)
                            {
                                ___text.text = __instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].count.ToString();
                            }
                            else
                            {
                                ___text.text = RDString.Get("status.go", null);
                                SetStartTileProg(__instance.controller);
                                ___timeGoTween = (float)(__instance.conductor.crotchet / __instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].speed) / __instance.conductor.song.pitch;
                            }
                        }
                        __instance.controller.curCountdown++;
                    }
                }
                scnEditor editor = ADOBase.editor;
                if (editor != null && editor.inStrictlyEditingMode) return false;
                if (!__instance.controller.goShown && !__instance.conductor.fastTakeoff && (__instance.controller.state == States.PlayerControl || __instance.controller.state == States.Countdown || __instance.controller.state == States.Checkpoint))
                {
                    double num2 = AudioSettings.dspTime - (double)scrConductor.calibration_i;
                    int countdownTicks = __instance.conductor.countdownTicks;
                    if (num2 > __instance.conductor.GetCountdownTime(countdownTicks - 1) && !__instance.controller.goShown)
                    {
                        ___text.text = RDString.Get("status.go", null);
                        SetStartTileProg(__instance.controller);
                        __instance.controller.goShown = true;
                        ___timeGoTween = (float)__instance.conductor.crotchet;
                    }
                    else if (num2 > __instance.conductor.GetCountdownTime(0))
                    {
                        int num3 = countdownTicks - 1;
                        for (int i = 1; i < countdownTicks - 1; i++)
                        {
                            if (num2 > __instance.conductor.GetCountdownTime(i))
                            {
                                num3 = countdownTicks - 1 - i;
                            }
                        }
                        ___text.text = num3.ToString();
                    }
                    else
                    {
                        string text = (GCS.speedTrialMode ? "levelSelect.SpeedTrial" : "status.getReady");
                        text = (GCS.practiceMode ? "status.practiceMode" : text);
                        ___text.text = RDString.Get(text, null);
                        SetStartTileProg(__instance.controller);
                    }
                }
                if (!__instance.controller.goShown && __instance.conductor.fastTakeoff && (__instance.controller.state == States.PlayerControl || __instance.controller.state == States.Countdown || __instance.controller.state == States.Checkpoint))
                {
                    ___text.text = RDString.Get("status.go", null);
                    SetStartTileProg(__instance.controller);
                    __instance.controller.goShown = true;
                    ___timeGoTween = (float)__instance.conductor.crotchet;
                }
                if (___timeGoTween > 0f)
                {
                    ___timeGoTween -= UnityEngine.Time.unscaledDeltaTime;
                    if (___timeGoTween <= 0f)
                    {
                        __instance.CancelGo();
                    }
                }
                return false;
            }
            public static void SetStartTileProg(scrController ctrl)
            {
                if (Tile.IsStarted)
                {
                    Tile.StartProgress = ctrl.percentComplete * 100;
                    Tile.StartTile = ctrl.currentSeqID + 1;
                    Tile.IsStarted = false;
                }
            }
        }
        [LazyPatch("Tags.Tile.StartedResetter_scnEditor", "scnEditor", "ResetScene", Triggers = new string[]
        {
            nameof(Tile.StartTile), nameof(Tile.StartProgress)
        })]
        [LazyPatch("Tags.Tile.StartedResetter_scnGame", "scnGame", "ResetScene", Triggers = new string[]
        {
            nameof(Tile.StartTile), nameof(Tile.StartProgress)
        })]
        public static class StartedResetter
        {
            public static void Postfix()
            {
                Tile.IsStarted = true;
            }
        }
    }
}
