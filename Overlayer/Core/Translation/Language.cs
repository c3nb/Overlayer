using GDMiniJSON;
using JSON;
using Overlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Overlayer.Core.Translation
{
    public class Language
    {
        const string update = "Update!Update!Update!";
        private static Language Korean;
        private static Language English;
        public bool Initialized = false;
        public OverlayerLanguage Lang;
        private bool updateMode = false;
        private Dictionary<string, string> pairs = new Dictionary<string, string>();
        public static event Action OnInitialize = delegate { };
        Language(OverlayerLanguage lang)
        {
            Lang = lang;
            Download();
        }
        async void Download()
        {
            if (Initialized) return;
            string json;
            try 
            { 
                json = await OverlayerWebAPI.GetLanguageJson(Lang);
                Main.Logger.Log($"Received Language Json From Server ({Lang})");
            }
            catch 
            {
                try 
                { 
                    json = File.ReadAllText(Path.Combine(Main.Mod.Path, $"{Lang}.json"));
                    Main.Logger.Log($"Resolved Language Json From Local File ({Lang})");
                }
                catch { json = "{}"; Main.Logger.Log($"<color=#ff0000>C</color><color=#ff0e00>a</color><color=#ff1c00>n</color><color=#ff2a00>n</color><color=#ff3800>o</color><color=#ff4700>t</color><color=#ff5500> </color><color=#ff6300>R</color><color=#ff7100>e</color><color=#ff7f00>s</color><color=#ff8d00>o</color><color=#ff9b00>l</color><color=#ffaa00>v</color><color=#ffb800>e</color><color=#ffc600> </color><color=#ffd400>L</color><color=#ffe300>a</color><color=#fff100>n</color><color=#ffff00>g</color><color=#e3ff00>u</color><color=#c6ff00>a</color><color=#aaff00>g</color><color=#8eff00>e</color><color=#71ff00> </color><color=#55ff00>J</color><color=#39ff00>s</color><color=#1cff00>o</color><color=#00ff00>n</color><color=#00ff1c> </color><color=#00ff39>F</color><color=#00ff55>r</color><color=#00ff71>o</color><color=#00ff8e>m</color><color=#00ffaa> </color><color=#00ffc6>S</color><color=#00ffe3>e</color><color=#00ffff>r</color><color=#00e3ff>v</color><color=#00c6ff>e</color><color=#00aaff>r</color><color=#008eff> </color><color=#0071ff>O</color><color=#0055ff>r</color><color=#0039ff> </color><color=#001cff>L</color><color=#0000ff>o</color><color=#0f00ff>c</color><color=#1f00ff>a</color><color=#2e00ff>l</color><color=#3e00ff> </color><color=#4d00ff>F</color><color=#5d00ff>i</color><color=#6c00ff>l</color><color=#7c00ff>e</color><color=#8b00ff>!</color> ({Lang})"); }
            }
            JsonNode node = JsonNode.Parse(json);
            foreach (var pair in node.KeyValues)
                pairs.Add(pair.Key, pair.Value);
            await Task.Delay(500);
            OnInitialize();
            Initialized = true;
            if (updateMode) ActivateUpdateMode();
        }
        public string this[string key]
        {
            get => updateMode ? update : pairs.TryGetValue(key, out var value) ? value : key;
            set => pairs[key] = updateMode ? update : value;
        }
        public void ActivateUpdateMode()
        {
            updateMode = true;
            if (Initialized)
            {
                foreach (var key in pairs.Keys.ToList())
                    pairs[key] = update;
            }
        }
        public static Language GetLangauge(OverlayerLanguage lang)
        {
            switch (lang)
            {
                case OverlayerLanguage.Korean:
                    return Korean ??= new Language(OverlayerLanguage.Korean);
                case OverlayerLanguage.English:
                    return English ??= new Language(OverlayerLanguage.English);
                default: return null;
            }
        }
    }
}
