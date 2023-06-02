using Overlayer.Core;
using Overlayer.Core.Translation;
using Overlayer.Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityModManagerNet;

namespace Overlayer
{
    public class FontMeta
    {
        public string name;
        public float lineSpacing = 1;
        public float fontScale = 0.5f;
        public bool Apply(out FontData font)
        {
            if (FontManager.TryGetFont(name, out font))
            {
                font.lineSpacing = lineSpacing;
                font.lineSpacingTMP = lineSpacing;
                font.fontScale = fontScale;
                return true;
            }
            return false;
        }
    }
    public class Settings : UnityModManager.ModSettings
    {
        public void Load()
        {
            if (AdofaiFont.Apply(out FontData font))
                FontManager.SetFont(AdofaiFont.name, font);
        }
        public bool AllowCollectingLevels = false;
        public bool ChangeFont = false;
        public bool DebugMode = true;
        public FontMeta AdofaiFont = new FontMeta();
        public SystemLanguage Lang = SystemLanguage.English;
        public bool DisableScript = false;
        //public int PerfStatUpdateRate = 100;
        public float FPSUpdateRate = 100;
        public float FrameTimeUpdateRate = 100;
        public void Draw()
        {
            AllowCollectingLevels = GUIUtils.RightToggle(AllowCollectingLevels, Main.Language[TranslationKeys.AllowCollectingLevel]);
            if (ChangeFont = GUIUtils.RightToggle(ChangeFont, Main.Language[TranslationKeys.AdofaiFont], v =>
            {
                if (!v)
                {
                    AdofaiFont.name = "Default";
                    if (AdofaiFont.Apply(out FontData font))
                    {
                        FontManager.SetFont(AdofaiFont.name, font);
                        RDString.initialized = false;
                        RDString.Setup();
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
            }))
            {
                GUIUtils.BeginIndent();
                GUILayout.BeginHorizontal();
                GUILayout.Label(Main.Language[TranslationKeys.AdofaiFont_Font]);
                AdofaiFont.name = GUILayout.TextField(AdofaiFont.name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(Main.Language[TranslationKeys.AdofaiFont_FontScale]);
                string scale = GUILayout.TextField(AdofaiFont.fontScale.ToString());
                _ = float.TryParse(scale, out float s) ? AdofaiFont.fontScale = s : 0;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(Main.Language[TranslationKeys.AdofaiFont_LineSpacing]);
                string lineSpacing = GUILayout.TextField(AdofaiFont.lineSpacing.ToString());
                _ = float.TryParse(lineSpacing, out float ls) ? AdofaiFont.lineSpacing = ls : 0;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Main.Language[TranslationKeys.Apply]))
                {
                    if (AdofaiFont.Apply(out FontData font))
                    {
                        FontManager.SetFont(AdofaiFont.name, font);
                        RDString.initialized = false;
                        RDString.Setup();
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                    else Main.Logger.Log($"Font Name '{AdofaiFont.name}' Does Not Exist.");
                }
                if (GUILayout.Button(Main.Language[TranslationKeys.LogFontList]))
                    foreach (string font in FontManager.OSFonts)
                        Main.Logger.Log(font);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUIUtils.EndIndent();
            }
            DebugMode = GUIUtils.RightToggle(DebugMode, Main.Language[TranslationKeys.DebugMode]);
            DisableScript = GUIUtils.RightToggle(DisableScript, "Disable Script");
            //string psur = PerfStatUpdateRate.ToString();
            //if (GUIUtils.DrawTextField(ref psur, Main.Language[TranslationKeys.PerfStatUpdateRate]))
            //    PerfStatUpdateRate = StringConverter.ToInt32(psur);
            string fur = FPSUpdateRate.ToString();
            if (GUIUtils.DrawTextField(ref fur, Main.Language[TranslationKeys.FPSUpdateRate]))
                FPSUpdateRate = StringConverter.ToFloat(fur);
            string ftur = FrameTimeUpdateRate.ToString();
            if (GUIUtils.DrawTextField(ref ftur, Main.Language[TranslationKeys.FrameTimeUpdateRate]))
                FrameTimeUpdateRate = StringConverter.ToFloat(ftur);
        }
    }
}
