using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;
using System.Globalization;
using static UnityModManagerNet.UnityModManager.UI;

namespace Overlayer
{
    public static class Utils
    {
        public static readonly string[] FontNames;
        public static readonly Dictionary<string, Font> Fonts;
        static Utils()
        {  
            FontNames = Font.GetOSInstalledFontNames();
            Fonts = new Dictionary<string, Font>();
        }
        public static Font TryGetFont(string name)
        {
            if (Fonts.TryGetValue(name, out var font))
                return font;
            else if (FontNames.Contains(name))
                return Fonts[name] = Font.CreateDynamicFontFromOSFont(name, 1);
            else return null;
        }
        public static HitMargin GetHitMargin(float angle)
        {
            double bpmTimesSpeed = scrConductor.instance.bpm * scrController.instance.speed;
            double conductorPitch = scrConductor.instance.song.pitch;
            double counted =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Counted, bpmTimesSpeed, conductorPitch);
            double perfect =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Perfect, bpmTimesSpeed, conductorPitch);
            double pure =
                scrMisc.GetAdjustedAngleBoundaryInDeg(
                    HitMarginGeneral.Pure, bpmTimesSpeed, conductorPitch);
            if (angle < -counted) return HitMargin.TooEarly;
            else if (angle < -perfect) return HitMargin.VeryEarly;
            else if (angle < -pure) return HitMargin.EarlyPerfect;
            else if (angle <= pure) return HitMargin.Perfect;
            else if (angle <= perfect) return HitMargin.LatePerfect;
            else if (angle <= counted) return HitMargin.VeryLate;
            else return HitMargin.TooLate;
        }
        public static HitMargin GetHitMarginForDifficulty(float angle, Difficulty difficulty)
        {
            Difficulty temp = GCS.difficulty;
            GCS.difficulty = difficulty;
            HitMargin margin = GetHitMargin(angle);
            GCS.difficulty = temp;
            return margin;
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
        public static bool DrawColor(ref float[] color, GUIStyle style = null, params GUILayoutOption[] option) => DrawFloatMultiField(ref color, new string[]
            {
        "<color=#FF0000>R</color>",
        "<color=#00FF00>G</color>",
        "<color=#0000FF>B</color>",
        "A"
            }, style, option);
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
        public static T GetThis<T>(this T @this, out T t) => t = @this;
        public static bool DrawEnum<T>(string title, ref T @enum) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            string[] names = values.Select(x => x.ToString()).ToArray();
            int selected = Array.IndexOf(values, @enum);
            bool result = PopupToggleGroup(ref selected, names, title);
            @enum = values[selected];
            return result;
        }
        public static T[] CopyArray<T>(T[] array)
        {
            var len = array.Length;
            T[] arr = new T[len];
            Array.Copy(array, 0, arr, 0, len);
            return arr;
        }
        public static double ComputeTagValue(string val, string val2, string op)
        {
            double a, b;
            if (Tag.NameTags.TryGetValue(val, out var tag))
                a = double.Parse(tag.Value);
            else a = double.Parse(val);
            if (Tag.NameTags.TryGetValue(val2, out tag))
                b = double.Parse(tag.Value);
            else b = double.Parse(val);
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
    }
}
