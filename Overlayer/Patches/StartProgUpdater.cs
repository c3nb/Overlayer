using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrCountdown), "Update")]
    public static class StartProgUpdater
    {
        public static bool Prefix(scrCountdown __instance, ref Text ___text, ref float ___timeGoTween)
        {
			if ((scnEditor.instance != null || __instance.controller.currentState == States.PlayerControl) && (__instance.controller.goShown || __instance.conductor.fastTakeoff))
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
							SetStartProg(__instance.controller);
							___timeGoTween = (float)(__instance.conductor.crotchet / __instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].speed) / __instance.conductor.song.pitch;
						}
					}
					__instance.controller.curCountdown++;
				}
			}
			scnEditor editor = scnEditor.instance;
			if (editor != null && editor.inStrictlyEditingMode) return false;
			if (!__instance.controller.goShown && !__instance.conductor.fastTakeoff && (__instance.controller.state == States.PlayerControl || __instance.controller.state == States.Countdown || __instance.controller.state == States.Checkpoint))
			{
				double num2 = AudioSettings.dspTime - (double)scrConductor.calibration_i;
				int countdownTicks = __instance.conductor.countdownTicks;
				if (num2 > __instance.conductor.GetCountdownTime(countdownTicks - 1) && !__instance.controller.goShown)
				{
					___text.text = RDString.Get("status.go", null);
					SetStartProg(__instance.controller);
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
					string text = GCS.speedTrialMode ? "levelSelect.SpeedTrial" : "status.getReady";
					text = (GCS.practiceMode ? "status.practiceMode" : text);
					___text.text = RDString.Get(text, null);
					SetStartProg(__instance.controller);
				}
			}
			if (!__instance.controller.goShown && __instance.conductor.fastTakeoff && (__instance.controller.state == States.PlayerControl || __instance.controller.state == States.Countdown || __instance.controller.state == States.Checkpoint))
			{
				___text.text = RDString.Get("status.go", null);
				SetStartProg(__instance.controller);
				__instance.controller.goShown = true;
				___timeGoTween = (float)__instance.conductor.crotchet;
			}
			if (___timeGoTween > 0f)
			{
				___timeGoTween -= Time.unscaledDeltaTime;
				if (___timeGoTween <= 0f)
				{
					__instance.CancelGo();
				}
			}
			return false;
        }
        public static void SetStartProg(scrController ctrl)
        {
            if (Variables.IsStarted)
            {
                Variables.StartProg = ctrl.percentComplete * 100;
                Variables.StartTile = ctrl.currentSeqID;
                Variables.IsStarted = false;
            }
        }
    }
}
