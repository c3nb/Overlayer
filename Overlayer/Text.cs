using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TinyJson;
using UnityModManagerNet;
using System.IO;

namespace Overlayer
{
    public class Text
    {
        public class Setting
        {
            public Setting()
            {
                Position = new float[2] { 0.011628f, 1 };
                Color = new float[4] { 1, 1, 1, 1 };
                NotPlayingText = "Not Playing";
                PlayingText = @"엄격 : <b>{SHit}</b> | <color=#ED3E3E>{STE}</color> <color=#EB9A46>{SVE}</color> <color=#E3E370>{SEP}</color> <color=#86E370>{SP}</color> <color=#E3E370>{SLP}</color> <color=#EB9A46>{SVL}</color> <color=#ED3E3E>{STL}</color>
보통 : <b>{NHit}</b> | <color=#ED3E3E>{NTE}</color> <color=#EB9A46>{NVE}</color> <color=#E3E370>{NEP}</color> <color=#86E370>{NP}</color> <color=#E3E370>{NLP}</color> <color=#EB9A46>{NVL}</color> <color=#ED3E3E>{NTL}</color>
느슨 : <b>{LHit}</b> | <color=#ED3E3E>{LTE}</color> <color=#EB9A46>{LVE}</color> <color=#E3E370>{LEP}</color> <color=#86E370>{LP}</color> <color=#E3E370>{LLP}</color> <color=#EB9A46>{LVL}</color> <color=#ED3E3E>{LTL}</color>";
                FontSize = 44;
                IsExpanded = true;
            }
            public float[] Position;
            public float[] Color;
            public int FontSize;
            public string NotPlayingText;
            public string PlayingText;
            public bool IsExpanded;
        }
        public static bool IsPlaying => scrController.instance?.gameworld ?? false || ADOBase.sceneName.Contains("-X");
        public static List<Text> Texts = new List<Text>();
        public static readonly string JsonPath = Path.Combine("Mods", "Overlayer", "Texts.json");
        public static void Load()
        {
            if (File.Exists(JsonPath))
            {
                List<Setting> settings = File.ReadAllText(JsonPath).FromJson<List<Setting>>();
                for (int i = 0; i < settings.Count; i++)
                    new Text(settings[i]).Apply();
                Order();
            }
        }
        public static void Save()
        {
            List<Setting> settings = new List<Setting>();
            foreach (Text text in Texts)
                settings.Add(text.TSetting);
            File.WriteAllText(JsonPath, settings.ToJson());
        }
        public static void DestroyAll()
        {
            for (int i = 0; i < Texts.Count; i++)
                Texts[i].Destroy();
        }
        public static void Order() => Texts = new List<Text>(Texts.OrderBy(t => t.SText.Number));
        public Text(Setting setting = null)
        {
            SText = ShadowText.NewText();
            UnityEngine.Object.DontDestroyOnLoad(SText.gameObject);
            TSetting = setting ?? new Setting();
            SText.Updater = () =>
            {
                if (IsPlaying)
                    SText.Text = Tag.Replace(TSetting.PlayingText);
                else SText.Text = TSetting.NotPlayingText;
            };
            Number = SText.Number;
            Texts.Add(this);
        }
        public void GUI()
        {
            if (TSetting.IsExpanded = GUILayout.Toggle(TSetting.IsExpanded, $"Text {Number}"))
            {
                Utils.IndentGUI(() =>
                {
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
                    if (Number != 1 && GUILayout.Button("Destroy"))
                        Destroy();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                });
            }
        }
        public void Destroy()
        {
            ShadowText.Destroy(SText);
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
        public Text Apply()
        {
            SText.Color = new Color(TSetting.Color[0], TSetting.Color[1], TSetting.Color[2], TSetting.Color[3]);
            Vector2 pos = new Vector2(TSetting.Position[0], TSetting.Position[1]);
            SText.Center = pos;
            SText.Position = pos;
            SText.FontSize = TSetting.FontSize;
            return this;
        }
        public readonly ShadowText SText;
        public readonly Setting TSetting;
        public int Number;
    }
}
