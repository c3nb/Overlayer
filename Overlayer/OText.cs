using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TinyJson;
using UnityModManagerNet;
using System.IO;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace Overlayer
{
    public class OText
    {
        public class Setting
        {
            public Setting() => InitValues();
            public void InitValues()
            {
                Position = new float[2] { 0.011628f, 1 };
                Color = new float[4] { 1, 1, 1, 1 };
                NotPlayingText = "Not Playing";
                PlayingText = "<color=#ED3E3E>{CurTE}</color> <color=#EB9A46>{CurVE}</color> <color=#E3E370>{CurEP}</color> <color=#86E370>{CurP}</color> <color=#E3E370>{CurLP}</color> <color=#EB9A46>{CurVL}</color> <color=#ED3E3E>{CurTL}</color>";
                FontSize = 44;
                IsExpanded = true;
                Alignment = TextAnchor.LowerLeft;
                Font = "Default";
                Active = true;
            }
            public float[] Position;
            public float[] Color;
            public int FontSize;
            public string NotPlayingText;
            public string PlayingText;
            public bool IsExpanded;
            public string Font;
            public bool Active;
            public TextAnchor Alignment;
            public void ValidCheck()
            {
                if (Position == null)
                    Position = new float[2] { 0.011628f, 1 };
                if (Color == null)
                    Color = new float[4] { 1, 1, 1, 1 };
                if (NotPlayingText == null)
                    NotPlayingText = "Not Playing";
                if (PlayingText == null)
                    PlayingText = "<color=#ED3E3E>{CurTE}</color> <color=#EB9A46>{CurVE}</color> <color=#E3E370>{CurEP}</color> <color=#86E370>{CurP}</color> <color=#E3E370>{CurLP}</color> <color=#EB9A46>{CurVL}</color> <color=#ED3E3E>{CurTL}</color>";
                if (FontSize == 0)
                    FontSize = 44;
                if (Font == null)
                    Font = "Default";
            }
        }
        public static readonly Regex tagBreaker = new Regex("<(.|\n)*?>", RegexOptions.Compiled);
        public static bool IsPlaying => (!scrController.instance?.paused ?? false) && (scrConductor.instance?.isGameWorld ?? false);
        public static List<OText> Texts = new List<OText>();
        public static readonly string JsonPath = Path.Combine("Mods", "Overlayer", "Texts.json");
        public static void Load()
        {
            if (File.Exists(JsonPath))
            {
                List<Setting> settings = File.ReadAllText(JsonPath).FromJson<List<Setting>>();
                for (int i = 0; i < settings.Count; i++)
                    new OText(settings[i]).Apply();
                Order();
            }
        }
        public static void Save()
        {
            List<Setting> settings = new List<Setting>();
            foreach (OText text in Texts)
                settings.Add(text.TSetting);
            File.WriteAllText(JsonPath, settings.ToJson());
        }
        public static void DestroyAll()
        {
            for (int i = 0; i < Texts.Count; i++)
                Texts[i].Destroy();
        }
        public static void Order() => Texts = new List<OText>(Texts.OrderBy(t => t.SText.Number));
        public OText(Setting setting = null)
        {
            SText = ShadowText.NewText();
            UnityEngine.Object.DontDestroyOnLoad(SText.gameObject);
            TSetting = setting ?? new Setting();
            TSetting.ValidCheck();
            BrokenPlayingText = tagBreaker.Replace(TSetting.PlayingText, string.Empty);
            BrokenNotPlayingText = tagBreaker.Replace(TSetting.NotPlayingText, string.Empty);
            Number = SText.Number;
            PlayingCompiler = new TagCompiler(TSetting.PlayingText);
            NotPlayingCompiler = new TagCompiler(TSetting.NotPlayingText);
            BrokenPlayingCompiler = new TagCompiler(BrokenPlayingText);
            BrokenNotPlayingCompiler = new TagCompiler(BrokenNotPlayingText);
            SText.Updater = () =>
            {
                if (IsPlaying)
                {
                    SText.Main.text = PlayingCompiler.Value;
                    SText.Shadow.text = BrokenPlayingCompiler.Value;
                }
                else
                {
                    SText.Main.text = NotPlayingCompiler.Value;
                    SText.Shadow.text = BrokenNotPlayingCompiler.Value;
                }
            };
            Texts.Add(this);
        }
        public void GUI()
        {
            if (TSetting.IsExpanded = GUILayout.Toggle(TSetting.IsExpanded, $"Text {Number}"))
            {
                Utils.IndentGUI(() =>
                {
                    SText.gameObject.SetActive(TSetting.Active = GUILayout.Toggle(TSetting.Active, "Active"));
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    if (UnityModManager.UI.DrawFloatField(ref TSetting.Position[0], "Text X-Position")) Apply();
                    TSetting.Position[0] = GUILayout.HorizontalSlider(TSetting.Position[0], 0, 1);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (UnityModManager.UI.DrawFloatField(ref TSetting.Position[1], "Text Y-Position")) Apply();
                    TSetting.Position[1] = GUILayout.HorizontalSlider(TSetting.Position[1], 0, 1);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Text Alignment:");
                    if (Utils.DrawEnum($"Text {Number} Alignment", ref TSetting.Alignment)) Apply();
                    if (GUILayout.Button("Reset"))
                    {
                        TSetting.Alignment = TextAnchor.LowerLeft;
                        Apply();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (Utils.DrawTextField(ref TSetting.Font, "Font")) Apply();
                    if (GUILayout.Button("Reset"))
                    {
                        TSetting.Font = "Default";
                        SText.Font = RDString.GetFontDataForLanguage(RDString.language).font;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (UnityModManager.UI.DrawIntField(ref TSetting.FontSize, "Text Size")) Apply();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Text Color");
                    GUILayout.Space(1);
                    if (Utils.DrawColor(ref TSetting.Color)) Apply();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (Utils.DrawTextArea(ref TSetting.PlayingText, "Text")) Apply();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (Utils.DrawTextArea(ref TSetting.NotPlayingText, "Text Displayed When Not Playing")) Apply();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Reset"))
                    {
                        SText.Font = RDString.GetFontDataForLanguage(RDString.language).font;
                        TSetting = new Setting();
                        Apply();
                    }
                    if (Number != 1 && GUILayout.Button("Destroy"))
                        Destroy();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                });
            }
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(SText.gameObject);
            int index = Texts.IndexOf(this);
            Texts.RemoveAt(index);
            for (int i = index; i < Texts.Count; i++)
            {
                var text = Texts[i];
                text.Number--;
                text.SText.Number--;
            }
            ShadowText.Count--;
            GC.SuppressFinalize(this);
        }
        public OText Apply()
        {
            SText.Color = new Color(TSetting.Color[0], TSetting.Color[1], TSetting.Color[2], TSetting.Color[3]);
            Vector2 pos = new Vector2(TSetting.Position[0], TSetting.Position[1]);
            SText.Center = pos;
            SText.Position = pos;
            SText.FontSize = TSetting.FontSize;
            SText.Alignment = TSetting.Alignment;
            BrokenPlayingText = tagBreaker.Replace(TSetting.PlayingText, string.Empty);
            BrokenNotPlayingText = tagBreaker.Replace(TSetting.NotPlayingText, string.Empty);
            Utils.TrySetFont(TSetting.Font, f => SText.Font = f);
            PlayingCompiler.Compile(TSetting.PlayingText);
            NotPlayingCompiler.Compile(TSetting.NotPlayingText);
            BrokenPlayingCompiler.Compile(BrokenPlayingText);
            BrokenNotPlayingCompiler.Compile(BrokenNotPlayingText);
            return this;
        }
        public TagCompiler PlayingCompiler;
        public TagCompiler BrokenPlayingCompiler;
        public TagCompiler NotPlayingCompiler;
        public TagCompiler BrokenNotPlayingCompiler;
        public string BrokenPlayingText;
        public string BrokenNotPlayingText;
        public readonly ShadowText SText;
        public Setting TSetting;
        public int Number;
    }
}
