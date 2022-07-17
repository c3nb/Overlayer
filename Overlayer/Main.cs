using HarmonyLib;
using Overlayer.Patches;
using Overlayer.Tags;
using Overlayer.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace Overlayer
{
    public static class Main
    {
        public static ModEntry Mod;
        public static ModEntry.ModLogger Logger;
        public static Harmony Harmony;
        public static float fpsTimer = 0;
        public static float fpsTimeTimer = 0;
        public static float lastDeltaTime;
        public static TagCollection AllTags;
        public static TagCollection NotPlayingTags;
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            modEntry.OnUpdate = (mod, deltaTime) =>
            {
                if (Input.anyKeyDown)
                    Variables.KpsTemp++;
                lastDeltaTime += (Time.deltaTime - lastDeltaTime) * 0.1f;
                if (fpsTimer > Settings.Instance.FPSUpdateRate / 1000.0f)
                {
                    Variables.Fps = 1.0f / lastDeltaTime;
                    fpsTimer = 0;
                }
                fpsTimer += deltaTime;

                if (fpsTimeTimer > Settings.Instance.FrameTimeUpdateRate / 1000.0f)
                {
                    Variables.FrameTime = lastDeltaTime * 1000.0f;
                    fpsTimeTimer = 0;
                }
                fpsTimeTimer += deltaTime;
            };
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
        }
        public static bool OnToggle(ModEntry modEntry, bool value)
        {
            try
            {
                if (value)
                {
                    var asm = Assembly.GetExecutingAssembly();
                    Settings.Load(modEntry);
                    Variables.Reset();
                    AllTags = new TagCollection();
                    AllTags.LoadTags(asm);
                    NotPlayingTags = new TagCollection(new[]
                    {
                        AllTags["Year"],
                        AllTags["Month"],
                        AllTags["Day"],
                        AllTags["Hour"],
                        AllTags["Minute"],
                        AllTags["Second"],
                        AllTags["MilliSecond"],
                        AllTags["Fps"],
                        AllTags["FrameTime"],
                        AllTags["CurKps"],
                    });
                    AllTags.ForEach(t => t.Start());
                    CustomTag.Load();
                    foreach (CustomTag cTag in CustomTag.Tags)
                    {
                        string err = cTag.Compile(AllTags, cTag.name, cTag.description, cTag.expression);
                        cTag.name_ = cTag.name;
                        cTag.description_ = cTag.description;
                        cTag.expression_ = cTag.expression;
                        if (err != null)
                            Logger.Log($"Custom Tag {cTag.name} Is An Error Occured ({err}). Please Check Your Expression");
                    }
                    OText.Load();
                    if (!OText.Texts.Any())
                        _ = new OText().Apply();
                    Harmony = new Harmony(modEntry.Info.Id);
                    Harmony.PatchAll(asm);
                    var settings = Settings.Instance;
                    DeathMessagePatch.compiler = new TagCompiler(AllTags);
                    if (!string.IsNullOrEmpty(settings.DeathMessage))
                        DeathMessagePatch.compiler.Compile(settings.DeathMessage);
                }
                else
                {
                    OnSaveGUI(modEntry);
                    try
                    {
                        OText.Clear();
                        AllTags.Clear();
                        NotPlayingTags.Clear();
                        DeathMessagePatch.compiler = null;
                        AllTags = null;
                        NotPlayingTags = null;
                        GC.Collect();
                    }
                    finally
                    {
                        Harmony.UnpatchAll(Harmony.Id);
                        Harmony = null;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
                return false;
            }
        }
        public static void OnGUI(ModEntry modEntry)
        {
            Settings.Instance.Draw(modEntry);
            var settings = Settings.Instance;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Death Message");
            var dm = GUILayout.TextField(settings.DeathMessage);
            if (dm != settings.DeathMessage)
            {
                settings.DeathMessage = dm;
                if (!string.IsNullOrEmpty(settings.DeathMessage))
                    DeathMessagePatch.compiler.Compile(settings.DeathMessage);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            var cTags = CustomTag.Tags;
            if (settings.EditingCustomTags = GUILayout.Toggle(settings.EditingCustomTags, "Edit CustomTags"))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("New Custom Tag"))
                    cTags.Add(new CustomTag());
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUIUtils.IndentGUI(() =>
                {
                    for (int i = 0; i < cTags.Count; i++)
                    {
                        var changed = false;
                        CustomTag cTag = cTags[i];
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:");
                        cTag.name_ = GUILayout.TextField(cTag.name_);
                        if (cTag.name_ != cTag.name)
                            changed = true;
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Description:");
                        cTag.description_ = GUILayout.TextField(cTag.description_);
                        if (cTag.description_ != cTag.description)
                            changed = true;
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Expression:");
                        cTag.expression_ = GUILayout.TextField(cTag.expression_);
                        if (cTag.expression_ != cTag.expression)
                            changed = true;
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        GUILayout.Label($"This Tag Can{(cTag.canUsedByNotPlaying ? "" : "not")} Be Used By Not Playing Text.");

                        GUILayout.BeginHorizontal();
                        if (changed == true)
                        {
                            if (GUILayout.Button("Compile"))
                            {
                                cTag.Compile(AllTags, cTag.name_, cTag.description_, cTag.expression_);
                                changed = false;
                            }
                        }
                        if (GUILayout.Button("Remove"))
                        {
                            AllTags.RemoveTag(cTag.name);
                            NotPlayingTags.RemoveTag(cTag.name);
                            cTag.Recompile();
                            cTags.RemoveAt(i);
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        if (cTag.error != null)
                            GUILayout.Label($"Compilation Error: {cTag.error}");
                        GUILayout.Space(3);
                    }
                });
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Text"))
            {
                _ = new OText().Apply();
                OText.Order();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            for (int i = 0; i < OText.Texts.Count; i++)
                OText.Texts[i].GUI();
            AllTags.DescGUI();
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            Settings.Save(modEntry);
            Variables.Reset();
            OText.Save();
            CustomTag.Save();
        }
    }
}
