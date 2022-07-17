using Overlayer.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable

namespace Overlayer
{
    public class ShadowText : MonoBehaviour
    {
        public static readonly GameObject tmp;
        static ShadowText()
        {
            AssetBundle bundle = AssetBundle.LoadFromFile("Mods/Overlayer/text");
            tmp = bundle.LoadAsset<GameObject>("Assets/TMP.prefab");
        }
        public static int Count = 0;
        public static ShadowText NewText()
        {
            int count = ++Count;
            ShadowText st = new GameObject($"ShadowText_{count}").AddComponent<ShadowText>();
            st.Number = count;
            return st;
        }
        public static readonly Dictionary<string, TMP_FontAsset> Fonts = new Dictionary<string, TMP_FontAsset>();
        public Action Updater;
        public TextMeshProUGUI Main;
        public int Number;
        public TextAnchor Alignment
        {
            get => Main.alignment.ToAnchor();
            set => Main.alignment = value.ToAlignment();
        }
        public int FontSize
        {
            get => (int)Main.fontSize;
            set => Main.fontSize = value;
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
            }
        }
        public Vector2 Position
        {
            get => Main.rectTransform.anchoredPosition;
            set => Main.rectTransform.anchoredPosition = value;
        }
        private void Awake()
        {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            ContentSizeFitter fitter;
            GameObject mainObject = Instantiate(tmp);
            mainObject.transform.SetParent(transform);
            fitter = mainObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            Main = mainObject.GetComponent<TextMeshProUGUI>();
            Main.enableVertexGradient = true;
        }
        private void Update() => Updater();
    }
}
