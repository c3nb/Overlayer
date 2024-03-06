using JSON;
using Overlayer.Core.Translation;
using Overlayer.Models;
using Overlayer.Unity;
using Overlayer.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Overlayer.Core
{
    public static class TextManager
    {
        public static bool Initialized { get; private set; }
        public static int Count => Texts.Count;
        private static List<OverlayerText> Texts;
        public static void Initialize()
        {
            if (Initialized) return;
            Texts = new List<OverlayerText>();
            string textsPath = Path.Combine(Main.Mod.Path, "Texts.json");
            List<TextConfig> configs = new List<TextConfig>();
            if (File.Exists(textsPath))
                configs = ModelUtils.UnwrapList<TextConfig>((JsonArray)JsonNode.Parse(File.ReadAllText(textsPath)));
            configs.ForEach(c => CreateText(c));
            Refresh();
            Initialized = true;
        }
        public static OverlayerText CreateText(TextConfig config)
        {
            if (string.IsNullOrEmpty(config.Name))
                config.Name = string.Format(Main.Lang[TranslationKeys.Misc.Text], Count + 1);
            GameObject go = new GameObject($"OverlayerText_{config.Name}");
            var text = go.AddComponent<OverlayerText>();
            text.Init(config);
            Texts.Add(text);
            return text;
        }
        public static OverlayerText Get(int index) => Texts[index];
        public static OverlayerText Find(TextConfig configRef) => Texts.Find(ot => ReferenceEquals(ot.Config, configRef));
        public static void Remove(int index) => DestroyText(Texts[index]);
        public static void DestroyText(OverlayerText text)
        {
            UnityEngine.Object.Destroy(text.gameObject);
            Texts.Remove(text);
            Refresh();
        }
        public static void Save()
        {
            var array = ModelUtils.WrapList(Texts.Select(ot => ot.Config).ToList());
            string textsPath = Path.Combine(Main.Mod.Path, "Texts.json");
            File.WriteAllText(textsPath, array.ToString(4));
        }
        public static void Refresh()
        {
            Texts.ForEach(ot => ot.ApplyConfig());
        }
        public static void Release()
        {
            if (!Initialized) return;
            Save();
            Texts = null;
            UnityEngine.Object.Destroy(OverlayerText.PCanvasObj);
            Initialized = false;
        }
    }
}
