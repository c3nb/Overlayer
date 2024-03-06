using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace Overlayer.Core
{
    public static class FontManager
    {
        static TMP_FontAsset DefaultTMPFont;
        static Font DefaultFont;
        static bool initialized;
        static FontData defaultFont;
        static Dictionary<string, FontData> Fonts = new Dictionary<string, FontData>();
        public static bool Initialized => initialized;
        public static string[] OSFonts { get; private set; }
        public static string[] OSFontPaths { get; private set; }
        public static ReadOnlyCollection<FontData> FallbackFontDatas { get; private set; }
        public static ReadOnlyCollection<Font> FallbackFonts { get; private set; }
        public static ReadOnlyCollection<TMP_FontAsset> FallbackTMPFonts { get; private set; }
        public static FontData GetFont(string name) => TryGetFont(name, out FontData font) ? font : defaultFont;
        public static void SetFont(string name, FontData font) => Fonts[name] = font;
        public static bool TryGetFont(string name, out FontData font)
        {
            if (string.IsNullOrEmpty(name))
            {
                font = defaultFont;
                return false;
            }
            if (name == "Default")
            {
                font = defaultFont;
                return true;
            }
            name = name.Replace("{ModDir}", Main.Mod.Path);
            if (Fonts.TryGetValue(name, out FontData data))
            {
                font = data;
                return true;
            }
            else
            {
                if (File.Exists(name))
                {
                    FontData newData = defaultFont;
                    Font newFont = new Font(name);
                    TMP_FontAsset newTMPFont = TMP_FontAsset.CreateFontAsset(newFont);
                    if (newTMPFont)
                        newTMPFont.fallbackFontAssetTable = FallbackTMPFonts.ToList();
                    newData.font = newFont;
                    newData.fontTMP = newTMPFont ?? defaultFont.fontTMP;
                    Fonts.Add(name, newData);
                    font = newData;
                    return true;
                }
                else
                {
                    int index = Array.IndexOf(OSFonts, name);
                    if (index != -1)
                    {
                        FontData newData = defaultFont;
                        Font newFont = Font.CreateDynamicFontFromOSFont(name, defaultFont.font.fontSize);
                        TMP_FontAsset newTMPFont = TMP_FontAsset.CreateFontAsset(new Font(OSFontPaths[index]));
                        if (newTMPFont)
                            newTMPFont.fallbackFontAssetTable = FallbackTMPFonts.ToList();
                        newData.font = newFont;
                        newData.fontTMP = newTMPFont ?? defaultFont.fontTMP;
                        Fonts.Add(name, newData);
                        font = newData;
                        return true;
                    }
                }
                font = defaultFont;
                return false;
            }
        }
        public static void Initialize()
        {
            if (!initialized)
            {
                DefaultFont = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
                DefaultTMPFont = TMP_FontAsset.CreateFontAsset(DefaultFont, 100, 10, GlyphRenderMode.SDFAA, 1024, 1024);
                FallbackFontDatas = RDString.AvailableLanguages.Select(RDString.GetFontDataForLanguage).ToList().AsReadOnly();
                FallbackFonts = FallbackFontDatas.Select(f => f.font).ToList().AsReadOnly();
                FallbackTMPFonts = FallbackFontDatas.Select(f => f.fontTMP).ToList().AsReadOnly();
                DefaultTMPFont.fallbackFontAssetTable = FallbackTMPFonts.ToList();
                defaultFont = RDString.fontData;
                defaultFont.lineSpacing = 1f;
                defaultFont.lineSpacingTMP = 1;
                defaultFont.fontScale = 0.5f;
                defaultFont.font = DefaultFont;
                defaultFont.fontTMP = DefaultTMPFont;
                OSFonts = Font.GetOSInstalledFontNames();
                OSFontPaths = Font.GetPathsToOSFonts();
                Fonts = new Dictionary<string, FontData>();
                initialized = true;
            }
        }
        public static void Release()
        {
            DefaultFont = null;
            DefaultTMPFont = null;
            FallbackFontDatas = null;
            FallbackFonts = null;
            defaultFont = default;
            OSFonts = null;
            OSFontPaths = null;
            Fonts = null;
            initialized = false;
        }
    }
}