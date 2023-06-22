using HarmonyLib;
using ADOFAI;
using UnityEngine;
using System;
using Overlayer.Scripting;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(PropertyControl_Text), "Validate")]
    public static class ValidatePatch
    {
        public static bool Prefix(PropertyControl_Text __instance, ref string __result)
        {
            if (__instance.propertyInfo == null)
            {
                __result = __instance.inputField.text;
                return false;
            }
            if (__instance.propertyInfo.type == PropertyType.Float)
            {
                if (float.TryParse(__instance.inputField.text, out float value))
                    value = __instance.propertyInfo.Validate(value);
                else
                {
                    try { value = Convert.ToSingle(Script.EvaluateSource(__instance.inputField.text, ScriptType.JavaScript)); }
                    catch { value = (float)__instance.propertyInfo.value_default; }
                }
                __result = value.ToString();
                return false;
            }
            if (__instance.propertyInfo.type == PropertyType.Int || __instance.propertyInfo.type == PropertyType.Tile)
            {
                int value2;
                if (float.TryParse(__instance.inputField.text, out float f))
                {
                    value2 = Mathf.RoundToInt(f);
                    value2 = __instance.propertyInfo.Validate(value2);
                }
                else
                {
                    try { value2 = Convert.ToInt32(Script.EvaluateSource(__instance.inputField.text, ScriptType.JavaScript)); }
                    catch { value2 = (int)__instance.propertyInfo.value_default; }
                }
                __result = value2.ToString();
                return false;
            }
            __result = __instance.inputField.text;
            return false;
        }
    }
}