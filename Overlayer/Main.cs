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
using System.Text;
using Overlayer.Core.Translation;
using Overlayer.Core.Tags;
using Overlayer.Core.Tags.Nodes;

namespace Overlayer
{
    public static class Main
    {
        public static ModEntry Mod;
        public static ModEntry.ModLogger Logger;
        public static Harmony Harmony;
        public static Language Language;
        public static float fpsTimer = 0;
        public static float fpsTimeTimer = 0;
        public static float lastDeltaTime;
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            var asm = Assembly.GetExecutingAssembly();
            Settings.Load(modEntry);
            UpdateLanguage();
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

                TagManager.AllTags["TEHex"],
                TagManager.AllTags["VEHex"],
                TagManager.AllTags["EPHex"],
                TagManager.AllTags["PHex"],
                TagManager.AllTags["LPHex"],
                TagManager.AllTags["VLHex"],
                TagManager.AllTags["TLHex"],
                TagManager.AllTags["MPHex"],
                TagManager.AllTags["FMHex"],
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
                    Function.Load();
                    foreach (CustomTag cTag in CustomTag.Tags)
                    {
                        try
                        {
                            string err = cTag.Compile(TagManager.AllTags, cTag.name, cTag.description, cTag.expression, Recompile);
                            cTag.name_ = cTag.name;
                            cTag.description_ = cTag.description;
                            cTag.expression_ = cTag.expression;
                            if (err != null)
                                Logger.Log($"{Language[TranslationKeys.CustomTag]} {cTag.name} {Language[TranslationKeys.IsErrorOccured]} ({err}). {Language[TranslationKeys.PlzChkYourExpr]}");
                        }
                        catch (Exception e) { Logger.Log($"{Language[TranslationKeys.ExceptionAt]} {TranslationKeys.CustomTag} {cTag.name}.\n{e}"); }
                    }
                    foreach (Function func in Function.Functions)
                    {
                        try
                        {
                            func.Compile(func.name, func.expression, Recompile);
                            func.name_ = func.name;
                            func.expression_ = func.expression;
                            if (func.error != null)
                                Logger.Log($"{Language[TranslationKeys.Function]} {func.name} {Language[TranslationKeys.IsErrorOccured]} ({func.error}). {Language[TranslationKeys.PlzChkYourExpr]}");
                        }
                        catch (Exception e) { Logger.Log($"{Language[TranslationKeys.ExceptionAt]} {TranslationKeys.Function} {func.name}.\n{e}"); }
                    }
                    OText.Load();
                    if (!OText.Texts.Any())
                        new OText().Apply();
                    Harmony = new Harmony(modEntry.Info.Id);
                    Harmony.PatchAll(Assembly.GetExecutingAssembly());
                    UpdateLanguage();
                    var settings = Settings.Instance;
                    DeathMessagePatch.compiler = new TextCompiler(TagManager.AllTags);
                    if (!string.IsNullOrEmpty(settings.DeathMessage))
                        DeathMessagePatch.compiler.Compile(settings.DeathMessage);
                    if (!string.IsNullOrEmpty(settings.ClearMessage))
                        ClearMessagePatch.compiler.Compile(settings.ClearMessage);
                }
                else
                {
                    OnSaveGUI(modEntry);
                    try
                    {
                        OText.Clear();
                        DeathMessagePatch.compiler = null;
                        ClearMessagePatch.compiler = null;
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
            var settings = Settings.Instance;
            LangGUI(settings);
            settings.DrawManual();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Language[TranslationKeys.DeathMessage]);
            var dm = GUILayout.TextField(settings.DeathMessage);
            if (dm != settings.DeathMessage)
            {
                settings.DeathMessage = dm;
                if (!string.IsNullOrEmpty(settings.DeathMessage))
                    DeathMessagePatch.compiler.Compile(settings.DeathMessage);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Language[TranslationKeys.ClearMessage]);
            var cm = GUILayout.TextField(settings.ClearMessage);
            if (cm != settings.ClearMessage)
            {
                settings.ClearMessage = cm;
                if (!string.IsNullOrEmpty(settings.ClearMessage))
                    ClearMessagePatch.compiler.Compile(settings.ClearMessage);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            var cTags = CustomTag.Tags;
            if (settings.EditingCustomTags = GUILayout.Toggle(settings.EditingCustomTags, Language[TranslationKeys.EditCustomTag]))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Language[TranslationKeys.NewCustomTag]))
                    cTags.Add(new CustomTag());
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUIUtils.IndentGUI(() =>
                {
                    for (int i = 0; i < cTags.Count; i++)
                    {
                        CustomTag cTag = cTags[i];
                        if (cTag.editing = GUILayout.Toggle(cTag.editing, $"{cTag.name}"))
                        {
                            GUIUtils.IndentGUI(() =>
                            {
                                var changed = false;
                                GUILayout.BeginHorizontal();
                                GUILayout.Label($"{Language[TranslationKeys.Name]}:");
                                cTag.name_ = GUILayout.TextField(cTag.name_);
                                if (cTag.name_ != cTag.name)
                                    changed = true;
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label($"{Language[TranslationKeys.Description]}:");
                                cTag.description_ = GUILayout.TextField(cTag.description_);
                                if (cTag.description_ != cTag.description)
                                    changed = true;
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label($"{Language[TranslationKeys.Expression]}:");
                                cTag.expression_ = GUILayout.TextField(cTag.expression_);
                                if (cTag.expression_ != cTag.expression)
                                    changed = true;
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();

                                GUILayout.Label($"{Language[TranslationKeys.ThisTag]} {(cTag.canUsedByNotPlaying ? Language[TranslationKeys.CanBeUsedNotPlayingText] : Language[TranslationKeys.CannotBeUsedNotPlayingText])}.");
                                GUILayout.Label((cTag.isStringTag ? Language[TranslationKeys.ThisTagIsStringTag] : Language[TranslationKeys.ThisFuncIsNumberFunc]) + '.');

                                GUILayout.BeginHorizontal();
                                if (changed == true)
                                {
                                    if (GUILayout.Button(Language[TranslationKeys.Compile]))
                                    {
                                        cTag.Compile(TagManager.AllTags, cTag.name_, cTag.description_, cTag.expression_, Recompile);
                                        changed = false;
                                    }
                                }
                                if (GUILayout.Button(Language[TranslationKeys.Remove]))
                                {
                                    TagManager.AllTags.RemoveTag(cTag.name);
                                    TagManager.NotPlayingTags.RemoveTag(cTag.name);
                                    Recompile();
                                    cTags.RemoveAt(i);
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                                if (cTag.error != null)
                                    GUILayout.Label($"{Language[TranslationKeys.CompilationError]}: {cTag.error}");
                            });
                        }
                        GUILayout.Space(3);
                    }
                });
                CustomTag.ConstantsGUI();
                CustomTag.FunctionGUI();
            }
            var funcs = Function.Functions;
            if (settings.EditingFunctions = GUILayout.Toggle(settings.EditingFunctions, Language[TranslationKeys.EditFunction]))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Language[TranslationKeys.NewFunction]))
                    funcs.Add(new Function(""));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUIUtils.IndentGUI(() =>
                {
                    for (int i = 0; i < funcs.Count; i++)
                    {
                        Function func = funcs[i];
                        if (func.editing = GUILayout.Toggle(func.editing, $"{func.name}"))
                        {
                            GUIUtils.IndentGUI(() =>
                            {
                                var changed = false;
                                GUILayout.BeginHorizontal();
                                GUILayout.Label($"{Language[TranslationKeys.Name]}:");
                                func.name_ = GUILayout.TextField(func.name_);
                                if (func.name_ != func.name)
                                    changed = true;
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label($"{Language[TranslationKeys.Expression]}:");
                                func.expression_ = GUILayout.TextField(func.expression_);
                                if (func.expression_ != func.expression)
                                    changed = true;
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                if (GUILayout.Button(Language[TranslationKeys.NewArgument]))
                                    func.AddArgument(false);
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                                for (int j = 0; j < func.args.Count; j++)
                                {
                                    var arg = func.args[j];
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label($"{Language[TranslationKeys.Index]} {j} {Language[TranslationKeys.Argument]}:");
                                    if (GUILayout.Button(arg.IsString ? Language[TranslationKeys.String] : Language[TranslationKeys.Number]))
                                        arg.IsString = !arg.IsString;
                                    if (j == func.args.Count - 1)
                                    {
                                        if (GUILayout.Button(Language[TranslationKeys.Remove]))
                                            func.RemoveArgument();
                                    }
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();
                                }

                                GUILayout.Label($"{Language[TranslationKeys.ThisFunc]} {(func.canUsedByNotPlaying ? Language[TranslationKeys.CanBeUsedNotPlayingText] : Language[TranslationKeys.CannotBeUsedNotPlayingText])}.");
                                GUILayout.Label((func.isStringFunc ? Language[TranslationKeys.ThisFuncIsStringFunc] : Language[TranslationKeys.ThisFuncIsNumberFunc]) + '.');

                                GUILayout.BeginHorizontal();
                                if (changed == true)
                                {
                                    if (GUILayout.Button(Language[TranslationKeys.Compile]))
                                    {
                                        func.Compile(func.name_, func.expression_, Recompile);
                                        changed = false;
                                    }
                                }
                                if (GUILayout.Button(Language[TranslationKeys.Remove]))
                                {
                                    func.Remove();
                                    Recompile();
                                    funcs.RemoveAt(i);
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                                if (func.error != null)
                                    GUILayout.Label($"{Language[TranslationKeys.CompilationError]}: {func.error}");
                            });
                        }
                        GUILayout.Space(3);
                    }
                });
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Language[TranslationKeys.AddText]))
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
        public static void LangGUI(Settings settings)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("한국어"))
            {
                settings.lang = SystemLanguage.Korean;
                UpdateLanguage();
            }
            if (GUILayout.Button("English"))
            {
                settings.lang = SystemLanguage.English;
                UpdateLanguage();
            }
            if (GUILayout.Button("中國語"))
            {
                settings.lang = SystemLanguage.Chinese;
                UpdateLanguage();
            }
            if (GUILayout.Button("日本語"))
            {
                settings.lang = SystemLanguage.Japanese;
                UpdateLanguage();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static void UpdateLanguage()
        {
            switch (Settings.Instance.lang)
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
        public static void OnSaveGUI(ModEntry modEntry)
        {
            Settings.Save(modEntry);
            Variables.Reset();
            OText.Save();
            CustomTag.Save();
            Function.Save();
        }
        public static void Recompile()
        {
            foreach (CustomTag tag in CustomTag.Tags)
            {
                try { tag.Compile(TagManager.AllTags, tag.name, tag.description, tag.expression); }
                catch { }
            }
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
