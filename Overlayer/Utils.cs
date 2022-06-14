using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Runtime.CompilerServices;
using HarmonyLib;
using static UnityModManagerNet.UnityModManager.UI;
using TMPro;

namespace Overlayer
{
    public static class Utils
    {
        public static readonly string[] FontNames;
        public static readonly Dictionary<string, Font> Fonts;
        public static void InjectUnderlay(this TextMeshPro tmp)
        {
            var mat = tmp.renderer.material;
            mat.EnableKeyword("UNDERLAY_ON");
            mat.SetFloat("_UnderlaySoftness", 0.3f);
            mat.SetFloat("_UnderlayDilate", 0.1f);
        }
        public static void InjectUnderlay(this TextMeshProUGUI tmp)
        {
            var mat = tmp.fontMaterial;
            mat.EnableKeyword("UNDERLAY_ON");
            mat.SetFloat("_UnderlaySoftness", 0.3f);
            mat.SetFloat("_UnderlayDilate", 0.1f);
        }
        public static void Log(this object obj)
            => Main.Logger.Log(obj.ObjToString());
        public static string ObjToString(this object obj, bool declaredOnly = true)
        {
            StringBuilder sb = new StringBuilder();
            List<object> visited = new List<object>();
            sb.AppendLine($"{obj.GetType().FullName}:");
            ObjToStringInternal(sb, obj, 2, visited);
            return sb.ToString();
        }
        static void ObjToStringInternal(StringBuilder sb, object obj, int offset = 0, List<object> visited = null, bool declaredOnly = true)
        {
            BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            if (declaredOnly) bf |= BindingFlags.DeclaredOnly;
            if (visited.Any(o => o.Equals(obj))) return;
            visited.Add(obj);
            if (obj == null)
            {
                sb.Append(' ', offset).AppendLine("null");
                return;
            }
            Type type = obj.GetType();
            var props = type.GetProperties(bf).Where(p => p.CanRead);
            var fields = type.GetFields(bf);
            if (fields.Any())
            {
                sb.Append(' ', offset).AppendLine("Fields:");
                foreach (var field in fields)
                {
                    Type fieldType = field.FieldType;
                    var value = default(object);
                    try { value = field.GetValue(obj); }
                    catch { value = null; }
                    if (fieldType.IsPointer)
                    {
                        sb.Append(' ', offset).AppendLine($"{fieldType} {field.Name}:{value}");
                        continue;
                    }
                    if (fieldType != typeof(object) && (fieldType.IsAssignableFrom(type) || type.IsAssignableFrom(fieldType)))
                    {
                        sb.Append(' ', offset).AppendLine($"{fieldType} {field.Name}:Cannot GetValue Due To Infinity Recursive Call");
                        continue;
                    }
                    if (value is IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                            ObjToStringInternal(sb, item, offset + 2, visited);
                    }
                    else if (fieldType.IsPrimitive || fieldType.IsEnum)
                        sb.Append(' ', offset).AppendLine($"{fieldType} {field.Name}:{value}");
                    else
                    {
                        sb.Append(' ', offset);
                        if (value != null)
                        {
                            sb.AppendLine($"{fieldType} {field.Name}:");
                            ObjToStringInternal(sb, value, offset + 2, visited);
                        }
                        else sb.AppendLine($"{fieldType} {field.Name}:null");
                    }
                }
            }
            if (props.Any())
            {
                sb.Append(' ', offset).AppendLine("Properties:");
                foreach (var prop in props)
                {
                    Type propType = prop.PropertyType;
                    var value = default(object);
                    try { value = prop.GetValue(obj); }
                    catch { value = null; }
                    if (propType.IsPointer)
                    {
                        sb.Append(' ', offset).AppendLine($"{propType} {prop.Name}:{value}");
                        continue;
                    }
                    if (propType != typeof(object) && (propType.IsAssignableFrom(type) || type.IsAssignableFrom(propType)))
                    {
                        sb.Append(' ', offset).AppendLine($"{propType} {prop.Name}:Cannot GetValue Due To Infinity Recursive Call");
                        continue;
                    }
                    var indexes = prop.GetIndexParameters();
                    if (indexes.Length != 0)
                    {
                        StringBuilder iSb = new StringBuilder();
                        for (int i = 0; i < indexes.Length; i++)
                        {
                            var index = indexes[i];
                            iSb.Append($"{index.ParameterType} {index.Name}");
                            if (i != indexes.Length - 1)
                                iSb.Append(", ");
                        }
                        sb.Append(' ', offset).Append($"Indexer:[{iSb}]");
                        continue;
                    }
                    if (value is IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                            ObjToStringInternal(sb, item, offset + 2, visited);
                    }
                    else if (propType.IsPrimitive || propType.IsEnum)
                        sb.Append(' ', offset).AppendLine($"{propType} {prop.Name}:{value}");
                    else
                    {
                        sb.Append(' ', offset);
                        if (value != null)
                        {
                            sb.AppendLine($"{propType} {prop.Name}:");
                            ObjToStringInternal(sb, value, offset + 2, visited);
                        }
                        else sb.AppendLine($"{propType} {prop.Name}:null");
                    }
                }
            }
        }
        public static bool IsPlaying(ADOBase adoBase) => !adoBase.controller.paused && adoBase.conductor.isGameWorld;
        static Utils()
        {  
            FontNames = Font.GetOSInstalledFontNames();
            Fonts = new Dictionary<string, Font>
            {
                { "Default", RDString.GetFontDataForLanguage(SystemLanguage.English).font }
            };
        }
        public static TextAnchor ToAnchor(this TextAlignmentOptions tao)
        {
            switch (tao)
            {
                case TextAlignmentOptions.TopLeft:
                    return TextAnchor.UpperLeft;
                case TextAlignmentOptions.Top:
                    return TextAnchor.UpperCenter;
                case TextAlignmentOptions.TopRight:
                    return TextAnchor.UpperRight;
                case TextAlignmentOptions.MidlineLeft:
                    return TextAnchor.MiddleLeft;
                case TextAlignmentOptions.Midline:
                    return TextAnchor.MiddleCenter;
                case TextAlignmentOptions.MidlineRight:
                    return TextAnchor.MiddleRight;
                case TextAlignmentOptions.BottomLeft:
                    return TextAnchor.LowerLeft;
                case TextAlignmentOptions.Bottom:
                    return TextAnchor.LowerCenter;
                case TextAlignmentOptions.BottomRight:
                    return TextAnchor.LowerRight;
                default: return TextAnchor.MiddleCenter;
            }
        }
        public static TextAlignmentOptions ToAlignment(this TextAnchor ta)
        {
            switch (ta)
            {
                case TextAnchor.UpperLeft:
                    return TextAlignmentOptions.TopLeft;
                case TextAnchor.UpperCenter:
                    return TextAlignmentOptions.Top;
                case TextAnchor.UpperRight:
                    return TextAlignmentOptions.TopRight;
                case TextAnchor.MiddleLeft:
                    return TextAlignmentOptions.MidlineLeft;
                case TextAnchor.MiddleCenter:
                    return TextAlignmentOptions.Midline;
                case TextAnchor.MiddleRight:
                    return TextAlignmentOptions.MidlineRight;
                case TextAnchor.LowerLeft:
                    return TextAlignmentOptions.BottomLeft;
                case TextAnchor.LowerCenter:
                    return TextAlignmentOptions.Bottom;
                case TextAnchor.LowerRight:
                    return TextAlignmentOptions.BottomRight;
                default: return TextAlignmentOptions.Midline;
            }
        }
        public static void TrySetFont(string name, Action<Font> setter)
        {
            if (TryGetFont(name, out var font))
                setter(font);
        }
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
        public static double GetTagOrVal(string val)
        {
            if (Tag.NameTags.TryGetValue(val, out var tag))
                return tag.NumberGetter();
            return double.Parse(val);
        }
    }
}
