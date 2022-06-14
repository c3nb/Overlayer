//#define TRACEALL
//#define KV
using System;
using System.Linq;
using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using Stopwatch = System.Diagnostics.Stopwatch;
using UnityEngine.SceneManagement;

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
        public static int talmo = 0;
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
#if DEBUG
                    SceneManager.sceneLoaded += (s, m) => AdofaiDebugger.sceneCache = s.name;
#endif
                    Settings.Load(modEntry);
                    Variables.Reset();
                    Tag.Init();
                    Tag.Load();
                    OText.Load();
                    if (!OText.Texts.Any())
                        new OText().Apply();
                    Harmony = new Harmony(modEntry.Info.Id);
                    Harmony.PatchAll(Assembly.GetExecutingAssembly());
                }
                else
                {
                    OnSaveGUI(modEntry);
                    OText.Texts.ForEach(o => UnityEngine.Object.Destroy(o.SText.gameObject));
                    OText.Texts.Clear();
                    Tag.Tags.ForEach(t => t.Stop());
                    Tag.Tags.Clear();
                    Harmony.UnpatchAll(Harmony.Id);
                    Harmony = null;
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
#if DEBUG
            AdofaiDebugger.DebugGUI();
#endif
            Settings.Instance.Draw(modEntry);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Text"))
            {
                new OText().Apply();
                OText.Order();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            Tag.CustomTagGUI();
            for (int i = 0; i < OText.Texts.Count; i++)
                OText.Texts[i].GUI();
            Tag.DescGUI();
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            Settings.Save(modEntry);
            Variables.Reset();
            OText.Save();
            Tag.Save();
        }
    }
}
