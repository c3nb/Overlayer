using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using GDMiniJSON;
using HarmonyLib;
using UnityModManagerNet;
using SA.GoogleDoc;
using System.Reflection;

namespace Overlayer.Core
{
    public static class Translate
    {
        public class _ : MonoBehaviour
        {
            static _ Runner
            {
                get
                {
                    if (!runner)
                    {
                        runner = new GameObject().AddComponent<_>();
                        DontDestroyOnLoad(runner.gameObject);
                        return runner;
                    }
                    return runner;
                }
            }
            static _ runner;
            public static Coroutine Run(IEnumerator coroutine) => Runner.StartCoroutine(coroutine);
        }
        private static readonly string SPREADSHEET_URL_START = "https://docs.google.com/spreadsheets/d/";
        private static readonly string SPREADSHEET_URL_END = "/gviz/tq?tqx=out:json&tq&gid=";
        private static readonly string KEY = "1CzaJzpqnVT_Ku3QdH0mPwyOkEbpa4e0BpcrC5mc_mjk";
        private static readonly int GID = 0;
        private static readonly string LOCALIZATION_PATH = Path.Combine("Mods", "Overlayer", "Translations.txt");
        private static readonly Dictionary<string, string> translations = new Dictionary<string, string>();

        private static bool Loaded = false;
        private static bool Patched = false;
        private static bool Downloaded = false;
        public static void Load()
        {
            Loaded = true;
            if (!Patched)
            {
                Main.Harmony.Patch(typeof(Main).GetMethod("OnGUI"),
                        prefix: new HarmonyMethod(typeof(Translate), "OnGUIPrefix"),
                        postfix: new HarmonyMethod(typeof(Translate), "OnGUIPostfix"));
                Main.Harmony.Patch(AccessTools.Method(typeof(GUILayout), "DoTextField"),
                    prefix: new HarmonyMethod(typeof(Translate), "DoTextFieldPrefix"),
                    postfix: new HarmonyMethod(typeof(Translate), "DoTextFieldPostfix"));
                Main.Harmony.Patch(AccessTools.Method(typeof(UnityModManager.UI), "PopupToggleGroup", new Type[] { typeof(int), typeof(string[]), typeof(Action<int>), typeof(string), typeof(int), typeof(GUIStyle), typeof(GUILayoutOption[]) }),
                    prefix: new HarmonyMethod(typeof(Translate), "PopupToggleGroupPrefix"));
                Main.Harmony.Patch(AccessTools.Method(typeof(GUIContent), "Temp", new Type[] { typeof(string) }),
                    prefix: new HarmonyMethod(typeof(Translate), "TempPrefix"));
                Patched = true;
            }
            if (!Downloaded)
            {
                Download().Run();
                Downloaded = true;
            }
        }
        public static void Unload() => Loaded = false;
        public static bool Get(string key, out string value)
        {
            if (translations.TryGetValue(key, out value))
            {
                if (key.StartsWith("*") && value.StartsWith("*") && key.EndsWith(key.Substring(1)))
                    value = key.Substring(0, key.Length - key.Length + 1) + value.Substring(1);
            }
            return !string.IsNullOrEmpty(value);
        }
        private static IEnumerator Download()
        {
            UnityWebRequest request = UnityWebRequest.Get(SPREADSHEET_URL_START + KEY + SPREADSHEET_URL_END + GID);
            yield return request.SendWebRequest();
            byte[] bytes = request.downloadHandler.data;
            if (bytes == null)
            {
                LoadFromFile();
                yield break;
            }
            string strData = Encoding.UTF8.GetString(bytes);
            strData = strData.Substring(47, strData.Length - 49);
            translations.Clear();
            StringBuilder sb = new StringBuilder();
            foreach (object obj in ((Json.Deserialize(strData) as Dictionary<string, object>)["table"] as Dictionary<string, object>)["rows"] as List<object>)
            {
                List<object> list = (obj as Dictionary<string, object>)["c"] as List<object>;
                string key = (list[0] as Dictionary<string, object>)["v"] as string;
                string value = (list[1] as Dictionary<string, object>)["v"] as string;
                if (key.IsNullOrEmpty() || value.IsNullOrEmpty())
                    continue;
                translations.Add(key, value);
                sb.AppendLine(key.Escape() + ":" + value.Escape());
            }
            File.WriteAllText(LOCALIZATION_PATH, sb.ToString());
            Main.Logger.Log($"Loaded {translations.Count} Localizations from Sheet");
        }
        private static void LoadFromFile()
        {
            if (File.Exists(LOCALIZATION_PATH))
            {
                string[] lines = File.ReadAllLines(LOCALIZATION_PATH);
                translations.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    int offset = 0;
                    while (true)
                    {
                        string sub = line.Substring(offset);
                        offset += sub.IndexOf(':');
                        if (offset <= 0 || line[offset - 1] != '\\')
                            break;
                        offset++;
                    }
                    if (offset == -1)
                    {
                        Main.Logger.Log($"Invalid Line in Localizations File! (line: {i + 1})");
                        continue;
                    }
                    string key = line.Substring(0, offset).Unescape();
                    string value = line.Substring(offset + 1).Unescape();
                    translations.Add(key, value);
                }
                Main.Logger.Log($"Loaded {translations.Count} Localizations from Local File");
                return;
            }
            Main.Logger.Log($"Couldn't Load Localizations!");
        }
        private static Coroutine Run(this IEnumerator coroutine) => _.Run(coroutine);
        private static string Escape(this string str) => str.Replace(@"\", @"\\").Replace(":", @"\:");
        private static string Unescape(this string str) => str.Replace(@"\:", ":").Replace(@"\\", @"\");
        private static bool patch = false;
        public static void OnGUIPrefix() => patch = true;
        public static void OnGUIPostfix() => patch = false;
        public static void DoTextFieldPrefix(out bool __state)
        {
            __state = patch;
            patch = false;
        }
        public static void DoTextFieldPostfix(bool __state) => patch = __state;
        public static void PopupToggleGroupPrefix(string[] values, ref string title)
        {
            if (!patch)
                return;
            for (int i = 0; i < values.Length; i++)
                values[i] = Get(values[i], out string v) ? v : values[i];
            title = Get(title, out string v2) ? v2 : title;
        }
        public static void TempPrefix(ref string t)
        {
            if (!Loaded) return;
            if (!patch) return;
            if (Get(t, out string value))
                t = value;
            else if (!check.Contains(t))
            {
                check.Add(t);
                Main.Logger.Log($"No Localization Found for Text '{t}'!");
            }
        }
        private static readonly List<string> check = new List<string>();
    }
}
