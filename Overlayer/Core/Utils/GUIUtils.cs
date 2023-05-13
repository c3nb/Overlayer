using Overlayer.Core.Translation;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityModManagerNet.UnityModManager.UI;

namespace Overlayer.Core.Utils
{
    public static class GUIUtils
    {
        public static void BeginIndent(float hIndent = 20f, float vIndent = 0f)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(hIndent);
            GUILayout.BeginVertical();
            GUILayout.Space(vIndent);
        }
        public static bool DrawColor(ref VertexGradient color, GUIStyle style = null, params GUILayoutOption[] option)
        {
            bool result = false;

            GUILayout.BeginHorizontal();
            GUILayout.Label(Main.Language[TranslationKeys.TopLeft]);
            GUILayout.Space(1);
            result |= DrawColor(ref color.topLeft, style, option);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Main.Language[TranslationKeys.TopRight]);
            GUILayout.Space(1);
            result |= DrawColor(ref color.topRight, style, option);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Main.Language[TranslationKeys.BottomLeft]);
            GUILayout.Space(1);
            result |= DrawColor(ref color.bottomLeft, style, option);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Main.Language[TranslationKeys.BottomRight]);
            GUILayout.Space(1);
            result |= DrawColor(ref color.bottomRight, style, option);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            return result;
        }
        public static bool DrawColor(ref Color color, GUIStyle style = null, params GUILayoutOption[] option)
        {
            float[] arr = new float[] { color.r, color.g, color.b, color.a };
            bool result = DrawColor(ref arr, style, option);
            if (result)
            {
                color.r = arr[0];
                color.g = arr[1];
                color.b = arr[2];
                color.a = arr[3];
            }
            return result;
        }
        public static bool DrawColor(ref float[] color, GUIStyle style = null, params GUILayoutOption[] option)
            => DrawFloatMultiField(ref color, new string[]
            {
                "<color=#FF0000>R</color>",
                "<color=#00FF00>G</color>",
                "<color=#0000FF>B</color>",
                "A"
            }, style, option);
        public static bool DrawEnum<T>(string title, ref T @enum) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            string[] names = values.Select(x => x.ToString()).ToArray();
            int selected = Array.IndexOf(values, @enum);
            bool result = PopupToggleGroup(ref selected, names, title);
            @enum = values[selected];
            return result;
        }
        public static bool DrawTextArea(ref string value, string label, GUIStyle style = null, params GUILayoutOption[] option)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(label, new GUILayoutOption[] { GUILayout.ExpandWidth(false) });
            string text = GUILayout.TextArea(value, style ?? GUI.skin.textArea, option);
            GUILayout.FlexibleSpace();
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
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            string text = GUILayout.TextField(value, style ?? GUI.skin.textArea, option);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (text != value)
            {
                value = text;
                return true;
            }
            value = text;
            return false;
        }
        public static void EndIndent()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        public static bool IsMouseHovering()
        {
            return Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
        }
        public static bool RightToggle(bool value, string label, Action<bool> onChange = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            bool prev = value;
            value = GUILayout.Toggle(value, "");
            if (prev != value) onChange?.Invoke(value);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return value;
        }
    }
}