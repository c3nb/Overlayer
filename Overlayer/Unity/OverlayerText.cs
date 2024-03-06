using Overlayer.Core;
using Overlayer.Core.TextReplacing;
using Overlayer.Models;
using Overlayer.Tags;
using Overlayer.Utils;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Overlayer.Unity
{
    public class OverlayerText : MonoBehaviour
    {
        public bool Initialized { get; private set; }
        public TextConfig Config;
        public Replacer PlayingReplacer;
        public Replacer NotPlayingReplacer;
        public TextMeshProUGUI Text;
        #region Statics
        public static GameObject PCanvasObj;
        public static Canvas PublicCanvas;
        public static readonly Shader sr_msdf = (Shader)typeof(ShaderUtilities).GetProperty("ShaderRef_MobileSDF", (BindingFlags)15420).GetValue(null);
        #endregion
        public void Init(TextConfig config)
        {
            if (Initialized) return;
            Config = config;
            if (string.IsNullOrEmpty(config.Name))
                config.Name = $"Text {TextManager.Count + 1}";
            PlayingReplacer = new Replacer(config.PlayingText, TagManager.All.Select(ot => ot.Tag));
            NotPlayingReplacer = new Replacer(config.NotPlayingText, TagManager.NP.Select(ot => ot.Tag));
            DontDestroyOnLoad(gameObject);
            if (!PublicCanvas)
            {
                GameObject pCanvasObj = PCanvasObj = new GameObject("Overlayer Canvas");
                PublicCanvas = pCanvasObj.AddComponent<Canvas>();
                PublicCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                PublicCanvas.sortingOrder = int.MaxValue;
                CanvasScaler scaler = pCanvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                var currentRes = Screen.currentResolution;
                scaler.referenceResolution = new Vector2(currentRes.width, currentRes.height);
                DontDestroyOnLoad(PublicCanvas);
            }
            var font = FontManager.GetFont(config.Font);
            GameObject mainObject = gameObject;
            mainObject.transform.SetParent(PublicCanvas.transform);
            mainObject.MakeFlexible();
            Text = mainObject.AddComponent<TextMeshProUGUI>();
            Text.font = font.fontTMP;
            Text.enableVertexGradient = true;
            Text.color = Color.white;
            Text.colorGradient = config.TextColor;
            var rt = Text.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            Text.enableAutoSizing = false;
            Text.lineSpacing = config.LineSpacing;
            Text.lineSpacingAdjustment = config.LineSpacingAdj;
            rt.eulerAngles = config.Rotation;
            var mainMat = new Material(Text.fontSharedMaterial);
            if (sr_msdf) mainMat.shader = sr_msdf;
            mainMat.EnableKeyword(ShaderUtilities.Keyword_Outline);
            mainMat.SetColor(ShaderUtilities.ID_OutlineColor, config.OutlineColor);
            mainMat.SetFloat(ShaderUtilities.ID_OutlineWidth, config.OutlineWidth);
            mainMat.EnableKeyword(ShaderUtilities.Keyword_Underlay);
            mainMat.SetColor(ShaderUtilities.ID_UnderlayColor, config.ShadowColor);
            mainMat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, .5f);
            mainMat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -.5f);
            mainMat.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0);
            mainMat.SetFloat(ShaderUtilities.ID_UnderlaySoftness, .5f);
            Text.fontSharedMaterial = mainMat;
            Text.gameObject.SetActive(config.Active);
            Initialized = true;
        }
        public void Update()
        {
            if (Main.IsPlaying) Text.text = PlayingReplacer.Replace();
            else Text.text = NotPlayingReplacer.Replace();
        }
        public void ApplyConfig()
        {
            PlayingReplacer.Source = Config.PlayingText;
            NotPlayingReplacer.Source = Config.NotPlayingText;
            PlayingReplacer.UpdateTags(TagManager.All.Select(ot => ot.Tag));
            NotPlayingReplacer.UpdateTags(TagManager.NP.Select(ot => ot.Tag));
            PlayingReplacer.Compile();
            NotPlayingReplacer.Compile();
            TagManager.UpdatePatch();
            Text.lineSpacing = Config.LineSpacing;
            Text.lineSpacingAdjustment = Config.LineSpacingAdj;
            Text.colorGradient = Config.TextColor;
            Text.rectTransform.pivot = Config.Pivot;
            Text.rectTransform.anchoredPosition = (Config.Position - new Vector2(0.5f, 0.5f)) * new Vector2(Screen.width, Screen.height);
            Text.rectTransform.eulerAngles = Config.Rotation;
            Text.fontSize = Config.FontSize;
            Text.alignment = Config.Alignment;
            var mainMat = new Material(Text.fontSharedMaterial);
            mainMat.SetColor(ShaderUtilities.ID_OutlineColor, Config.OutlineColor);
            mainMat.SetFloat(ShaderUtilities.ID_OutlineWidth, Config.OutlineWidth);
            mainMat.SetColor(ShaderUtilities.ID_UnderlayColor, Config.ShadowColor);
            Text.fontSharedMaterial = mainMat;
            if (FontManager.TryGetFont(Config.Font, out FontData font))
                Text.font = font.fontTMP;
        }
    }
}
