using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;
using System.Runtime.CompilerServices;
using static UnityModManagerNet.UnityModManager.UI;

namespace Overlayer
{
    public static class Utils
    {
        public static readonly string[] FontNames;
        public static readonly Dictionary<string, Font> Fonts;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Utils()
        {  
            FontNames = Font.GetOSInstalledFontNames();
            Fonts = new Dictionary<string, Font>
            {
                { "Default", RDString.GetFontDataForLanguage(RDString.language).font }
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrySetFont(string name, Action<Font> setter)
        {
            if (TryGetFont(name, out var font))
                setter(font);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetFont(string name, out Font font)
        {
            if (Fonts.TryGetValue(name, out font))
                return true;
            else if (FontNames.Contains(name))
            {
                font = Fonts[name] = Font.CreateDynamicFontFromOSFont(name, 1);
                return true;
            }
            else return false;
        }
        public static double GetAdjustedAngleBoundaryInDeg(Difficulty diff, HitMarginGeneral marginType, double bpmTimesSpeed, double conductorPitch, double marginMult = 1.0)
        {
            float num = 0.065f;
            if (diff == Difficulty.Lenient)
                num = 0.091f;
            if (diff == Difficulty.Normal)
                num = 0.065f;
            if (diff == Difficulty.Strict)
                num = 0.04f;
            bool isMobile = ADOBase.isMobile;
            num = isMobile ? 0.09f : (num / GCS.currentSpeedTrial);
            float num2 = isMobile ? 0.07f : (0.03f / GCS.currentSpeedTrial);
            float a = isMobile ? 0.05f : (0.02f / GCS.currentSpeedTrial);
            num = Mathf.Max(num, 0.025f);
            num2 = Mathf.Max(num2, 0.025f);
            double num3 = (double)Mathf.Max(a, 0.025f);
            double val = scrMisc.TimeToAngleInRad((double)num, bpmTimesSpeed, conductorPitch, false) * 57.295780181884766;
            double val2 = scrMisc.TimeToAngleInRad((double)num2, bpmTimesSpeed, conductorPitch, false) * 57.295780181884766;
            double val3 = scrMisc.TimeToAngleInRad(num3, bpmTimesSpeed, conductorPitch, false) * 57.295780181884766;
            double result = Math.Max(GCS.HITMARGIN_COUNTED * marginMult, val);
            double result2 = Math.Max(45.0 * marginMult, val2);
            double result3 = Math.Max(30.0 * marginMult, val3);
            if (marginType == HitMarginGeneral.Counted)
                return result;
            if (marginType == HitMarginGeneral.Perfect)
                return result2;
            if (marginType == HitMarginGeneral.Pure)
                return result3;
            return result;
        }
        public static HitMargin GetHitMargin(Difficulty diff, float hitangle, float refangle, bool isCW, float bpmTimesSpeed, float conductorPitch, double marginScale)
        {
            float num = (hitangle - refangle) * (isCW ? 1 : -1);
            HitMargin result = HitMargin.TooEarly;
            float num2 = num;
            num2 = 57.29578f * num2;
            double adjustedAngleBoundaryInDeg = GetAdjustedAngleBoundaryInDeg(diff, HitMarginGeneral.Counted, (double)bpmTimesSpeed, (double)conductorPitch, marginScale);
            double adjustedAngleBoundaryInDeg2 = GetAdjustedAngleBoundaryInDeg(diff, HitMarginGeneral.Perfect, (double)bpmTimesSpeed, (double)conductorPitch, marginScale);
            double adjustedAngleBoundaryInDeg3 = GetAdjustedAngleBoundaryInDeg(diff, HitMarginGeneral.Pure, (double)bpmTimesSpeed, (double)conductorPitch, marginScale);
            if ((double)num2 > -adjustedAngleBoundaryInDeg)
                result = HitMargin.VeryEarly;
            if ((double)num2 > -adjustedAngleBoundaryInDeg2)
                result = HitMargin.EarlyPerfect;
            if ((double)num2 > -adjustedAngleBoundaryInDeg3)
                result = HitMargin.Perfect;
            if ((double)num2 > adjustedAngleBoundaryInDeg3)
                result = HitMargin.LatePerfect;
            if ((double)num2 > adjustedAngleBoundaryInDeg2)
                result = HitMargin.VeryLate;
            if ((double)num2 > adjustedAngleBoundaryInDeg)
                result = HitMargin.TooLate;
            return result;
        }
        public static HitMargin GetHitMarginForDifficulty(ADOBase adoBase, Difficulty diff)
        {
            scrController ctrl = adoBase.controller;
            scrPlanet planet = ctrl.chosenplanet;
            scrConductor conductor = adoBase.conductor;
            var marginScale = (planet.currfloor.nextfloor == null) ? 1.0 : planet.currfloor.nextfloor.marginScale;
            return GetHitMargin(diff, (float)planet.angle, (float)planet.targetExitAngle, ctrl.isCW, (float)(conductor.bpm * ctrl.speed), conductor.song.pitch, marginScale);
        }
        public static int GetCurDiffCount(HitMargin hit)
        {
            switch (GCS.difficulty)
            {
                case Difficulty.Lenient: return Variables.LenientCounts[hit];
                case Difficulty.Normal: return Variables.NormalCounts[hit];
                case Difficulty.Strict: return Variables.StrictCounts[hit];
                default: return 0;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HitMargin GetCurHitMargin(Difficulty diff)
        {
            switch (diff)
            {
                case Difficulty.Lenient: return Variables.Lenient;
                case Difficulty.Normal: return Variables.Normal;
                case Difficulty.Strict: return Variables.Strict;
                default: return 0;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IndentGUI(Action GUI, float verticalSpace = 0f, float indentSize = 20f)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentSize);
            GUILayout.BeginVertical();
            GUILayout.Space(verticalSpace);
            GUI();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DrawColor(ref float[] color, GUIStyle style = null, params GUILayoutOption[] option) => DrawFloatMultiField(ref color, new string[]
            {
        "<color=#FF0000>R</color>",
        "<color=#00FF00>G</color>",
        "<color=#0000FF>B</color>",
        "A"
            }, style, option);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DrawTextArea(ref string value, string label, GUIStyle style = null, params GUILayoutOption[] option)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(label, new GUILayoutOption[]
            {
            GUILayout.ExpandWidth(false)
            });
            string text = GUILayout.TextArea(value, style ?? GUI.skin.textArea, option);
            GUILayout.EndHorizontal();
            if (text != value)
            {
                value = text;
                return true;
            }
            value = text;
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DrawTextField(ref string value, string label, GUIStyle style = null, params GUILayoutOption[] option)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(label, new GUILayoutOption[]
            {
            GUILayout.ExpandWidth(false)
            });
            string text = GUILayout.TextField(value, style ?? GUI.skin.textArea, option);
            GUILayout.EndHorizontal();
            if (text != value)
            {
                value = text;
                return true;
            }
            value = text;
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetThis<T>(this T @this, out T t) => t = @this;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DrawEnum<T>(string title, ref T @enum) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            string[] names = values.Select(x => x.ToString()).ToArray();
            int selected = Array.IndexOf(values, @enum);
            bool result = PopupToggleGroup(ref selected, names, title);
            @enum = values[selected];
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] CopyArray<T>(T[] array)
        {
            var len = array.Length;
            T[] arr = new T[len];
            Array.Copy(array, 0, arr, 0, len);
            return arr;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ComputeTagValue(string val, string val2, string op)
        {
            double a = GetTagOrVal(val), b = GetTagOrVal(val2);
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "%": return a % b;
                default: return a;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetTagOrVal(string val)
        {
            if (Tag.NameTags.TryGetValue(val, out var tag))
                return tag.NumberGetter();
            return double.Parse(val);
        }
#if KV
        public static Color GetColor(this HitMargin hit)
        {
            Color result;
            switch (hit)
            {
                case HitMargin.TooEarly:
                    result = Settings.Instance.te;
                    break;
                case HitMargin.VeryEarly:
                    result = Settings.Instance.ve;
                    break;
                case HitMargin.EarlyPerfect:
                    result = Settings.Instance.ep;
                    break;
                case HitMargin.Perfect:
                    result = Settings.Instance.p;
                    break;
                case HitMargin.LatePerfect:
                    result = Settings.Instance.lp;
                    break;
                case HitMargin.VeryLate:
                    result = Settings.Instance.vl;
                    break;
                case HitMargin.TooLate:
                    result = Settings.Instance.tl;
                    break;
                case HitMargin.Multipress:
                    result = Settings.Instance.mp;
                    break;
                default:
                    result = Color.white;
                    break;
            }
            return result;
        }
#endif
    }
}
