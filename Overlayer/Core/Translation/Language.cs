using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine.Networking;
using GDMiniJSON;

namespace Overlayer.Core.Translation
{
    public class Language
    {
        //V2
        //public static string GetUrl(int gid) =>
        //    $"https://docs.google.com/spreadsheets/d/1es4yyxCxR88VuouFOwED9VM7PqtP2lTaGl8m5FS8F50/gviz/tq?tqx=out:json&tq&gid={gid}";
        //V1
        public static string GetUrl(int gid) =>
            $"https://docs.google.com/spreadsheets/d/1CzaJzpqnVT_Ku3QdH0mPwyOkEbpa4e0BpcrC5mc_mjk/gviz/tq?tqx=out:json&tq&gid={gid}";
        public static readonly Language Korean = new Language(GID.KOREAN);
        public static readonly Language English = new Language(GID.ENGLISH);
        public static readonly Language Japanese = new Language(GID.JAPANESE);
        public static readonly Language Chinese = new Language(GID.CHINESE);
        public readonly string url;
        public Dictionary<string, string> dict { get; private set; }
        public readonly string path;
        public GID gid;
        public bool Initialized { get; private set; }
        public Language(GID gid)
        {
            this.gid = gid;
            url = GetUrl((int)gid);
            dict = new Dictionary<string, string>();
            path = Path.Combine(Main.Mod.Path, $"Translations_{gid}.txt");
            StaticCoroutine.Run(Download());
        }
        public string this[string key]
        {
            get
            {
                if (dict.TryGetValue(key, out string value))
                    return value;
                return key;
            }
            set => dict[key] = value;
        }
        IEnumerator Download()
        {
            if (Initialized)
                yield break;
            LoadFromFile();
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            var dict = new Dictionary<string, string>();
            byte[] bytes = request.downloadHandler.data;
            if (bytes == null)
            {
                Initialized = true;
                yield break;
            }
            string strData = Encoding.UTF8.GetString(bytes);
            strData = strData.Substring(47, strData.Length - 49);
            StringBuilder sb = new StringBuilder();
            foreach (object obj in ((Json.Deserialize(strData) as Dictionary<string, object>)["table"] as Dictionary<string, object>)["rows"] as List<object>)
            {
                List<object> list = (obj as Dictionary<string, object>)["c"] as List<object>;
                string key = (list[0] as Dictionary<string, object>)["v"] as string;
                string value = (list[1] as Dictionary<string, object>)["v"] as string;
                if (key.IsNullOrEmpty() || value.IsNullOrEmpty())
                    continue;
                dict.Add(key, value);
                sb.AppendLine(key.Escape() + ":" + value.Escape());
            }
            if (!this.dict.SequenceEqual(dict))
                this.dict = dict;
            else
            {
                Initialized = true;
                yield break;
            }
            File.WriteAllText(path, sb.ToString());
            Main.Logger.Log($"Loaded {dict.Count} Localizations from Sheet");
            Initialized = true;
        }
        bool LoadFromFile()
        {
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                dict.Clear();
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
                    dict.Add(key, value);
                }
                Main.Logger.Log($"Loaded {dict.Count} Localizations from Local File");
                return true;
            }
            Main.Logger.Log($"Couldn't Load Localizations!");
            return false;
        }
    }
}
