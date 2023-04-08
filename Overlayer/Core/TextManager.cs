using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Overlayer.Core.Translation;
using TinyJson;
using UnityEngine;

namespace Overlayer.Core
{
    public static class TextManager
    {
        static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
        };
        public static List<OverlayerText> Texts = new List<OverlayerText>();
        public static void Load()
        {
            string path = Path.Combine(Main.Mod.Path, "Texts.json");
            if (!File.Exists(path)) return;
            List<TextConfig> configs = JsonConvert.DeserializeObject<List<TextConfig>>(File.ReadAllText(path), settings);
            foreach (var config in configs)
                CreateText(config);
        }
        public static void Save()
        {
            string path = Path.Combine(Main.Mod.Path, "Texts.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(Texts.Select(t => t.config).ToList(), settings));
        }
        public static void CreateText(TextConfig config = null)
        {
            Texts.Add(new OverlayerText(config));
            Refresh();
        }
        public static void RemoveText(OverlayerText text)
        {
            text.Text.Destroy();
            UnityEngine.Object.Destroy(text.Text);
            ShadowText.TotalCount--;
            Texts.Remove(text);
            Refresh();
        }
        public static void Release()
        {
            Texts.ForEach(RemoveText);
            UnityEngine.Object.Destroy(ShadowText.PCanvasObj);
            ShadowText.PublicCanvas = null;
        }
        public static void Refresh()
        {
            Texts.ForEach(t => t.Apply());
            Texts = Texts.OrderBy(x => x.config.Name).ToList();
        }
        public static void GUI()
        {
            for (int i = 0; i < Texts.Count; i++)
                Texts[i].GUI();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Main.Language[TranslationKeys.AddText]))
                CreateText();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
