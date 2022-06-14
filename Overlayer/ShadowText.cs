using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#pragma warning disable

namespace Overlayer
{
    public class ShadowText : MonoBehaviour
    {
        public static int Count = 0;
        public static ShadowText NewText()
        {
            int count;
            ShadowText st = new GameObject($"ShadowText_{count = ++Count}").AddComponent<ShadowText>();
            st.Number = count;
            return st;
        }
        public static readonly Dictionary<string, TMP_FontAsset> Fonts = new Dictionary<string, TMP_FontAsset>();
        public Action Updater;
        public TextMeshProUGUI Main;
        public Text Shadow;
        public int Number;
        public TextAnchor Alignment
        {
            get => Main.alignment.ToAnchor();
            set
            {
                Main.alignment = value.ToAlignment();
                Shadow.alignment = value;
            }
        }
        public Font Font
        {
            get => Main.font.sourceFontFile;
            set
            {
                if (Fonts.TryGetValue(value.name, out var fontAsset))
                    Main.font = fontAsset;
                else Main.font = Fonts[value.name] = TMP_FontAsset.CreateFontAsset(value);
                Shadow.font = value;
            }
        }
        public int FontSize
        {
            get => (int)Main.fontSize;
            set
            {
                Main.fontSize = value;
                Shadow.fontSize = value;
                Shadow.rectTransform.anchoredPosition = Position + new Vector2(value / 20f, -value / 20f);
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
                Shadow.rectTransform.anchoredPosition = value + new Vector2(FontSize / 20f, -FontSize / 20f);
            }
        }
        private void Awake()
        {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            ContentSizeFitter fitter;
            SetActiveShadow(this);
            GameObject mainObject = new GameObject();
            mainObject.transform.SetParent(transform);
            fitter = mainObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            Main = mainObject.AddComponent<TextMeshProUGUI>();
            var font = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
            if (!Fonts.TryGetValue(font.name, out var fontAsset))
                Fonts[font.name] = fontAsset = Fonts[font.name] = TMP_FontAsset.CreateFontAsset(font);
            Main.font = fontAsset;
            Main.enableVertexGradient = true;
        }
        private void Update() => Updater();
        public static void SetActiveShadow(ShadowText text)
        {
            GameObject shadowObject = new GameObject();
            ContentSizeFitter fitter = shadowObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            shadowObject.transform.SetParent(text.transform);
            text.Shadow = shadowObject.AddComponent<Text>();
            text.Shadow.font = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
            text.Shadow.color = Color.black.WithAlpha(0.4f);
        }
    }
}
