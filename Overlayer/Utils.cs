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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                return double.Parse(tag.Value);
            return double.Parse(val);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Replace(string source)
        {
            int maxTagLength = Tag.NameTags.Select(t => t.Key).Select(k => k.Length).Max();
            int len = source.Length;
            StringBuilder sb = new StringBuilder(len);
            StringBuilder tagSb = new StringBuilder(maxTagLength);
            bool tagMode = false;
            for (int i = 0; i < len; i++)
            {
                char c = source[i];
                if (tagMode)
                {
                    if (c == '{')
                    {
                        sb.Append(tagSb);
                        tagSb.Clear();
                        tagSb.Append(c);
                    }
                    else if (c == '}')
                    {
                        tagSb.Append(c);
                        Tag tag = Tag.NameTags[tagSb.ToString()];
                        if (tag != null) sb.Append(tag.Value);
                        else sb.Append(tagSb);
                        tagSb.Clear();
                        tagMode = false;
                    }
                    else tagSb.Append(c);
                }
                else
                {
                    if (c == '{')
                    {
                        tagMode = true;
                        tagSb.Append(c);
                    }
                    else sb.Append(c);
                }
            }
            return sb.ToString();
        }
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
    }
}
