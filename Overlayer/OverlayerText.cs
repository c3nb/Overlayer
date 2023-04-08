using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Overlayer.Core;
using Overlayer.Core.Tags;
using Overlayer.Core.Translation;
using SA.GoogleDoc;
using TMPro;
using UnityEngine;
using static Community.CsharpSqlite.Sqlite3;
using UnityModManagerNet;

namespace Overlayer
{
    public class OverlayerText
    {
        public TextConfig config;
        public ShadowText Text { get; private set; }
        public Replacer PlayingText { get; private set; }
        public Replacer NotPlayingText { get; private set; }
        public Replacer ShadowPlayingText { get; private set; }
        public Replacer ShadowNotPlayingText { get; private set; }
        public OverlayerText(TextConfig config = null)
        {
            this.config = config ?? new TextConfig();
            PlayingText = new Replacer(config.PlayingText, TagManager.All);
            NotPlayingText = new Replacer(config.NotPlayingText, TagManager.NP);
            ShadowPlayingText = new Replacer(config.PlayingText, TagManager.All);
            ShadowNotPlayingText = new Replacer(config.NotPlayingText, TagManager.NP);
            Text = ShadowText.NewText();
            Text.Init(config);
        }
        public void SettingsUI()
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
                Core.Utils.BeginIndent();
                var active = GUILayout.Toggle(config.Active, Main.Language[TranslationKeys.Active]);
                if (active != config.Active)
                    Text.Active = config.Active = active;
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
                if (GUIUtils.DrawEnum($"{Main.Language[TranslationKeys.Text]} {Number} {Main.Language[TranslationKeys.Alignment]}", ref config.Alignment)) Apply();
                if (GUILayout.Button(Main.Language[TranslationKeys.Reset]))
                {
                    config.Alignment = TextAlignmentOptions.Left;
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

                GUILayout.BeginHorizontal();
                GUILayout.Label(Main.Language[TranslationKeys.TextColor]);
                GUILayout.Space(1);
                if (GUIUtils.DrawColor(ref config.Color)) Apply();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(Main.Language[TranslationKeys.ShadowColor]);
                GUILayout.Space(1);
                if (GUIUtils.DrawColor(ref config.ShadowColor)) Apply();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                bool newGradientText = GUILayout.Toggle(config.GradientText, Main.Language[TranslationKeys.Gradient]);
                if (newGradientText)
                {
                    GUIUtils.IndentGUI(() =>
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(Main.Language[TranslationKeys.TopLeft]);
                        GUILayout.Space(1);
                        if (GUIUtils.DrawColor(ref config.Gradient[0])) Apply();
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(Main.Language[TranslationKeys.TopRight]);
                        GUILayout.Space(1);
                        if (GUIUtils.DrawColor(ref config.Gradient[1])) Apply();
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(Main.Language[TranslationKeys.BottomLeft]);
                        GUILayout.Space(1);
                        if (GUIUtils.DrawColor(ref config.Gradient[2])) Apply();
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(Main.Language[TranslationKeys.BottomRight]);
                        GUILayout.Space(1);
                        if (GUIUtils.DrawColor(ref config.Gradient[3])) Apply();
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    });
                }
                if (newGradientText != config.GradientText)
                {
                    config.GradientText = newGradientText;
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
                    config = new Setting();
                    Apply();
                }
                if (GUILayout.Button(Main.Language[TranslationKeys.Destroy]))
                {

                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                Core.Utils.EndIndent();
            }
        }
        public void Apply()
        {
            PlayingText.Source = config.PlayingText;
            NotPlayingText.Source = config.NotPlayingText;
            ShadowPlayingText.Source = config.PlayingText;
            ShadowNotPlayingText.Source = config.NotPlayingText;
            PlayingText.SetReference(TagManager.All);
            NotPlayingText.SetReference(TagManager.NP);
            ShadowPlayingText.SetReference(TagManager.All);
            ShadowNotPlayingText.SetReference(TagManager.NP);
            Text.Main.lineSpacing = config.LineSpacing;
            Text.Shadow.lineSpacing = config.LineSpacing;
            Text.Main.colorGradient = config.TextColor;
            Text.Shadow.colorGradient = config.ShadowColor;
            Text.Center = Text.Position = config.Position;
            Text.FontSize = config.FontSize;
            Text.Alignment = config.Alignment;
        }
    }
}
