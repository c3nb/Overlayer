using HarmonyLib;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(CustomLevel), "Play")]
    public static class StartProgUpdater
    {
        public static void Postfix(CustomLevel __instance)
        {
            Variables.StartProg = __instance.controller.percentComplete * 100;
        }
    }
}
