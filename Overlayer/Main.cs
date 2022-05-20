//#define TRACEALL
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;
using UnityEngine;
using Overlayer.KeyViewer;
using static UnityModManagerNet.UnityModManager;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Overlayer
{
    public static class Main
    {
        public static ModEntry Mod;
        public static ModEntry.ModLogger Logger;
        public static Harmony Harmony;
#if TRACEALL
        public static float timer;
        public static Queue<string> logs = new Queue<string>();
        public static List<long> runningTimes = new List<long>();
        public static List<MethodBase> methods = new List<MethodBase>();
        public static Dictionary<MethodBase, Stopwatch> watches = new Dictionary<MethodBase, Stopwatch>();
#endif
        public static float fpsTimer = 0;
        public static float lastDeltaTime;
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            modEntry.OnUpdate = (mod, deltaTime) =>
            {
                if (Input.anyKeyDown)
                    KpsTemp++;
                if (fpsTimer > Settings.Instance.FPSUpdateRate / 1000.0f)
                {
                    lastDeltaTime += (Time.unscaledDeltaTime - lastDeltaTime) * 0.1f;
                    Variables.Fps = 1.0f / lastDeltaTime;
                    fpsTimer = 0;
                }
                fpsTimer += deltaTime;
                KeyViewerTweaks.Instance.OnUpdate();
            };
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnHideGUI = mod => KeyViewerTweaks.Instance.OnHideGUI();
            try
            {
                var ue = Assembly.Load(System.IO.File.ReadAllBytes(System.IO.Path.Combine("Mods", "Overlayer", "UnityExplorer.STANDALONE.Mono.dll")));
                Method method = ue.GetType("UnityExplorer.ExplorerStandalone").GetMethod("CreateInstance", BindingFlags.Public | BindingFlags.Static, null, Type.EmptyTypes, null);
                method.AddPrefix(new Action(() => AppDomain.CurrentDomain.Load(System.IO.File.ReadAllBytes(System.IO.Path.Combine("Mods", "Overlayer", "UniverseLib.Mono.dll")))), true);
                method.Invoke(null);
            }
            catch { }
#if TRACEALL
            modEntry.OnUpdate = (mod, dt) =>
            {
                timer += dt;
                if (timer > 1)
                {
                    if (runningTimes.Any())
                    {
                        var max = runningTimes.Max();
                        var index = runningTimes.IndexOf(max);
                        var method = methods[index];
                        Logger.Log($"Invoked:{logs.Count}, Longest RunningTime is {method.DeclaringType}.{method.Name}:{max}ms");
                        runningTimes.Clear();
                        methods.Clear();
                    }
                    else Logger.Log($"Invoked:{logs.Count}");
                    logs.Clear();
                    timer = 0;
                }
            };
#endif
        }
        public static int KpsTemp;
        public static bool OnToggle(ModEntry modEntry, bool value)
        {
            try
            {
                if (value)
                {
                    Settings.Load(modEntry);
                    Variables.Reset();
                    Tag.AddTag("{CurKps}", "Current KPS").GetThis(out Tag kpsTag).MakeThreadingTag(() =>
                    {
                        LinkedList<int> timePoints = new LinkedList<int>();
                        int max = 0, prev = 0, total = 0;
                        long n = 0;
                        double avg = 0;
                        Stopwatch watch = Stopwatch.StartNew();
                        while (true)
                        {
                            if (watch.ElapsedMilliseconds >= Settings.Instance.KPSUpdateRate)
                            {
                                int temp = KpsTemp;
                                KpsTemp = 0;
                                int num = temp;
                                foreach (int i in timePoints)
                                    num += i;
                                max = Math.Max(num, max);
                                if (num != 0)
                                {
                                    avg = (avg * n + num) / (n + 1.0);
                                    n += 1L;
                                    total += temp;
                                }
                                prev = num;
                                timePoints.AddFirst(temp);
                                if (timePoints.Count >= 1000 / Settings.Instance.KPSUpdateRate)
                                    timePoints.RemoveLast();
                                kpsTag.ThreadValue = num;
                                watch.Restart();
                            }
                        }
                    });
                    Tag.Load();
                    OText.Load();
                    if (!OText.Texts.Any())
                        new OText().Apply();
                    KeyViewerTweaks.Instance.OnEnable();
                    Harmony = new Harmony(modEntry.Info.Id);
#if TRACEALL
                    MethodInfo log = typeof(Main).GetMethod("Log", (BindingFlags)15420, null, new[] { typeof(MethodBase) }, null);
                    MethodInfo logR = typeof(Main).GetMethod("Log", (BindingFlags)15420, null, new[] { typeof(MethodBase), typeof(object) }, null);
                    MethodInfo logPre = typeof(Main).GetMethod("LogPre", (BindingFlags)15420);
                    foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                    {
                        foreach (MethodInfo method in type.GetMethods((BindingFlags)15422))
                        {
                            if (Filter(method)) continue;
                            try
                            {
                                if (method.ReturnType != typeof(void))
                                    Harmony.Patch(method, new HarmonyMethod(logPre), new HarmonyMethod(logR));
                                else Harmony.Patch(method, new HarmonyMethod(logPre), new HarmonyMethod(log));
                                Logger.Log($"Patched {method}");
                            }
                            catch { }
                        }
                    }
#endif
                    Harmony.PatchAll(Assembly.GetExecutingAssembly());
                }
                else
                {
                    OnSaveGUI(modEntry);
                    OText.DestroyAll();
                    Tag.Tags.ForEach(t => t.Stop());
                    KeyViewerTweaks.Instance.OnDisable();
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
            if (Settings.Instance.IsKeyViewerEnabled = GUILayout.Toggle(Settings.Instance.IsKeyViewerEnabled, "KeyViewer"))
                KeyViewerTweaks.Instance.OnSettingsGUI();
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            Settings.Save(modEntry);
            Variables.Reset();
            OText.Save();
            Tag.Save();
        }
#if TRACEALL
        public static int Time(Action action)
        {
            Stopwatch sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.Elapsed.Milliseconds;
        }
        public static bool Filter(MethodInfo method)
        {
            if (!method.HasMethodBody()) return true;
            var decType = method.DeclaringType;
            if (decType == typeof(Main)) return true;
            var name = method.Name;
            if (name == "Update" || name == "LateUpdate" || name == "Prefix" || name == "Postfix") return true;
            //if (name.Contains("<") || name.Contains("set_") || name.Contains("get_") || name.Contains("Replace")) return true;
            return false;
        }
        public static void LogPre(MethodBase __originalMethod)
            => watches[__originalMethod] = Stopwatch.StartNew();
        public static void Log(MethodBase __originalMethod)
        {
            logs.Enqueue($"Invoked {__originalMethod.DeclaringType}.{__originalMethod.Name}");
            var watch = watches[__originalMethod];
            watch.Stop();
            methods.Add(__originalMethod);
            runningTimes.Add(watch.ElapsedMilliseconds);
        }
        public static void Log(MethodBase __originalMethod, object __result)
        {
            logs.Enqueue($"Invoked {__originalMethod.DeclaringType}.{__originalMethod.Name} With Result:{__result}");
            var watch = watches[__originalMethod];
            watch.Stop();
            methods.Add(__originalMethod);
            runningTimes.Add(watch.ElapsedMilliseconds);
        }
#endif
    }
}
