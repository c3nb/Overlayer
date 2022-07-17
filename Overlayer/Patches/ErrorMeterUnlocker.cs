using HarmonyLib;

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
                scrController ctrl = scrController.instance;
                scrPlanet cPlanet = ctrl.chosenplanet;
                float angle = (float)(cPlanet.angle - cPlanet.targetExitAngle);
                if (ctrl.isCW) angle *= -1f;
                angleDiff = angle / (float)ctrl.currFloor.marginScale;
            }
        }
    }
}
