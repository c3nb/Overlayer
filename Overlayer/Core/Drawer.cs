using Overlayer.Core.Interfaces;
using Overlayer.Models;
using Overlayer.Utils;
using System;
using System.Linq;
using UnityEngine;
using TKM = Overlayer.Core.Translation.TranslationKeys.Misc;

namespace Overlayer.Core
{
    public static class Drawer
    {
        public static bool DrawVector2(string label, ref Vector2 vec2, float lValue, float rValue)
        {
            bool changed = false;
            ButtonLabel($"<b>{label}</b>", Main.OpenDiscordLink);
            changed |= DrawSingleWithSlider("X", ref vec2.x, lValue, rValue, 300f);
            changed |= DrawSingleWithSlider("Y", ref vec2.y, lValue, rValue, 300f);
            return changed;
        }
        public static bool DrawVector3(string label, ref Vector3 vec3, float lValue, float rValue)
        {
            bool changed = false;
            ButtonLabel($"<b>{label}</b>", Main.OpenDiscordLink);
            changed |= DrawSingleWithSlider("X", ref vec3.x, lValue, rValue, 300f);
            changed |= DrawSingleWithSlider("Y", ref vec3.y, lValue, rValue, 300f);
            changed |= DrawSingleWithSlider("Z", ref vec3.z, lValue, rValue, 300f);
            return changed;
        }
        public static void DrawGColor(string label, ref GColor color, bool canEnableGradient, Action onChange)
        {
            ButtonLabel(label, Main.OpenDiscordLink);
            DrawGColor(ref color, canEnableGradient).IfTrue(onChange);
        }
        public static bool DrawGColor(ref GColor color, bool canEnableGradient)
        {
            bool ge = color.gradientEnabled, prevGe = color.gradientEnabled;
            if (canEnableGradient && DrawBool(Main.Lang[TKM.EnableGradient], ref ge))
                color = color with { gradientEnabled = ge };
            color.gradientEnabled &= canEnableGradient;
            bool result = ge != prevGe;
            if (color.gradientEnabled)
            {
                Color tl = color.topLeft, tr = color.topRight,
                bl = color.bottomLeft, br = color.bottomRight;
                ExpandableGUI(color.topLeftStatus, Main.Lang[TKM.TopLeft], () => result |= DrawColor(ref tl));
                ExpandableGUI(color.topRightStatus, Main.Lang[TKM.TopRight], () => result |= DrawColor(ref tr));
                ExpandableGUI(color.bottomLeftStatus, Main.Lang[TKM.BottomLeft], () => result |= DrawColor(ref bl));
                ExpandableGUI(color.bottomRightStatus, Main.Lang[TKM.BottomRight], () => result |= DrawColor(ref br));
                if (result)
                {
                    color.topLeft = tl;
                    color.topRight = tr;
                    color.bottomLeft = bl;
                    color.bottomRight = br;
                }
            }
            else
            {
                Color dummy = color.topLeft;
                if (result = DrawColor(ref dummy)) color = dummy;
            }
            return result;
        }
        public static void ExpandableGUI(GUIStatus status, string label, Action drawer)
        {
            GUILayoutEx.ExpandableGUI(drawer, label, ref status.Expanded);
        }
        public static bool DrawColor(ref Color color)
        {
            bool result = false;
            result |= DrawSingleWithSlider("<color=#FF0000>R</color>", ref color.r, 0, 1, 300f);
            result |= DrawSingleWithSlider("<color=#00FF00>G</color>", ref color.g, 0, 1, 300f);
            result |= DrawSingleWithSlider("<color=#0000FF>B</color>", ref color.b, 0, 1, 300f);
            result |= DrawSingleWithSlider("A", ref color.a, 0, 1, 300f);
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            if (DrawString("Hex:", ref hex))
            {
                result = true;
                ColorUtility.TryParseHtmlString("#" + hex, out color);
            }
            return result;
        }
        public static void TitleButton(string label, string btnLabel, Action pressed, Action horizontal = null)
        {
            GUILayout.BeginHorizontal();
            ButtonLabel(label, Main.OpenDiscordLink);
            if (GUILayout.Button(btnLabel))
                pressed?.Invoke();
            horizontal?.Invoke();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static bool DrawSingleWithSlider(string label, ref float value, float lValue, float rValue, float width)
        {
            GUILayout.BeginHorizontal();
            float newValue = GUILayoutEx.NamedSliderContent(label, value, lValue, rValue, width);
            GUILayout.EndHorizontal();
            bool result = newValue != value;
            value = newValue;
            return result;
        }
        public static bool DrawStringArray(ref string[] array, Action<int> arrayResized = null, Action<int> elementRightGUI = null, Action<int, string> onElementChange = null)
        {
            bool result = false;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                Array.Resize(ref array, array.Length + 1);
                arrayResized?.Invoke(array.Length);
                result = true;
            }
            if (array.Length > 0 && GUILayout.Button("-"))
            {
                Array.Resize(ref array, array.Length - 1);
                arrayResized?.Invoke(array.Length);
                result = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            for (int i = 0; i < array.Length; i++)
            {
                string cache = array[i];
                GUILayout.BeginHorizontal();
                Drawer.ButtonLabel($"{i}: ", Main.OpenDiscordLink);
                cache = GUILayout.TextField(cache);
                elementRightGUI?.Invoke(i);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (cache != array[i])
                {
                    array[i] = cache;
                    onElementChange?.Invoke(i, cache);
                }
            }
            return result;
        }
        public static bool DrawArray(string label, ref object[] array)
        {
            bool result = false;
            Drawer.ButtonLabel(label, Main.OpenDiscordLink);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
                Array.Resize(ref array, array.Length + 1);
            if (array.Length > 0 && GUILayout.Button("-"))
                Array.Resize(ref array, array.Length - 1);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for (int i = 0; i < array.Length; i++)
                result |= DrawObject($"{i}: ", ref array[i]);
            GUILayout.EndVertical();
            return result;
        }
        public static bool DrawBool(string label, ref bool value)
        {
            bool prev = value;
            GUILayout.BeginHorizontal();
            Drawer.ButtonLabel(label, Main.OpenDiscordLink);
            value = GUILayout.Toggle(value, string.Empty);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return prev != value;
        }
        public static bool DrawByte(string label, ref byte value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt8(str);
            return result;
        }
        public static bool DrawDouble(string label, ref double value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToDouble(str);
            return result;
        }
        public static bool DrawEnum<T>(string label, ref T @enum, int unique = 0) where T : Enum
        {
            int current = EnumHelper<T>.IndexOf(@enum);
            string[] names = EnumHelper<T>.GetNames();
            bool result = UnityModManagerNet.UnityModManager.UI.PopupToggleGroup(ref current, names, label, unique);
            @enum = EnumHelper<T>.GetValues()[current];
            return result;
        }
        public static bool DrawInt16(string label, ref short value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt16(str);
            return result;
        }
        public static bool DrawInt32(string label, ref int value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt32(str);
            return result;
        }
        public static bool DrawInt64(string label, ref long value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt64(str);
            return result;
        }
        public static void DrawObject(string label, object value)
        {
            if (value == null) return;
            if (value is IDrawable drawable)
            {
                drawable.Draw();
                return;
            }
            Type t = value.GetType();
            if (!t.IsPrimitive && t != typeof(string)) return;
            var fields = t.GetFields();
            foreach (var field in fields)
            {
                var fValue = field.GetValue(value);
                if (DrawObject(field.Name, ref fValue))
                    field.SetValue(value, fValue);
            }
            var props = t.GetProperties();
            foreach (var prop in props.Where(p => p.CanRead && p.CanWrite))
            {
                var pValue = prop.GetValue(value);
                if (DrawObject(prop.Name, ref pValue))
                    prop.SetValue(value, pValue);
            }
        }
        public static bool DrawObject(string label, ref object obj)
        {
            bool result = false;
            switch (obj)
            {
                case bool bb:
                    result = DrawBool(label, ref bb);
                    obj = bb;
                    break;
                case sbyte sb:
                    result = DrawSByte(label, ref sb);
                    obj = sb;
                    break;
                case byte b:
                    result = DrawByte(label, ref b);
                    obj = b;
                    break;
                case short s:
                    result = DrawInt16(label, ref s);
                    obj = s;
                    break;
                case ushort us:
                    result = DrawUInt16(label, ref us);
                    obj = us;
                    break;
                case int i:
                    result = DrawInt32(label, ref i);
                    obj = i;
                    break;
                case uint ui:
                    result = DrawUInt32(label, ref ui);
                    obj = ui;
                    break;
                case long l:
                    result = DrawInt64(label, ref l);
                    obj = l;
                    break;
                case ulong ul:
                    result = DrawUInt64(label, ref ul);
                    obj = ul;
                    break;
                case float f:
                    result = DrawSingle(label, ref f);
                    obj = f;
                    break;
                case double d:
                    result = DrawDouble(label, ref d);
                    obj = d;
                    break;
                case string str:
                    result = DrawString(label, ref str);
                    obj = str;
                    break;
                default:
                    Drawer.ButtonLabel($"{label}{obj}", Main.OpenDiscordLink);
                    break;
            }
            return result;
        }
        public static bool DrawSByte(string label, ref sbyte value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToInt8(str);
            return result;
        }
        public static bool DrawSingle(string label, ref float value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToFloat(str);
            return result;
        }
        public static bool DrawString(string label, ref string value, bool textArea = false)
        {
            string prev = value;
            GUILayout.BeginHorizontal();
            ButtonLabel(label, Main.OpenDiscordLink);
            if (!textArea)
                value = GUILayout.TextField(value);
            else value = GUILayout.TextArea(value);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return prev != value;
        }
        public static bool DrawToggleGroup(string[] labels, bool[] toggleGroup)
        {
            bool result = false;
            for (int i = 0; i < labels.Length; i++)
                if (DrawBool(labels[i], ref toggleGroup[i]))
                {
                    result = true;
                    for (int j = 0; j < toggleGroup.Length; j++)
                        if (j == i) continue;
                        else toggleGroup[j] = false;
                    break;
                }
            return result;
        }
        public static bool DrawUInt16(string label, ref ushort value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt16(str);
            return result;
        }
        public static bool DrawUInt32(string label, ref uint value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt32(str);
            return result;
        }
        public static bool DrawUInt64(string label, ref ulong value)
        {
            string str = value.ToString();
            bool result = DrawString(label, ref str);
            value = StringConverter.ToUInt64(str);
            return result;
        }
        public static void ButtonLabel(string label, Action onPressed, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(label, GUI.skin.label, options))
                onPressed?.Invoke();
        }
    }
}
