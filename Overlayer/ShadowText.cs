using TagLib.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.Pool;
#pragma warning disable

namespace Overlayer
{
    public class ShadowText : MonoBehaviour
    {
        public static int Count = 0;
        public static Canvas PublicCanvas;
        public static ShadowText NewText()
        {
            int count = ++Count;
            ShadowText st = new GameObject($"ShadowText_{count}").AddComponent<ShadowText>();
            st.Number = count;
            return st;
        }
        public static Dictionary<string, FontData> Fonts = new Dictionary<string, FontData>();
        public static TMP_FontAsset DefaultTMPFont;
        public static Font DefaultFont;
        public Action Updater;
        public TextMeshProUGUI Main;
        public TextMeshProUGUI Shadow;
        public int Number;
        public TextAlignmentOptions Alignment
        {
            get => Main.alignment;
            set
            {
                Main.alignment = value;
                Shadow.alignment = value;
            }
        }
        public float FontSize
        {
            get => Main.fontSize;
            set
            {
                Main.fontSize = value;
                Shadow.fontSize = value;
                var xy = FontSize / 20f;
                Shadow.rectTransform.anchoredPosition = Position + new Vector2(xy, -xy);
            }
        }
        public Color Color
        {
            get => Main.color;
            set => Main.color = value;
        }
        public Vector2 Center
        {
            get => Main.rectTransform.anchorMin;
            set
            {
                Main.rectTransform.anchorMin = value;
                Main.rectTransform.anchorMax = value;
                Main.rectTransform.pivot = value;

                Shadow.rectTransform.anchorMin = value;
                Shadow.rectTransform.anchorMax = value;
                Shadow.rectTransform.pivot = value;
            }
        }
        public Vector2 Position
        {
            get => Main.rectTransform.anchoredPosition;
            set
            {
                Main.rectTransform.anchoredPosition = value;
                var xy = FontSize / 20f;
                Shadow.rectTransform.anchoredPosition = value + new Vector2(xy, -xy);
            }
        }
        private void Awake()
        {
            if (DefaultFont == null)
                DefaultFont = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
            if (DefaultTMPFont == null)
                DefaultTMPFont = TMP_FontAsset.CreateFontAsset(DefaultFont, 100, 10, GlyphRenderMode.SDFAA, 1024, 1024);
            if (!PublicCanvas)
            {
                GameObject pCanvasObj = new GameObject("Overlayer Canvas");
                PublicCanvas = pCanvasObj.AddComponent<Canvas>();
                PublicCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler scaler = pCanvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                DontDestroyOnLoad(PublicCanvas);
            }

            GameObject shadowObject = new GameObject();
            shadowObject.transform.SetParent(PublicCanvas.transform);
            shadowObject.MakeFlexible();
            Shadow = shadowObject.AddComponent<TextMeshProUGUI>();
            Shadow.color = Color.black.WithAlpha(0.4f);
            Shadow.font = DefaultTMPFont;

            GameObject mainObject = new GameObject();
            mainObject.transform.SetParent(PublicCanvas.transform);
            mainObject.MakeFlexible();
            Main = mainObject.AddComponent<TextMeshProUGUI>();
            Main.font = DefaultTMPFont;
            Main.enableVertexGradient = true;

            Main.lineSpacing -= 25f;
            Main.lineSpacingAdjustment = 25f;

            Shadow.lineSpacing -= 25f;
            Shadow.lineSpacingAdjustment = 25f;
        }
        private void Update() => Updater();
        static bool initialized;
        internal static string[] fontNames;
        public bool TrySetFont(string name)
        {
            if (!initialized)
            {
                fontNames = Font.GetOSInstalledFontNames();
                Fonts = new Dictionary<string, FontData>();
                initialized = true;
            }
            if (name == "Default")
            {
                Main.font = DefaultTMPFont;
                Shadow.font = DefaultTMPFont;
                return true;
            }
            if (Fonts.TryGetValue(name, out FontData data))
            {
                Main.font = data.fontTMP;
                Shadow.font = data.fontTMP;
                return true;
            }
            else
            {
                int index = Array.IndexOf(fontNames, name);
                if (index != -1)
                {
                    FontData newData = new FontData();
                    Font newFont = Font.CreateDynamicFontFromOSFont(name, 1);
                    TMP_FontAsset newTMPFont = TMP_FontAsset.CreateFontAsset(newFont);
                    Main.font = newTMPFont;
                    Shadow.font = newTMPFont;
                    newData.font = newFont;
                    newData.fontTMP = newTMPFont;
                    Fonts.Add(name, newData);
                    return true;
                }
                return false;
            }
        }
        public bool Active
        {
            get => Main.gameObject.activeSelf;
            set
            {
                Main.gameObject.SetActive(value);
                Shadow.gameObject.SetActive(value);
            }
        }
    }
}
