using TMPro;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Reflection;

namespace TagLib.Utils
{
    public static class Extensions
    {
        public static T CheckNull<T>(this T obj, string identifier = null)
        {
            if (ReferenceEquals(obj, null))
                Main.Logger.Log($"{identifier ?? typeof(T).ToString()} is Null!");
            else Main.Logger.Log($"{identifier ?? typeof(T).ToString()} is Not Null. ({obj})");
            return obj;
        }
        public static T MakeFlexible<T>(this T comp) where T : Component
        {
            comp.gameObject.MakeFlexible();
            return comp;
        }
        public static GameObject MakeFlexible(this GameObject go)
        {
            ContentSizeFitter csf = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return go;
        }
        public static Color ToColor(this float[] array)
        {
            if (array.Length > 3)
                return new Color(array[0], array[1], array[2], array[3]);
            return new Color(array[0], array[1], array[2]);
        }
        public static float[] ToArray(this Color color)
            => new float[] { color.r, color.g, color.b, color.a };
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
    }
}
