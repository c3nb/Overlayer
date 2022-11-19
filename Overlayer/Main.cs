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
using Rewired.UI.ControlMapper;
using Overlayer.Core.JavaScript;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        public static byte[] Impljs;
        public static void Load(ModEntry modEntry)
        {
            CustomTagsPath = Path.Combine(modEntry.Path, "CustomTags");
            InitJSPath = Path.Combine(modEntry.Path, "Inits");
            Mod = modEntry;
            Logger = modEntry.Logger;
            var asm = Assembly.GetExecutingAssembly();
            Settings.Load(modEntry);
            UpdateLanguage();
            Performance.Init();
            TagManager.AllTags.LoadTags(asm);
            using var impljs = asm.GetManifestResourceStream("Overlayer.Impl.js");
            Impljs = new byte[impljs.Length];
            impljs.Read(Impljs, 0, Impljs.Length);

            //StringBuilder sb = new StringBuilder();
            //foreach (Tag tag in TagManager.AllTags)
            //{
            //    sb.AppendLine("/**");
            //    if (tag.IsOpt)
            //    {
            //        if (tag.IsStringOpt)
            //            sb.AppendLine(" * @param {string} opt");
            //        else sb.AppendLine(" * @param {number} opt");
            //    }
            //    if (tag.IsString)
            //        sb.AppendLine($" * @returns {{string}} {tag.Description}");
            //    else sb.AppendLine($" * @returns {{number}} {tag.Description}");
            //    sb.AppendLine(" */");
            //    sb.Append($"function {tag.Name}(");
            //    if (tag.IsOpt)
            //        sb.Append("opt");
            //    sb.AppendLine(");");
            //}
            //File.WriteAllText("Mods/Overlayer/Tags.js", sb.ToString());

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
        public static bool LoadJSTag(string source, string name, out Tag tag)
        {
            tag = null;
            string desc = "";
            using (StringReader sr = new StringReader(source))
            {
                string first = sr.ReadLine();
                if (first.StartsWith("//"))
                    desc = first.Remove(0, 2).Trim();
            }
            try
            {
                var del = source.CompileEval();
                tag = new Tag(name, desc, del);
                Logger.Log($"Loaded '{name}' Tag.");
                return true;
            }
            catch (Exception e)
            {
                Logger.Log($"Exception At Loading {name} Tag..\n({e})");
                return false;
            }
        }
        public static readonly List<string> JSTagCache = new List<string>();
        public static void LoadAllJSTags(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                var impljsPath = Path.Combine(folderPath, "Impl.js");
                File.WriteAllBytes(impljsPath, Impljs);
                File.SetAttributes(impljsPath, FileAttributes.ReadOnly);
                return;
            }
            if (!File.Exists(Path.Combine(folderPath, "Impl.js")))
            {
                var impljsPath = Path.Combine(folderPath, "Impl.js");
                File.WriteAllBytes(impljsPath, Impljs);
                File.SetAttributes(impljsPath, FileAttributes.ReadOnly);
            }
            int success = 0, fail = 0;
            foreach (string path in Directory.GetFiles(folderPath, "*.js"))
            {
                var name = Path.GetFileNameWithoutExtension(path);
                if (name == "Impl") continue;
                if (LoadJSTag(File.ReadAllText(path), name, out Tag tag))
                {
                    TagManager.AllTags[tag.Name] = tag;
                    TagManager.NotPlayingTags[tag.Name] = tag;
                    JSTagCache.Add(tag.Name);
                    success++;
                }
                else fail++;
            }
            Logger.Log($"Loaded {success} Scripts Successfully. (Failed: {fail})");
        }
        public static void ReloadAllJSTags(string folderPath)
        {
            Logger.Log($"Reloading {JSTagCache.Count} Scripts..");
            UnloadAllJSTags();
            LoadAllJSTags(folderPath);
        }
        public static void RunInits()
        {
            if (!Directory.Exists(InitJSPath))
            {
                Directory.CreateDirectory(InitJSPath);
                var impljsPath = Path.Combine(InitJSPath, "Impl.js");
                File.WriteAllBytes(impljsPath, Impljs);
                File.SetAttributes(impljsPath, FileAttributes.ReadOnly);
            }
            else
            {
                if (!File.Exists(Path.Combine(InitJSPath, "Impl.js")))
                {
                    var impljsPath = Path.Combine(InitJSPath, "Impl.js");
                    File.WriteAllBytes(impljsPath, Impljs);
                    File.SetAttributes(impljsPath, FileAttributes.ReadOnly);
                }
                foreach (string file in Directory.GetFiles(InitJSPath, "*.js"))
                {
                    if (Path.GetFileNameWithoutExtension(file) == "Impl")
                        continue;
                    File.ReadAllText(file).CompileExec()();
                }
            }
        }
        public static void UnloadAllJSTags()
        {
            foreach (string tagName in JSTagCache)
            {
                TagManager.AllTags.RemoveTag(tagName);
                TagManager.NotPlayingTags.RemoveTag(tagName);
            }
            JSTagCache.Clear();
        }
        public static string CustomTagsPath;
        public static string InitJSPath;
        public static UnityAction<Scene, LoadSceneMode> evt = (s, m) => JSPatches.SceneLoads();
        public static bool OnToggle(ModEntry modEntry, bool value)
        {
            try
            {
                if (value)
                {
                    SceneManager.sceneLoaded += evt;
                    Settings.Load(modEntry);
                    Variables.Reset();
                    LoadAllJSTags(CustomTagsPath);
                    OText.Load();
                    if (!OText.Texts.Any())
                        new OText().Apply();
                    RunInits();
                    Harmony = new Harmony(modEntry.Info.Id);
                    Harmony.PatchAll(Assembly.GetExecutingAssembly());
                    UpdateLanguage();
                    var settings = Settings.Instance;
                    DeathMessagePatch.compiler = new TextCompiler(TagManager.AllTags);
                    ClearMessagePatch.compiler = new TextCompiler(TagManager.AllTags);
                    if (!string.IsNullOrEmpty(settings.DeathMessage))
                        DeathMessagePatch.compiler.Compile(settings.DeathMessage);
                    if (!string.IsNullOrEmpty(settings.ClearMessage))
                        ClearMessagePatch.compiler.Compile(settings.ClearMessage);
                }
                else
                {
                    SceneManager.sceneLoaded -= evt;
                    OnSaveGUI(modEntry);
                    try
                    {
                        OText.Clear();
                        UnloadAllJSTags();
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

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Language[TranslationKeys.ReloadCustomTags]))
            {
                ReloadAllJSTags(CustomTagsPath);
                Recompile();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

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
