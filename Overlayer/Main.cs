using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using static UnityModManagerNet.UnityModManager.ModEntry;
using static UnityModManagerNet.UnityModManager;
using System.IO;
using Overlayer.Core;
using Overlayer.Core.Translation;
using Overlayer.Scripting;
using UnityEngine;
using Overlayer.Scripting.JS;
using Overlayer.Scripting.CJS;
using Overlayer.Scripting.Python;
using UnityEngine.SceneManagement;
using Overlayer.Tags;
using Overlayer.Patches;
using System.Threading.Tasks;
using Overlayer.Core.Api.Overlayer;
using Overlayer.Core.Utils;

namespace Overlayer
{
    public static class Main
    {
        #region Variables
        public static bool HasScripts { get; private set; }
        public static ModEntry Mod { get; private set; }
        public static ModLogger Logger { get; private set; }
        public static Harmony Harmony { get; private set; }
        public static string ScriptPath => Path.Combine(Mod.Path, "Scripts");
        public static Language Language { get; private set; }
        public static Settings Settings { get; private set; }
        public static Texture2D OverlayerV2Logo { get; private set; }
        public static Scene ActiveScene { get; private set; }
        public static Version ModVersion { get; private set; }
        public static bool Initialized { get; private set; }
        public static bool ScriptsRunning { get; private set; }
        public static bool IsGUIOpen { get; private set; }
        #endregion
        #region UMM Impl
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            ModVersion = Mod.Version;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnShowGUI = OnShowGUI;
            modEntry.OnHideGUI = OnHideGUI;
            var logo = LoadManifestResource("ov2Logo.png");
            Texture2D logoTexture = new Texture2D(1, 1);
            logoTexture.LoadImage(logo);
            OverlayerV2Logo = logoTexture;
        }
        public static bool OnToggle(ModEntry modEntry, bool value)
        {
            if (value)
            {
                Variables.Reset();
                OverlayerDebug.Init();
                OverlayerDebug.Begin("Overlayer Initialized");
                OverlayerApi.Instance.StartSendHeartbeat();
                SceneManager.activeSceneChanged += SceneChanged;
                Backup();
                Settings = ModSettings.Load<Settings>(modEntry);
                Settings.Load();
                UpdateLanguage();
                Assembly ass = Assembly.GetExecutingAssembly();
                Harmony = new Harmony(modEntry.Info.Id);
                Harmony.PatchAll(ass);
                DistinctHarmonyPatches.Patch(Harmony);
                try
                {
                    TagManager.Load(ass);
                    TextManager.Load();
                    //File.WriteAllText(Path.Combine(Mod.Path, "TagInfos.json"), TagManager.GetTagInfos());
                }
                catch (Exception e) 
                { 
                    OverlayerDebug.Exception(e, "OnToggle: Loading Tag, Text");
                    OverlayerDebug.OpenDebugLog();
                }
                RunScriptsNonBlocking(ScriptPath);
                Initialized = true;
                OverlayerDebug.End();
            }
            else
            {
                OverlayerApi.Instance.StopSendHeartbeat();
                Script.ClearCache();
                OverlayerDebug.Term();
                SceneManager.activeSceneChanged -= SceneChanged;
                Harmony.UnpatchAll(Harmony.Id);
                DistinctHarmonyPatches.Unpatch(Harmony);
                Harmony = null;
                TextManager.Save();
                TextManager.Release();
                TagManager.Release();
                Initialized = false;
                MemoryHelper.Clean(CleanOption.All);
            }
            return true;
        }
        public static void OnGUI(ModEntry modEntry)
        {
            OverlayerV2LogoGUI();
            LanguageSelectGUI();
            OnlineUsersGUI();
            Settings.Draw();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Language[TranslationKeys.CleanMemory]))
                MemoryHelper.Clean(CleanOption.All);
            if (GUILayout.Button(Language[TranslationKeys.ReloadScripts]))
                RunScriptsNonBlocking(ScriptPath);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            TextManager.GUI();
        }
        public static void OnShowGUI(ModEntry modEntry) 
        {
            IsGUIOpen = true;
            OverlayerApi.Instance.UpdateOnlineUsers();
        }
        public static void OnHideGUI(ModEntry modEntry) 
        { 
            IsGUIOpen = false;
        }
        static Color cacheColor = Color.white;
        static bool focused = false;
        static int notFocucedFrames = 0;
        public static void OverlayerV2LogoGUI()
        {
            GUILayout.BeginHorizontal();
            GUIStyle bStyle = new GUIStyle(GUI.skin.box); 
            bool boxBtn = GUILayout.Button(OverlayerV2Logo, bStyle, GUILayout.Width(100), GUILayout.Height(100));
            bool boxHovering = GUIUtils.IsMouseHovering();
            GUILayout.BeginVertical();
            GUILayout.Space(7);
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.font = FontManager.GetFont("Default").font;
            style.fontSize = 80;
            style.richText = true;
            style.normal.textColor = focused ? cacheColor : Color.white;
            GUILayout.BeginHorizontal();
            bool btn1 = GUILayout.Button($"Overlayer V2 <size=40>v{ModVersion}</size>", style);
            bool btn1Hovering = GUIUtils.IsMouseHovering();
            if (btn1 || boxBtn)
                Application.OpenURL("https://discord.gg/S2FfgY76ay");
            if (btn1Hovering || boxHovering)
            {
                focused = true;
                notFocucedFrames = 0;
                cacheColor = MiscUtils.ShiftHue(cacheColor, Time.deltaTime * 0.2f);
            }
            else notFocucedFrames++;
            if (notFocucedFrames > 1)
            {
                notFocucedFrames = 0;
                focused = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        public static void LanguageSelectGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("한국어"))
            {
                Settings.Lang = SystemLanguage.Korean;
                UpdateLanguage();
            }
            if (GUILayout.Button("English"))
            {
                Settings.Lang = SystemLanguage.English;
                UpdateLanguage();
            }
            if (GUILayout.Button("中國語"))
            {
                Settings.Lang = SystemLanguage.Chinese;
                UpdateLanguage();
            }
            if (GUILayout.Button("日本語"))
            {
                Settings.Lang = SystemLanguage.Japanese;
                UpdateLanguage();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static void OnlineUsersGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.font = FontManager.GetFont("Default").font;
            style.fontSize = 40;
            GUILayout.Label($"{Language[TranslationKeys.OnlineUsers]}: {OverlayerApi.Instance.OnlineUsers}", style);
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            // Force Save Persistence
            foreach (var kvp in PlaytimeCounter.PlayTimes)
                Persistence.generalPrefs.SetFloat(kvp.Key, kvp.Value);
            List<object> list = new List<object>();
            foreach (CalibrationPreset calibrationPreset in scrConductor.userPresets)
                list.Add(calibrationPreset.ToDict());
            Persistence.generalPrefs.SetList("calibrationPresets", list);
            PlayerPrefsJson playerPrefsJson = PlayerPrefsJson.SelectAll();
            playerPrefsJson.deltaDict.Add("version", 101);
            playerPrefsJson.ApplyDeltaDict();
            PlayerPrefsJson.SaveAllFiles();

            ModSettings.Save(Settings, modEntry);
            TextManager.Save();
            OverlayerDebug.SaveLog();
            MemoryHelper.Clean(CleanOption.CollectGarbage);
        }
        public static void OnUpdate(ModEntry modEntry, float deltaTime)
        {
            UpdateFpsTags(deltaTime);
        }
        #endregion
        #region Functions
        public static void SceneChanged(Scene from, Scene to)
        {
            ActiveScene = to;
        }
        public static async Task RunScripts(string folderPath)
        {
            if (ScriptsRunning) return;
            ScriptsRunning = true;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            OverlayerDebug.Log($"Generating Script Implementations..");
            await Task.Run(() => File.WriteAllText(Path.Combine(folderPath, "Impl.js"), new JavaScriptImpl().Generate()));
            await Task.Run(() => File.WriteAllText(Path.Combine(folderPath, "CImpl.js"), new CompilableJavaScriptImpl().Generate()));
            await Task.Run(() => File.WriteAllText(Path.Combine(folderPath, "Impl.py"), new PythonImpl().Generate()));
            OverlayerDebug.Log($"Preparing Executing Scripts..");
            Api.Clear();
            Script.ClearCache();
            Expression.expressions.Clear();
            OverlayerDebug.Begin("Executing All Scripts");
            foreach (string script in Directory.GetFiles(folderPath))
            {
                var nameWithoutExt = Path.GetFileNameWithoutExtension(script);
                if (nameWithoutExt == "Impl" ||
                    nameWithoutExt == "CImpl") 
                    continue;
                if (nameWithoutExt.EndsWith("_Proxy")) continue;
                if (nameWithoutExt.EndsWith("_Compilable")) continue;
                ScriptType sType = Script.GetScriptType(script);
                if (sType == ScriptType.None) continue;
                var name = Path.GetFileName(script);
                await RunScript(name, script, sType);
            }
            OverlayerDebug.Disable();
            MemoryHelper.Clean();
            OverlayerDebug.Enable();
            OverlayerDebug.End();
            ScriptsRunning = false;
        }
        public static async Task<bool> RunScript(string name, string script, ScriptType sType)
        {
            OverlayerDebug.Begin($"Executing Script {name}");
            bool success = await Task.Run(() =>
            {
                try
                {
                    var result = Script.Compile(script, sType);
                    result.Exec();
                    result.Dispose();
                    result = null;
                    HasScripts = true;
                    return true;
                }
                catch (Exception e) { OverlayerDebug.Log($"Exception At Executing Script \"{name}\":\n{e}"); return false; }
            });
            OverlayerDebug.End(success);
            return success;
        }
        public static async void RunScriptsNonBlocking(string folderPath)
        {
            await RunScripts(folderPath);
        }
        public static byte[] LoadManifestResource(string name)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            Stream s = ass.GetManifestResourceStream($"Overlayer.{name}");
            byte[] buffer = new byte[s.Length];
            s.Read(buffer, 0, buffer.Length);
            return buffer;
        }
        public static void UpdateLanguage()
        {
            switch (Settings.Lang)
            {
                case SystemLanguage.Korean:
                    Language = Language.Korean;
                    break;
                case SystemLanguage.English:
                    Language = Language.English;
                    break;
                case SystemLanguage.Chinese:
                    Language = Language.Chinese;
                    break;
                case SystemLanguage.Japanese:
                    Language = Language.Japanese;
                    break;
            }
        }
        public static void Backup()
        {
            foreach (var file in Directory.GetFiles(Mod.Path, "*.json").Concat(Directory.GetFiles(Mod.Path, "*.txtgrp")).Concat(Directory.GetFiles(Mod.Path, "*.xml")).Where(f => Path.GetFileName(f) != "info.json"))
                File.WriteAllBytes(file + ".backup", File.ReadAllBytes(file));
        }
        public static void Recover()
        {
            foreach (var file in Directory.GetFiles(Mod.Path, "*.backup"))
                File.WriteAllBytes(file.Remove(file.LastIndexOf(".backup"), 7), File.ReadAllBytes(file));
        }
        #endregion
        #region Misc Functions
        static float lastDeltaTime;
        static float fpsTimer;
        static float fpsTimeTimer;
        public static void UpdateFpsTags(float deltaTime)
        {
            lastDeltaTime += (deltaTime - lastDeltaTime) * 0.1f;
            if (fpsTimer > Settings.FPSUpdateRate / 1000.0f)
            {
                Variables.Fps = 1.0f / lastDeltaTime;
                fpsTimer = 0;
            }
            fpsTimer += deltaTime;
            if (fpsTimeTimer > Settings.FrameTimeUpdateRate / 1000.0f)
            {
                Variables.FrameTime = lastDeltaTime * 1000.0f;
                fpsTimeTimer = 0;
            }
            fpsTimeTimer += deltaTime;
        }
        #endregion
    }
}
