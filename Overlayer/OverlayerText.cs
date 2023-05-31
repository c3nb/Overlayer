using Overlayer.Core;
using Overlayer.Core.Tags;
using Overlayer.Core.Translation;
using Overlayer.Core.Utils;
using TMPro;
using UnityEngine;
using UnityModManagerNet;

namespace Overlayer
{
    public class OverlayerText
    {
        public static bool IsPlaying
        {
            get
            {
                var ctrl = scrController.instance;
                var cdt = scrConductor.instance;
                if (ctrl != null && cdt != null)
                    return !ctrl.paused && cdt.isGameWorld;
                return false;
            }
        }
        public TextConfig config;
        public ShadowText Text { get; private set; }
        public Replacer PlayingText { get; private set; }
        public Replacer NotPlayingText { get; private set; }
        public Replacer ShadowPlayingText { get; private set; }
        public Replacer ShadowNotPlayingText { get; private set; }
        public OverlayerText(TextConfig cfg = null)
        {
            config = cfg ?? new TextConfig();
            if (string.IsNullOrEmpty(config.Name))
                config.Name = $"Text {ShadowText.TotalCount}";
            PlayingText = new Replacer(config.PlayingText, TagManager.All);
            NotPlayingText = new Replacer(config.NotPlayingText, TagManager.NP);
            ShadowPlayingText = new Replacer(config.PlayingText.BreakRichTag(), TagManager.All);
            ShadowNotPlayingText = new Replacer(config.NotPlayingText.BreakRichTag(), TagManager.NP);
            Text = ShadowText.NewText();
            Text.Updater = () => Update(false);
            Object.DontDestroyOnLoad(Text);
            Text.Init(config);
        }
        public void Update(bool force)
        {
            if (!force && config.DisableUpdate) return;
            if (IsPlaying)
            {
                Text.Main.text = PlayingText.Replace();
                Text.Shadow.text = ShadowPlayingText.Replace();
            }
            else
            {
                Text.Main.text = NotPlayingText.Replace();
                Text.Shadow.text = ShadowNotPlayingText.Replace();
            }
        }
        public void GUI()
        {
            GUILayout.BeginHorizontal();
            config.IsExpanded = GUILayout.Toggle(config.IsExpanded, "");
            if (config.IsExpanded)
                config.Name = GUILayout.TextField(config.Name);
            else GUILayout.Label(config.Name);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (config.IsExpanded)
            {
                GUIUtils.BeginIndent();
                var active = GUILayout.Toggle(config.Active, Main.Language[TranslationKeys.Active]);
                if (active != config.Active)
                    Text.Active = config.Active = active;
                config.DisableUpdate = GUILayout.Toggle(config.DisableUpdate, "Disable Update");
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                if (UnityModManager.UI.DrawFloatField(ref config.Position.x, Main.Language[TranslationKeys.TextXPos])) Apply();
                config.Position.x = GUILayout.HorizontalSlider(config.Position.x, 0, 1);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (UnityModManager.UI.DrawFloatField(ref config.Position.y, Main.Language[TranslationKeys.TextYPos])) Apply();
                config.Position.y = GUILayout.HorizontalSlider(config.Position.y, 0, 1);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label($"{Main.Language[TranslationKeys.TextAlignment]}:");
                if (GUIUtils.DrawEnum($"{config.Name} {Main.Language[TranslationKeys.Alignment]}", ref config.Alignment)) Apply();
                if (GUILayout.Button(Main.Language[TranslationKeys.Reset]))
                {
                    config.Alignment = TextAlignmentOptions.TopLeft;
                    Apply();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (UnityModManager.UI.DrawFloatField(ref config.FontSize, Main.Language[TranslationKeys.TextSize])) Apply();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUIUtils.DrawTextField(ref config.Font, Main.Language[TranslationKeys.TextFont])) Apply();
                if (GUILayout.Button(Main.Language[TranslationKeys.LogFontList]))
                    foreach (string font in FontManager.OSFonts)
                        Main.Logger.Log(font);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                bool newGradientText = GUILayout.Toggle(config.Gradient, Main.Language[TranslationKeys.Gradient]);
                if (newGradientText)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(Main.Language[TranslationKeys.TextColor]);
                    GUILayout.Space(1);
                    if (GUIUtils.DrawColor(ref config.TextColor_G)) Apply();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    GUILayout.Label(Main.Language[TranslationKeys.ShadowColor]);
                    GUILayout.Space(1);
                    if (GUIUtils.DrawColor(ref config.ShadowColor_G)) Apply();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                }
                else
                {
                    Color color;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(Main.Language[TranslationKeys.TextColor]);
                    GUILayout.Space(1);
                    color = config.TextColor_;
                    if (GUIUtils.DrawColor(ref color))
                    {
                        config.TextColor_ = color;
                        Apply();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(Main.Language[TranslationKeys.ShadowColor]);
                    GUILayout.Space(1);
                    color = config.ShadowColor_;
                    if (GUIUtils.DrawColor(ref color))
                    {
                        config.ShadowColor_ = color;
                        Apply();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                if (newGradientText != config.Gradient)
                {
                    config.Gradient = newGradientText;
                    Apply();
                }
                GUILayout.BeginHorizontal();
                if (GUIUtils.DrawTextArea(ref config.PlayingText, Main.Language[TranslationKeys.Text])) Apply();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUIUtils.DrawTextArea(ref config.NotPlayingText, Main.Language[TranslationKeys.TextDisplayedWhenNotPlaying])) Apply();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Main.Language[TranslationKeys.Refresh])) Apply();
                if (GUILayout.Button(Main.Language[TranslationKeys.Reset]))
                {
                    config = new TextConfig();
                    Apply();
                }
                if (TextManager.Texts.Count > 1 && GUILayout.Button(Main.Language[TranslationKeys.Destroy]))
                    TextManager.RemoveText(this);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUIUtils.EndIndent();
            }
        }
        public void Apply()
        {
            OverlayerDebug.Log($"Applying Config At \"{config.Name}\"..");
            PlayingText.Source = config.PlayingText;
            NotPlayingText.Source = config.NotPlayingText;
            ShadowPlayingText.Source = config.PlayingText.BreakRichTag();
            ShadowNotPlayingText.Source = config.NotPlayingText.BreakRichTag();
            OverlayerDebug.Log($"Compiling Text At \"{config.Name}\"..");
            PlayingText.Compile();
            NotPlayingText.Compile();
            ShadowPlayingText.Compile();
            ShadowNotPlayingText.Compile();
            OverlayerDebug.Log($"Updating Tag Reference From Text \"{config.Name}\"..");
            TagManager.UpdateReference();
            PlayingText.SetReference(TagManager.All);
            NotPlayingText.SetReference(TagManager.NP);
            ShadowPlayingText.SetReference(TagManager.All);
            ShadowNotPlayingText.SetReference(TagManager.NP);
            OverlayerDebug.Log($"Setting Text Option At Text \"{config.Name}\"..");
            Text.Main.lineSpacing = config.LineSpacing;
            Text.Shadow.lineSpacing = config.LineSpacing;
            Text.Main.lineSpacingAdjustment = config.LineSpacingAdjustment;
            Text.Shadow.lineSpacingAdjustment = config.LineSpacingAdjustment;
            Text.Main.colorGradient = config.TextColor_G;
            Text.Shadow.colorGradient = config.ShadowColor_G;
            Text.Center = Text.Position = config.Position;
            Text.FontSize = config.FontSize;
            Text.Alignment = config.Alignment;
            Text.TrySetFont(config.Font);
        }
    }
}
