using HarmonyLib;
using Overlayer.Patches;
using Overlayer.Core;
using Overlayer.Core.Utils;
using Overlayer;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using UnityModManagerNet;
using System.Runtime.CompilerServices;
using static UnityModManagerNet.UnityModManager;
using System.IO;
using Overlayer.Tags.Global;

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
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            var asm = Assembly.GetExecutingAssembly();
            Settings.Load(modEntry);
            Performance.Init();
            TagManager.AllTags.LoadTags(asm);
            TagManager.NotPlayingTags.AddTags(new[]
            {
                TagManager.AllTags["Year"],
                TagManager.AllTags["Month"],
                TagManager.AllTags["Day"],
                TagManager.AllTags["Hour"],
                TagManager.AllTags["Minute"],
                TagManager.AllTags["Second"],
                TagManager.AllTags["MilliSecond"],
                TagManager.AllTags["Fps"],
                TagManager.AllTags["FrameTime"],
                TagManager.AllTags["CurKps"],

                TagManager.AllTags["ProcessorCount"],
                TagManager.AllTags["MemoryGBytes"],
                TagManager.AllTags["CpuUsage"],
                TagManager.AllTags["TotalCpuUsage"],
                TagManager.AllTags["MemoryUsage"],
                TagManager.AllTags["TotalMemoryUsage"],
                TagManager.AllTags["MemoryUsageGBytes"],
                TagManager.AllTags["TotalMemoryUsageGBytes"],

                TagManager.AllTags["TLHex"],
                TagManager.AllTags["VEHex"],
                TagManager.AllTags["EPHex"],
                TagManager.AllTags["PHex"],
                TagManager.AllTags["LPHex"],
                TagManager.AllTags["VLHex"],
                TagManager.AllTags["TLHex"],
                TagManager.AllTags["MPHex"],
                TagManager.AllTags["FOHex"],
                TagManager.AllTags["FOHex"],
            });
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
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
        }
        public static bool OnToggle(ModEntry modEntry, bool value)
        {
            try
            {
                if (value)
                {
                    Settings.Load(modEntry);
                    Variables.Reset();
                    CustomTag.Load();
                    foreach (CustomTag cTag in CustomTag.Tags)
                    {
                        try
                        {
                            string err = cTag.Compile(TagManager.AllTags, cTag.name, cTag.description, cTag.expression, Recompile);
                            cTag.name_ = cTag.name;
                            cTag.description_ = cTag.description;
                            cTag.expression_ = cTag.expression;
                            if (err != null)
                                Logger.Log($"Custom Tag {cTag.name} Is An Error Occured ({err}). Please Check Your Expression");
                        }
                        catch (Exception e) { Logger.Log($"Exception At Custom Tag {cTag.name}.\n{e}"); }
                    }
                    OText.Load();
                    if (!OText.Texts.Any())
                        new OText().Apply();
                    Harmony = new Harmony(modEntry.Info.Id);
                    Harmony.PatchAll(Assembly.GetExecutingAssembly());
                    var settings = Settings.Instance;
                    DeathMessagePatch.compiler = new TextCompiler(TagManager.AllTags);
                    if (!string.IsNullOrEmpty(settings.DeathMessage))
                        DeathMessagePatch.compiler.Compile(settings.DeathMessage);
                    Settings.Instance.OnChange();
                }
                else
                {
                    OnSaveGUI(modEntry);
                    try
                    {
                        OText.Clear();
                        DeathMessagePatch.compiler = null;
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
                        CustomTag cTag = cTags[i];
                        if (cTag.collapsed = GUILayout.Toggle(cTag.collapsed, $"{cTag.name}"))
                        {
                            GUIUtils.IndentGUI(() =>
                            {
                                var changed = false;
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
                                GUILayout.Label($"This Tag Is {(cTag.isStringTag ? "String" : "Number")} Tag.");

                                GUILayout.BeginHorizontal();
                                if (changed == true)
                                {
                                    if (GUILayout.Button("Compile"))
                                    {
                                        cTag.Compile(TagManager.AllTags, cTag.name_, cTag.description_, cTag.expression_, Recompile);
                                        changed = false;
                                    }
                                }
                                if (GUILayout.Button("Remove"))
                                {
                                    TagManager.AllTags.RemoveTag(cTag.name);
                                    TagManager.NotPlayingTags.RemoveTag(cTag.name);
                                    Recompile();
                                    cTags.RemoveAt(i);
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                                if (cTag.error != null)
                                    GUILayout.Label($"Compilation Error: {cTag.error}");
                            });
                        }
                        GUILayout.Space(3);
                    }
                });
                CustomTag.ConstantsGUI();
                CustomTag.FunctionGUI();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Text"))
            {
                new OText().Apply();
                OText.Order();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            for (int i = 0; i < OText.Texts.Count; i++)
                OText.Texts[i].GUI();
            TagManager.AllTags.DescGUI();
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            Settings.Save(modEntry);
            Variables.Reset();
            OText.Save();
            CustomTag.Save();
        }
        public static void Recompile()
        {
            foreach (OText text in OText.Texts)
            {
                text.PlayingCompiler.Compile(text.TSetting.PlayingText);
                text.NotPlayingCompiler.Compile(text.TSetting.NotPlayingText);
                text.BrokenPlayingCompiler.Compile(text.TSetting.PlayingText.BreakRichTagWithoutSize());
                text.BrokenNotPlayingCompiler.Compile(text.TSetting.NotPlayingText.BreakRichTagWithoutSize());
            }
        }
    }
}
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
    [AttributeUsage(AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, Inherited = false)]
    internal sealed class SkipLocalsInitAttribute : Attribute { }
    public static class RuntimeFeature
    {
        public const string CovariantReturnsOfClasses = nameof(CovariantReturnsOfClasses);
        public const string DefaultInterfaceImplementation = nameof(DefaultInterfaceImplementation);
        public const string PortablePdb = nameof(PortablePdb);
        public const string UnmanagedSignatureCallingConvention = nameof(UnmanagedSignatureCallingConvention);
        public const string VirtualStaticsInInterfaces = nameof(VirtualStaticsInInterfaces);
        public static bool IsDynamicCodeCompiled => true;
        public static bool IsDynamicCodeSupported => true;
        public static bool IsSupported(string feature)
        {
            switch (feature)
            {
                case CovariantReturnsOfClasses:
                case DefaultInterfaceImplementation:
                case PortablePdb:
                case UnmanagedSignatureCallingConvention:
                case VirtualStaticsInInterfaces:
                    return true;
                case nameof(IsDynamicCodeCompiled):
                    return IsDynamicCodeCompiled;
                case nameof(IsDynamicCodeSupported):
                    return IsDynamicCodeSupported;
                default:
                    return false;
            }
        }
    }
}
