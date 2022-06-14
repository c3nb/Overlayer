using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			if ((__instance.controller.isLevelEditor || __instance.controller.currentState == scrController.States.PlayerControl) && (__instance.controller.goShown || __instance.conductor.fastTakeoff))
			{
				double num = AudioSettings.dspTime - (double)scrConductor.calibration_i;
				if (__instance.controller.curCountdown < __instance.conductor.extraTicksCountdown.Count && num > __instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].time)
				{
					if (num < __instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].time + 1.0)
					{
						if (__instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].count > 0)
						{
							___text.text = __instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].count.ToString();
							SetStartProg(__instance.controller);
						}
						else
						{
							___text.text = RDString.Get("status.go", null);
							___timeGoTween = (float)(__instance.conductor.crotchet / __instance.conductor.extraTicksCountdown[__instance.controller.curCountdown].speed) / __instance.conductor.song.pitch;
							SetStartProg(__instance.controller);
						}
					}
					__instance.controller.curCountdown++;
				}
			}
			scnEditor editor = __instance.editor;
			if (editor != null && editor.inStrictlyEditingMode) return false;
			if (!__instance.controller.goShown && !__instance.conductor.fastTakeoff && (scrController.CheckStateIs("PlayerControl") || scrController.CheckStateIs("Countdown") || scrController.CheckStateIs("Checkpoint")) && GCS.usingCheckpoints)
			{
				double num2 = AudioSettings.dspTime - (double)scrConductor.calibration_i;
				int countdownTicks = __instance.conductor.countdownTicks;
				if (num2 > __instance.conductor.GetCountdownTime(countdownTicks - 1) && !__instance.controller.goShown)
				{
					___text.text = RDString.Get("status.go", null);
					__instance.controller.goShown = true;
					___timeGoTween = (float)__instance.conductor.crotchet;
					SetStartProg(__instance.controller);
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
					SetStartProg(__instance.controller);
				}
				else
				{
					string text = GCS.speedTrialMode ? "levelSelect.SpeedTrial" : "status.getReady";
					text = (GCS.practiceMode ? "status.practiceMode" : text);
					___text.text = RDString.Get(text, null);
					SetStartProg(__instance.controller);
				}
			}
			if (___timeGoTween > 0f)
			{
				___timeGoTween -= Time.unscaledDeltaTime;
				if (___timeGoTween <= 0f) __instance.CancelGo();
			}
			return false;
        }
		public static void SetStartProg(scrController ctrl)
			=> Variables.StartProg = ctrl.percentComplete * 100;
	}
}
