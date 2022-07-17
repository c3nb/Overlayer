using Overlayer.Tags;
using Overlayer.Utils;
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
                PlayingText = "<color=#ED3E3E>{CurTE}</color> <color=#EB9A46>{CurVE}</color> <color=#E3E370>{CurEP}</color> <color=#86E370>{CurP}</color> <color=#E3E370>{CurLP}</color> <color=#EB9A46>{CurVL}</color> <color=#ED3E3E>{CurTL}</color>";
                FontSize = 44;
                IsExpanded = true;
                Alignment = TextAnchor.LowerLeft;
                //Font = "Default";
                Active = true;
                GradientText = false;
                Gradient = new float[4][] { new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 } };
            }
            public float[] Position;
            public float[] Color;
            public int FontSize;
            public string NotPlayingText;
            public string PlayingText;
            public bool IsExpanded;
            public float[] ShadowColor;
            //public string Font;
            public bool Active;
            public TextAnchor Alignment;
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
                //if (Font == null)
                //    Font = "Default";
                if (Gradient == null)
                    Gradient = new float[4][] { new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 }, new float[4] { 1, 1, 1, 1 } };
            }
        }
        public static void Clear()
        {
            foreach (var text in Texts)
            {
                UnityEngine.Object.Destroy(text.SText.gameObject);
                text.PlayingCompiler = null;
                text.NotPlayingCompiler = null;
            }
            ShadowText.Count = 0;
            Texts.Clear();
        }
        public static readonly Regex tagBreaker = new Regex("<(.|\n)*?>", RegexOptions.Compiled);
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
            TSetting.ValidCheck();
            Number = SText.Number;
            PlayingCompiler = new TagCompiler(TSetting.PlayingText, Main.AllTags);
            NotPlayingCompiler = new TagCompiler(TSetting.NotPlayingText, Main.NotPlayingTags);
            SText.Updater = () =>
            {
                if (IsPlaying)
                    SText.Main.text = PlayingCompiler.Result;
                else SText.Main.text = NotPlayingCompiler.Result;
            };
            Texts.Add(this);
            SText.gameObject.SetActive(TSetting.Active);
        }
        public void GUI()
        {
            if (TSetting.IsExpanded = GUILayout.Toggle(TSetting.IsExpanded, $"Text {Number}"))
            {
                GUIUtils.IndentGUI(() =>
                {
                    SText.gameObject.SetActive(TSetting.Active = GUILayout.Toggle(TSetting.Active, "Active"));
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    if (UnityModManager.UI.DrawFloatField(ref TSetting.Position[0], "Text X-Position")) _ = Apply();
                    TSetting.Position[0] = GUILayout.HorizontalSlider(TSetting.Position[0], 0, 1);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (UnityModManager.UI.DrawFloatField(ref TSetting.Position[1], "Text Y-Position")) _ = Apply();
                    TSetting.Position[1] = GUILayout.HorizontalSlider(TSetting.Position[1], 0, 1);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Text Alignment:");
                    if (GUIUtils.DrawEnum($"Text {Number} Alignment", ref TSetting.Alignment)) _ = Apply();
                    if (GUILayout.Button("Reset"))
                    {
                        TSetting.Alignment = TextAnchor.LowerLeft;
                        _ = Apply();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (UnityModManager.UI.DrawIntField(ref TSetting.FontSize, "Text Size")) _ = Apply();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Text Color");
                    GUILayout.Space(1);
                    if (GUIUtils.DrawColor(ref TSetting.Color)) _ = Apply();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Shadow Color");
                    GUILayout.Space(1);
                    if (GUIUtils.DrawColor(ref TSetting.ShadowColor)) _ = Apply();
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
                            if (GUIUtils.DrawColor(ref TSetting.Gradient[0])) _ = Apply();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Top Right");
                            GUILayout.Space(1);
                            if (GUIUtils.DrawColor(ref TSetting.Gradient[1])) _ = Apply();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Bottom Left");
                            GUILayout.Space(1);
                            if (GUIUtils.DrawColor(ref TSetting.Gradient[2])) _ = Apply();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Bottom Right");
                            GUILayout.Space(1);
                            if (GUIUtils.DrawColor(ref TSetting.Gradient[3])) _ = Apply();
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        });
                    }
                    if (newGradientText != TSetting.GradientText)
                    {
                        TSetting.GradientText = newGradientText;
                        _ = Apply();
                    }
                    GUILayout.BeginHorizontal();
                    if (GUIUtils.DrawTextArea(ref TSetting.PlayingText, "Text")) _ = Apply();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUIUtils.DrawTextArea(ref TSetting.NotPlayingText, "Text Displayed When Not Playing")) _ = Apply();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Refresh")) _ = Apply();
                    if (GUILayout.Button("Reset"))
                    {
                        TSetting = new Setting();
                        _ = Apply();
                    }
                    if (Number != 1 && GUILayout.Button("Destroy"))
                    {
                        UnityEngine.Object.Destroy(SText.gameObject);
                        Remove(this);
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                });
            }
        }
        public static void Remove(OText text)
        {
            int index = Texts.IndexOf(text);
            Texts.RemoveAt(index);
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
            SText.Main.fontMaterial.SetColor("_UnderlayColor", new Color(TSetting.ShadowColor[0], TSetting.ShadowColor[1], TSetting.ShadowColor[2], TSetting.ShadowColor[3]));
            SText.Main.UpdateMeshPadding();
            PlayingCompiler.Compile(TSetting.PlayingText);
            NotPlayingCompiler.Compile(TSetting.NotPlayingText);
            return this;
        }
        public TagCompiler PlayingCompiler;
        public TagCompiler NotPlayingCompiler;
        public readonly ShadowText SText;
        public Setting TSetting;
        public int Number;
    }
}
