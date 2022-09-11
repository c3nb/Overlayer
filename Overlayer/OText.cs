using TagLib.Tags;
using TagLib.Utils;
using TagLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TinyJson;
using TMPro;
using UnityEngine;
using UnityModManagerNet;

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
                ShadowColor = new float[4] { 0, 0, 0, 0.5f };
                NotPlayingText = "Not Playing";
                PlayingText = "<color=#{FMHex}>{MissCount}</color> <color=#{TEHex}>{CurTE}</color> <color=#{VEHex}>{CurVE}</color> <color=#{EPHex}>{CurEP}</color> <color=#{PHex}>{CurP}</color> <color=#{LPHex}>{CurLP}</color> <color=#{VLHex}>{CurVL}</color> <color=#{TLHex}>{CurTL}</color> <color=#{FOHex}>{Overloads}</color>";
                FontSize = 44;
                IsExpanded = true;
                Alignment = TextAlignmentOptions.Left;
                Font = "Default";
                Active = true;
                GradientText = false;
                Gradient = new float[4][] { new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 } };
            }
            public string Name;
            public float[] Position;
            public float[] Color;
            public int FontSize;
            public string NotPlayingText;
            public string PlayingText;
            public bool IsExpanded;
            public float[] ShadowColor;
            public string Font;
            public bool Active;
            public TextAlignmentOptions Alignment;
            public bool GradientText;
            public float[][] Gradient;
            public void ValidCheck()
            {
                if (Position == null)
                    Position = new float[2] { 0.011628f, 1 };
                if (Color == null)
                    Color = new float[4] { 1, 1, 1, 1 };
                if (ShadowColor == null)
                    ShadowColor = new float[4] { 0, 0, 0, 0.5f };
                if (NotPlayingText == null)
                    NotPlayingText = "Not Playing";
                if (PlayingText == null)
                    PlayingText = "<color=#ED3E3E>{CurTE}</color> <color=#EB9A46>{CurVE}</color> <color=#E3E370>{CurEP}</color> <color=#86E370>{CurP}</color> <color=#E3E370>{CurLP}</color> <color=#EB9A46>{CurVL}</color> <color=#ED3E3E>{CurTL}</color>";
                if (FontSize == 0)
                    FontSize = 44;
                if (Font == null)
                    Font = "Default";
                if (Gradient == null)
                    Gradient = new float[4][] { new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 } };
            }
        }
        public static void Clear()
        {
            foreach (var text in Texts)
            {
                text.PlayingCompiler = null;
                text.NotPlayingCompiler = null;
                UnityEngine.Object.Destroy(text.SText.Main.gameObject);
                UnityEngine.Object.Destroy(text.SText.Shadow.gameObject);
            }
            UnityEngine.Object.Destroy(ShadowText.PublicCanvas);
            ShadowText.Count = 0;
            Texts.Clear();
        }
        public static bool IsPlaying
        {
            get
            {
                var ctrl = scrController.instance;
                var cdt = scrConductor.instance;
                if (ctrl && cdt)
                    return !ctrl.paused && cdt.isGameWorld;
                return false;
            }
        }
        public static List<OText> Texts = new List<OText>();
        public static readonly string JsonPath = Path.Combine("Mods", "Overlayer", "Texts.json");
        public static void Load()
        {
            if (File.Exists(JsonPath))
            {
                List<Setting> settings = File.ReadAllText(JsonPath).FromJson<List<Setting>>();
                for (int i = 0; i < settings.Count; i++)
                    _ = new OText(settings[i]).Apply();
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
        public static void Order() => Texts = new List<OText>(Texts.OrderBy(t => t.SText.Number));
        public OText(Setting setting = null)
        {
            SText = ShadowText.NewText();
            UnityEngine.Object.DontDestroyOnLoad(SText.gameObject);
            TSetting = setting ?? new Setting();
            Number = SText.Number;
            TSetting.ValidCheck();
            if (TSetting.Name == null)
                TSetting.Name = $"Text {Number}";
            PlayingCompiler = new TextCompiler(TSetting.PlayingText, TagManager.AllTags);
            NotPlayingCompiler = new TextCompiler(TSetting.NotPlayingText, TagManager.NotPlayingTags);
            BrokenPlayingCompiler = new TextCompiler(TSetting.PlayingText.BreakRichTag(), TagManager.AllTags);
            BrokenNotPlayingCompiler = new TextCompiler(TSetting.NotPlayingText.BreakRichTag(), TagManager.NotPlayingTags);
            SText.Updater = () =>
            {
                if (IsPlaying)
                {
                    SText.Main.text = PlayingCompiler.Result;
                    SText.Shadow.text = BrokenPlayingCompiler.Result;
                }
                else
                {
                    SText.Main.text = NotPlayingCompiler.Result;
                    SText.Shadow.text = BrokenNotPlayingCompiler.Result;
                }
            };
            Texts.Add(this);
            SText.gameObject.SetActive(TSetting.Active);
        }
        public void GUI()
        {
            GUILayout.BeginHorizontal();
            TSetting.IsExpanded = GUILayout.Toggle(TSetting.IsExpanded, "");
            if (TSetting.IsExpanded)
                TSetting.Name = GUILayout.TextField(TSetting.Name);
            else GUILayout.Label(TSetting.Name);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (TSetting.IsExpanded)
            {
                GUILayout.BeginVertical();
                GUIUtils.IndentGUI(() =>
                {
                    var active = GUILayout.Toggle(TSetting.Active, "Active");
                    if (active != TSetting.Active)
                        SText.Active = TSetting.Active = active;
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
                    if (GUIUtils.DrawEnum($"Text {Number} Alignment", ref TSetting.Alignment)) Apply();
                    if (GUILayout.Button("Reset"))
                    {
                        TSetting.Alignment = TextAlignmentOptions.Left;
                        Apply();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (UnityModManager.UI.DrawIntField(ref TSetting.FontSize, "Text Size")) Apply();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUIUtils.DrawTextField(ref TSetting.Font, "Text Font")) Apply();
                    if (GUILayout.Button("Log Font List"))
                        for (int i = 0; i < ShadowText.fontNames.Length; i++)
                            Main.Logger.Log($"{i + 1}. {ShadowText.fontNames[i]}");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Text Color");
                    GUILayout.Space(1);
                    if (GUIUtils.DrawColor(ref TSetting.Color)) Apply();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Shadow Color");
                    GUILayout.Space(1);
                    if (GUIUtils.DrawColor(ref TSetting.ShadowColor)) Apply();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    bool newGradientText = GUILayout.Toggle(TSetting.GradientText, "Gradient");
                    if (newGradientText)
                    {
                        GUIUtils.IndentGUI(() =>
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Top Left");
                            GUILayout.Space(1);
                            if (GUIUtils.DrawColor(ref TSetting.Gradient[0])) Apply();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Top Right");
                            GUILayout.Space(1);
                            if (GUIUtils.DrawColor(ref TSetting.Gradient[1])) Apply();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Bottom Left");
                            GUILayout.Space(1);
                            if (GUIUtils.DrawColor(ref TSetting.Gradient[2])) Apply();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Bottom Right");
                            GUILayout.Space(1);
                            if (GUIUtils.DrawColor(ref TSetting.Gradient[3])) Apply();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        });
                    }
                    if (newGradientText != TSetting.GradientText)
                    {
                        TSetting.GradientText = newGradientText;
                        Apply();
                    }
                    GUILayout.BeginHorizontal();
                    if (GUIUtils.DrawTextArea(ref TSetting.PlayingText, "Text")) Apply();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUIUtils.DrawTextArea(ref TSetting.NotPlayingText, "Text Displayed When Not Playing")) Apply();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Refresh")) Apply();
                    if (GUILayout.Button("Reset"))
                    {
                        TSetting = new Setting();
                        Apply();
                    }
                    if (ShadowText.Count > 1 && GUILayout.Button("Destroy"))
                    {
                        UnityEngine.Object.Destroy(SText.gameObject);
                        Remove(this);
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                });
                GUILayout.EndVertical();
            }
        }
        public static void Remove(OText text)
        {
            int index = Texts.IndexOf(text);
            Texts.RemoveAt(index);
            UnityEngine.Object.Destroy(text.SText.Main.gameObject);
            UnityEngine.Object.Destroy(text.SText.Shadow.gameObject);
            for (int i = index; i < Texts.Count; i++)
            {
                var txt = Texts[i];
                txt.Number--;
                txt.SText.Number--;
            }
            ShadowText.Count--;
            GC.SuppressFinalize(text);
        }
        public OText Apply()
        {
            SText.TrySetFont(TSetting.Font);
            if (TSetting.GradientText)
            {
                Color col1 = new Color(TSetting.Gradient[0][0], TSetting.Gradient[0][1], TSetting.Gradient[0][2], TSetting.Gradient[0][3]);
                Color col2 = new Color(TSetting.Gradient[1][0], TSetting.Gradient[1][1], TSetting.Gradient[1][2], TSetting.Gradient[1][3]);
                Color col3 = new Color(TSetting.Gradient[2][0], TSetting.Gradient[2][1], TSetting.Gradient[2][2], TSetting.Gradient[2][3]);
                Color col4 = new Color(TSetting.Gradient[3][0], TSetting.Gradient[3][1], TSetting.Gradient[3][2], TSetting.Gradient[3][3]);
                SText.Main.colorGradient = new VertexGradient(col1, col2, col3, col4);
            }
            else SText.Main.colorGradient = new VertexGradient(new Color(TSetting.Color[0], TSetting.Color[1], TSetting.Color[2], TSetting.Color[3]));
            Vector2 pos = new Vector2(TSetting.Position[0], TSetting.Position[1]);
            SText.Center = pos;
            SText.Position = pos;
            SText.FontSize = TSetting.FontSize;
            SText.Alignment = TSetting.Alignment;
            SText.Shadow.color = TSetting.ShadowColor.ToColor();
            Tags.Global.ProgressDeath.Reset();
            PlayingCompiler.Compile(TSetting.PlayingText);
            NotPlayingCompiler.Compile(TSetting.NotPlayingText);
            BrokenPlayingCompiler.Compile(TSetting.PlayingText.BreakRichTag());
            BrokenNotPlayingCompiler.Compile(TSetting.NotPlayingText.BreakRichTag());
            return this;
        }
        public TextCompiler PlayingCompiler;
        public TextCompiler NotPlayingCompiler;
        public TextCompiler BrokenPlayingCompiler;
        public TextCompiler BrokenNotPlayingCompiler;
        public readonly ShadowText SText;
        public Setting TSetting;
        public int Number;
    }
}
