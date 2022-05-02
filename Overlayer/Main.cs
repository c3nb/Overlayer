using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace Overlayer
{
    public static class Main
    {
        public static ModEntry Mod;
        public static ModEntry.ModLogger Logger;
        public static Harmony Harmony;
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            modEntry.OnUpdate = (mod, deltaTime) =>
            {
                if (Input.anyKeyDown)
                    KpsTemp++;
            };
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
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
                            if (watch.ElapsedMilliseconds >= Settings.Instance.UpdateRate)
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
                                if (timePoints.Count >= 1000 / Settings.Instance.UpdateRate)
                                    timePoints.RemoveLast();
                                kpsTag.ThreadValue = num;
                                watch.Restart();
                            }
                        }
                    });
                    Tag.Load();
                    Text.Load();
                    if (!Text.Texts.Any())
                        new Text().Apply();
                    Harmony = new Harmony(modEntry.Info.Id);
                    Harmony.PatchAll(Assembly.GetExecutingAssembly());
                }
                else
                {
                    OnSaveGUI(modEntry);
                    Text.DestroyAll();
                    Tag.Tags.ForEach(t => t.Stop());
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
                new Text().Apply();
                Text.Order();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            Tag.CustomTagGUI();
            for (int i = 0; i < Text.Texts.Count; i++)
                Text.Texts[i].GUI();
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            Settings.Save(modEntry);
            Variables.Reset();
            Text.Save();
            Tag.Save();
        }
    }
}
