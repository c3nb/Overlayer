using HarmonyLib;
using UnityEngine;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrHitErrorMeter), "AddHit")]
    public static class ErrorMeterUnlocker
    {
        public static void Prefix(ref float angleDiff)
        {
            scrConductor conductor = scrConductor.instance;
            if (conductor == null || !conductor) return;
			if (RDC.auto && Settings.Instance.UnlockErrorMeterAtAuto)
            {
                scrController ctrl = conductor.controller;
                scrPlanet cPlanet = ctrl.chosenplanet;
                float angle = (float)(cPlanet.angle - cPlanet.targetExitAngle);
                if (ctrl.isCW) angle *= -1f;
                angleDiff = angle / (float)ctrl.currFloor.marginScale;
            }
        }
    }
}
