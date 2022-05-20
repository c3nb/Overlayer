using HarmonyLib;
using UnityEngine;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrController), "Hit")]
    public static class ErrorMeterUnlocker
    {
        public static bool Prefix(scrController __instance)
        {
            if (!Settings.Instance.UnlockErrorMeterAtAuto) return true;
            if (!__instance.responsive) return false;
            if (__instance.isLevelEditor && __instance.controller.paused) return false;
            if (__instance.errorMeter && Persistence.GetHitErrorMeterSize() != ErrorMeterSize.Off)
            {
                double angle = __instance.chosenplanet.angle - __instance.chosenplanet.targetExitAngle;
                if (!__instance.isCW) angle *= -1f;
                if (!__instance.midspinInfiniteMargin)
                    __instance.errorMeter.AddHit((float)angle);
            }
            scrMisc.Vibrate(50L);
            __instance.chosenplanet.other.ChangeFace(true);
            __instance.chosenplanet = __instance.chosenplanet.SwitchChosen();
            if (ADOBase.playerIsOnIntroScene) return false;
            if (__instance.camy.followMode)
            {
                __instance.camy.frompos = __instance.camy.transform.localPosition;
                __instance.camy.topos = new Vector3(__instance.chosenplanet.transform.position.x, __instance.chosenplanet.transform.position.y, __instance.camy.transform.position.z);
                __instance.camy.timer = 0f;
            }
            if (__instance.camy.isPulsingOnHit)
                __instance.camy.Pulse();
            if (__instance.isEditingLevel)
                scnEditor.instance.OttoBlink();
            if (__instance.currFloor.midSpin)
            {
                __instance.midspinInfiniteMargin = true;
                __instance.keyTimes.Add(Time.timeAsDouble);
            }
            else __instance.midspinInfiniteMargin = false;
            __instance.chosenplanet.Update_RefreshAngles();
            return false;
        }
    }
}
