using System;
using HarmonyLib;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
    public static class BpmUpdater
    {
        public static float pitch, bpm;
        public static void Postfix(scrPlanet __instance, scrFloor floor)
        {
            if (!OText.IsPlaying) return;
            double speed, curBPM;
            speed = __instance.controller.speed;
            curBPM = GetRealBpm(floor, bpm);
            Settings setting = Settings.Instance;
            Variables.TileBpm = Math.Round(bpm * speed, setting.TileBpmDecimals);
            Variables.CurBpm = Math.Round(curBPM, setting.PerceivedBpmDecimals);
            Variables.RecKPS = Math.Round(curBPM / 60, setting.PerceivedKpsDecimals);
        }
        public static bool DoubleEqual(double f1, double f2) => Math.Abs(f1 - f2) < 0.0001;
        public static double GetRealBpm(scrFloor floor, float bpm)
        {
            double val = scrMisc.GetAngleMoved(floor.entryangle, floor.exitangle, !floor.isCCW) / 3.1415927410125732 * 180;
            double angle = Math.Round(val);
            double speed = floor.controller.speed;
            if (angle == 0) angle = 360;
            return 180 / angle * (speed * bpm);
        }
    }
}
