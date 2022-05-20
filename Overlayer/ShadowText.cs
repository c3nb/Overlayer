using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UText = UnityEngine.UI.Text;
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
        public Action Updater;
        public UText Main;
        public UText Shadow;
        public int Number;
        public TextAnchor Alignment
        {
            get => Main.alignment;
            set
            {
                Main.alignment = value;
                if (Settings.Instance.Shadow)
                    Shadow.alignment = value;
            }
        }
        public Font Font
        {
            get => Main.font;
            set
            {
                Main.font = value;
                if (Settings.Instance.Shadow)
                    Shadow.font = value;
            }
        }
        public int FontSize
        {
            get => Main.fontSize;
            set
            {
                Main.fontSize = value;
                if (Settings.Instance.Shadow)
                {
                    Shadow.fontSize = value;
                    Shadow.rectTransform.anchoredPosition =
                        Position + new Vector2(value / 20f, -value / 20f);
                }
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
                if (Settings.Instance.Shadow)
                {
                    Shadow.rectTransform.anchorMin = value;
                    Shadow.rectTransform.anchorMax = value;
                    Shadow.rectTransform.pivot = value;
                }
            }
        }
        public Vector2 Position
        {
            get => Main.rectTransform.anchoredPosition;
            set
            {
                Main.rectTransform.anchoredPosition = value;
                if (Settings.Instance.Shadow)
                {
                    Shadow.rectTransform.anchoredPosition =
                                       value + new Vector2(FontSize / 20f, -FontSize / 20f);
                }

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
            SetActiveShadow(this, Settings.Instance.Shadow);
            GameObject mainObject = new GameObject();
            mainObject.transform.SetParent(transform);
            fitter = mainObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            Main = mainObject.AddComponent<UText>();
            Main.font = RDString.GetFontDataForLanguage(RDString.language).font;
        }
        private void Update() => Updater();
        public static void SetActiveShadow(ShadowText text, bool active)
        {
            if (active)
            {
                GameObject shadowObject = new GameObject();
                ContentSizeFitter fitter = shadowObject.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                shadowObject.transform.SetParent(text.transform);
                text.Shadow = shadowObject.AddComponent<UText>();
                text.Shadow.font = RDString.GetFontDataForLanguage(RDString.language).font;
                text.Shadow.color = Color.black.WithAlpha(0.4f);
            }
            else if (text.Shadow != null)
            {
                UnityEngine.Object.Destroy(text.Shadow.gameObject);
                text.Shadow = null;
            }
        }
    }
}
