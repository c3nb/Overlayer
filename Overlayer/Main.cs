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
using SA.GoogleDoc;

namespace Overlayer
{
    public static class Main
    {
        #region Variables
        public static ModEntry Mod { get; private set; }
        public static ModLogger Logger { get; private set; }
        public static Harmony Harmony { get; private set; }
        public static string ScriptPath => Path.Combine(Mod.Path, "Scripts");
        public static Language Language { get; private set; }
        public static Settings Settings { get; private set; }
        #endregion
        #region UMM Impl
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
        }
        public static bool OnToggle(ModEntry modEntry, bool value)
        {
            if (value)
            {
                Settings = ModSettings.Load<Settings>(modEntry);
                Assembly ass = Assembly.GetExecutingAssembly();
                TagManager.Load(ass);
                Harmony = new Harmony(modEntry.Info.Id);
                Harmony.PatchAll(ass);
                RunScripts(ScriptPath);
            }
            else
            {
                Harmony.UnpatchAll(Harmony.Id);
                Harmony = null;
                TagManager.Release();
            }
            return true;
        }
        public static void OnGUI(ModEntry modEntry)
        {
            LanguageSelectGUI();
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
        public static void OnSaveGUI(ModEntry modEntry)
        {
            ModSettings.Save(Settings, modEntry);
        }
        #endregion
        #region Functions
        public static IEnumerable<Script> RunScripts(string folderPath)
        {
            foreach (string script in Directory.GetFiles(folderPath))
            {
                ScriptType sType = Script.GetScriptType(script);
                if (sType == ScriptType.None) continue;
                Script scr = Script.Create(script, sType);
                scr.Execute();
                yield return scr;
            }
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
    }
}
