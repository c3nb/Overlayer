using System;
using System.Collections.Generic;
using TinyJson;
using System.IO;
using System.Threading;
using UnityEngine;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Overlayer
{
    public class Tag
    {
        public class Setting
        {
            public string name;
            public string op;
            public string[] values;
        }
        static Tag()
        {
            Tags = new List<Tag>()
            {
                new Tag("{LHit}", "HitMargin in Lenient Difficulty").SetValueGetter(() => RDString.Get("HitMargin." + Variables.Lenient)),
                new Tag("{NHit}", "HitMargin in Normal Difficulty").SetValueGetter(() => RDString.Get("HitMargin." + Variables.Normal)),
                new Tag("{SHit}", "HitMargin in Strict Difficulty").SetValueGetter(() => RDString.Get("HitMargin." + Variables.Strict)),

                new Tag("{LTE}", "TooEarly in Lenient Difficulty").SetValueGetter(() => Variables.LenientCounts[HitMargin.TooEarly]),
                new Tag("{LVE}", "VeryEarly in Lenient Difficulty").SetValueGetter(() => Variables.LenientCounts[HitMargin.VeryEarly]),
                new Tag("{LEP}", "EarlyPerfect in Lenient Difficulty").SetValueGetter(() => Variables.LenientCounts[HitMargin.EarlyPerfect]),
                new Tag("{LP}", "Perfect in Lenient Difficulty").SetValueGetter(() => Variables.LenientCounts[HitMargin.Perfect]),
                new Tag("{LLP}", "LatePerfect in Lenient Difficulty").SetValueGetter(() => Variables.LenientCounts[HitMargin.LatePerfect]),
                new Tag("{LVL}", "VeryLate in Lenient Difficulty").SetValueGetter(() => Variables.LenientCounts[HitMargin.VeryLate]),
                new Tag("{LTL}", "TooLate in Lenient Difficulty").SetValueGetter(() => Variables.LenientCounts[HitMargin.TooLate]),

                new Tag("{NTE}", "TooEarly in Normal Difficulty").SetValueGetter(() => Variables.NormalCounts[HitMargin.TooEarly]),
                new Tag("{NVE}", "VeryEarly in Normal Difficulty").SetValueGetter(() => Variables.NormalCounts[HitMargin.VeryEarly]),
                new Tag("{NEP}", "EarlyPerfect in Normal Difficulty").SetValueGetter(() => Variables.NormalCounts[HitMargin.EarlyPerfect]),
                new Tag("{NP}", "Perfect in Normal Difficulty").SetValueGetter(() => Variables.NormalCounts[HitMargin.Perfect]),
                new Tag("{NLP}", "LatePerfect in Normal Difficulty").SetValueGetter(() => Variables.NormalCounts[HitMargin.LatePerfect]),
                new Tag("{NVL}", "VeryLate in Normal Difficulty").SetValueGetter(() => Variables.NormalCounts[HitMargin.VeryLate]),
                new Tag("{NTL}", "TooLate in Normal Difficulty").SetValueGetter(() => Variables.NormalCounts[HitMargin.TooLate]),

                new Tag("{STE}", "TooEarly in Strict Difficulty").SetValueGetter(() => Variables.StrictCounts[HitMargin.TooEarly]),
                new Tag("{SVE}", "VeryEarly in Strict Difficulty").SetValueGetter(() => Variables.StrictCounts[HitMargin.VeryEarly]),
                new Tag("{SEP}", "EarlyPerfect in Strict Difficulty").SetValueGetter(() => Variables.StrictCounts[HitMargin.EarlyPerfect]),
                new Tag("{SP}", "Perfect in Strict Difficulty").SetValueGetter(() => Variables.StrictCounts[HitMargin.Perfect]),
                new Tag("{SLP}", "LatePerfect in Strict Difficulty").SetValueGetter(() => Variables.StrictCounts[HitMargin.LatePerfect]),
                new Tag("{SVL}", "VeryLate in Strict Difficulty").SetValueGetter(() => Variables.StrictCounts[HitMargin.VeryLate]),
                new Tag("{STL}", "TooLate in Strict Difficulty").SetValueGetter(() => Variables.StrictCounts[HitMargin.TooLate]),

                new Tag("{Score}", "Score in Current Difficulty").SetValueGetter(() => Variables.CurrentScore),
                new Tag("{Combo}", "Combo").SetValueGetter(() => Variables.Combo),
                new Tag("{LScore}", "Score in Lenient Difficulty").SetValueGetter(() => Variables.LenientScore),
                new Tag("{NScore}", "Score in Normal Difficulty").SetValueGetter(() => Variables.NormalScore),
                new Tag("{SScore}", "Score in Strict Difficulty").SetValueGetter(() => Variables.StrictScore),

                new Tag("{CurTE}", "TooEarly in Current Difficulty").SetValueGetter(() => Utils.GetCurDiffCount(HitMargin.TooEarly)),
                new Tag("{CurVE}", "VeryEarly in Current Difficulty").SetValueGetter(() => Utils.GetCurDiffCount(HitMargin.VeryEarly)),
                new Tag("{CurEP}", "EarlyPerfect in Current Difficulty").SetValueGetter(() => Utils.GetCurDiffCount(HitMargin.EarlyPerfect)),
                new Tag("{CurP}", "Perfect in Current Difficulty").SetValueGetter(() => Utils.GetCurDiffCount(HitMargin.Perfect)),
                new Tag("{CurLP}", "LatePerfect in Current Difficulty").SetValueGetter(() => Utils.GetCurDiffCount(HitMargin.LatePerfect)),
                new Tag("{CurVL}", "VeryLate in Current Difficulty").SetValueGetter(() => Utils.GetCurDiffCount(HitMargin.VeryLate)),
                new Tag("{CurTL}", "TooLate in Current Difficulty").SetValueGetter(() => Utils.GetCurDiffCount(HitMargin.TooLate)),

                new Tag("{CurHit}", "HitMargin in Current Difficulty").SetValueGetter(() => RDString.Get("HitMargin." + Utils.GetCurHitMargin(GCS.difficulty))),
                new Tag("{CurDifficulty}", "Current Difficulty").SetValueGetter(() => RDString.Get("enum.Difficulty." + GCS.difficulty)),
                new Tag("{Accuracy}", "Accuracy").SetValueGetter(() => $"{Math.Round(scrController.instance.mistakesManager.percentAcc * 100, Settings.Instance.AccuracyDecimals)}"),
                new Tag("{Progress}", "Progress").SetValueGetter(() => $"{Math.Round((!scrController.instance.lm ? 0 : scrController.instance.percentComplete) * 100f, Settings.Instance.ProgressDecimals)}"),
                new Tag("{CheckPoint}", "Check Point Used Count").SetValueGetter(() => scrController.instance.customLevel.checkpointsUsed),
                new Tag("{Timing}", "Hit Timing").SetValueGetter(() => Variables.Timing),
                new Tag("{XAccuracy}", "XAccuracy" ).SetValueGetter(() => $"{Math.Round(scrController.instance.mistakesManager.percentXAcc * 100, Settings.Instance.XAccuracyDecimals)}".Replace("NaN", "100")),
                new Tag("{FailCount}", "Fail Count" ).SetValueGetter(() => Variables.FailCount),
                new Tag("{MissCount}", "Miss Count" ).SetValueGetter(() => scrMistakesManager.hitMarginsCount[8]),
                new Tag("{Overloads}", "Overload Count").SetValueGetter(() => scrMistakesManager.hitMarginsCount[9]),
                new Tag("{CurBpm}", "Perceived Bpm").SetValueGetter(() => Variables.CurBpm),
                new Tag("{TileBpm}", "Tile Bpm").SetValueGetter(() => Variables.TileBpm),
                new Tag("{RecKps}", "Perceived KPS").SetValueGetter(() => Variables.RecKPS),
                new Tag("{BestProgress}", "Best Progress").SetValueGetter(() => Math.Round(Variables.BestProg, Settings.Instance.BestProgDecimals)),
                new Tag("{LeastCheckPoint}", "Least Check Point Used Count").SetValueGetter(() => Variables.LeastChkPt),
                new Tag("{StartProgress}", "Start Progress (Do Not Use..)").SetValueGetter(() => Math.Round(Variables.StartProg, Settings.Instance.StartProgDecimals)),
                new Tag("{CurMinute}", "Now Minute Of Music").SetValueGetter(() => Variables.CurMinute),
                new Tag("{CurSecond}", "Now Second Of Music").SetValueGetter(() => Variables.CurSecond),
                new Tag("{TotalMinute}", "Total Minute Of Music").SetValueGetter(() => Variables.TotalMinute),
                new Tag("{TotalSecond}", "Total Second Of Music").SetValueGetter(() => Variables.TotalSecond),
                new Tag("{LeftTile}", "Left Tile Count").SetValueGetter(() => Variables.LeftTile),
                new Tag("{TotalTile}", "Total Tile Count").SetValueGetter(() => Variables.TotalTile),
                new Tag("{CurTile}", "Current Tile Count").SetValueGetter(() => Variables.CurrentTile),
                new Tag("{Attempts}", "Current Level Try Count").SetValueGetter(() => Variables.Attempts),
                new Tag("{Year}", "Year Of System Time").SetValueGetter(() => DT.Now.Year),
                new Tag("{Month}", "Month Of System Time").SetValueGetter(() => DT.Now.Month),
                new Tag("{Day}", "Day Of System Time").SetValueGetter(() => DT.Now.Day),
                new Tag("{Hour}", "Hour Of System Time").SetValueGetter(() => DT.Now.Hour),
                new Tag("{Minute}", "Minute Of System Time").SetValueGetter(() => DT.Now.Minute),
                new Tag("{Second}", "Second Of System Time").SetValueGetter(() => DT.Now.Second),
                new Tag("{MilliSecond}", "MilliSecond Of System Time").SetValueGetter(() => DT.Now.Millisecond),
                new Tag("{Fps}", "FrameRate").SetValueGetter(() => Math.Round(Variables.Fps, Settings.Instance.FPSDecimals)),
            };
            NameTags = new Dictionary<string, Tag>();
            CustomSettings = new List<Setting>();
            for (int i = 0; i < Tags.Count; i++)
                NameTags.Add(Tags[i].Name, Tags[i]);
            NPTags = new List<Tag>
            {
                NameTags["{Year}"],
                NameTags["{Month}"],
                NameTags["{Day}"],
                NameTags["{Hour}"],
                NameTags["{Minute}"],
                NameTags["{Second}"],
                NameTags["{MilliSecond}"],
                NameTags["{Fps}"],
            };
        }
        Tag(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public static Tag AddTag(string name, string description)
        {
            Tag tag = new Tag(name, description);
            Tags.Add(tag);
            NameTags.Add(name, tag);
            return tag;
        }
        public static bool TagDesc = false;
        public static void DescGUI()
        {
            GUILayout.BeginHorizontal();
            if (TagDesc = GUILayout.Toggle(TagDesc, "Tags"))
            {
                Utils.IndentGUI(() =>
                {
                    for (int i = 0; i < Tags.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(Tags[i].ToString());
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(1);
                    }
                }, 3f);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public readonly string Name;
        public readonly string Description;
        public bool IsThreading { get; private set; }
        private Thread MainThread;
        private List<Thread> SubThreads;
        public string ThreadValueStr = string.Empty;
        public double ThreadValueNum = 0;
        public bool IsCustom { get; private set; }
        public Func<double> NumberGetter = () => 0;
        public Func<string> StrGetter = () => "ValueGetter Is Not Implemented!";
        public bool IsString = false;
        public Tag SetValueGetter(Func<double> func)
        {
            if (IsThreading) throw new InvalidOperationException("Threading Tag Cannot Set ValueGetter!");
            NumberGetter = func;
            IsString = false;
            return this;
        }
        public Tag SetValueGetter(Func<int> func) => SetValueGetter(() => (double)func());
        public Tag SetValueGetter(Func<string> func)
        {
            if (IsThreading) throw new InvalidOperationException("Threading Tag Cannot Set ValueGetter!");
            StrGetter = func;
            IsString = true;
            return this;
        }
        public Tag MakeThreadingTag(Action func, bool number = true)
        {
            if (IsThreading)
            {
                MainThread.Abort();
                MainThread = new Thread(() => func());
                MainThread.Start();
                return this;
            }
            MainThread = new Thread(() => func());
            if (number)
                SetValueGetter(() => ThreadValueNum);
            else SetValueGetter(() => ThreadValueStr);
            MainThread.Start();
            IsThreading = true;
            return this;
        }
        public Tag MakeSubThread(Action func, bool ignoreThreadingTag = false)
        {
            if (!ignoreThreadingTag && !IsThreading) throw new InvalidOperationException("Normal Tag Cannot Make SubThread!");
            if (SubThreads == null) SubThreads = new List<Thread>();
            Thread subThread = new Thread(() => func());
            subThread.Start();
            SubThreads.Add(subThread);
            return this;
        }
        public string Value
        {
            get
            {
                if (IsString)
                    return StrGetter();
                else return NumberGetter().ToString(); 
            }
        }
        public void Stop()
        {
            if (!IsThreading) return;
            MainThread.Abort();
            SubThreads?.ForEach(t => t.Abort());
        }
        public Tag SetCustomTag()
        {
            IsCustom = true;
            return this;
        }
        public override string ToString() => $"{Name}:{Description}";
        public static string Replace(string text)
        {
            for (int i = 0; i < Tags.Count; i++)
            {
                Tag tag = Tags[i];
                if (text.Contains(tag.Name))
                    text = text.Replace(tag.Name, tag.Value);
            }
            return text;
        }
        public static string NPReplace(string text)
        {
            for (int i = 0; i < NPTags.Count; i++)
            {
                Tag tag = NPTags[i];
                if (text.Contains(tag.Name))
                    text = text.Replace(tag.Name, tag.Value);
            }
            return text;
        }
        public static string NameCache = string.Empty;
        public static string[] TagsCache = new string[2];
        public static string OpCache = "+";
        public Setting CustomSetting;
        
        public static void CustomTagGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:");
            NameCache = GUILayout.TextField(NameCache);
            GUILayout.Label(", Value:");
            TagsCache[0] = GUILayout.TextField(TagsCache[0]);
            GUILayout.Space(1);
            OpCache = GUILayout.TextField(OpCache);
            GUILayout.Space(1);
            TagsCache[1] = GUILayout.TextField(TagsCache[1]);
            if (GUILayout.Button("Create Custom Tag"))
                CreateCustomTag();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            for (int i = 0; i < Tags.Count; i++)
                Tags[i].GUI();
        }
        public void GUI()
        {
            if (!IsCustom) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Name:{Name}, Value:{CustomSetting.values[0]}{CustomSetting.op}{CustomSetting.values[1]}");
            if (GUILayout.Button("Remove"))
                RemoveCustomTag(Name);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static void CreateCustomTag(Setting setting = null)
        {
            setting = setting ?? new Setting
            {
                values = Utils.CopyArray(TagsCache),
                op = OpCache,
                name = NameCache
            };
            Tag tag = new Tag(setting.name, null)
            {
                IsCustom = true
            };
            tag.CustomSetting = setting;
            tag.SetValueGetter(() => Utils.ComputeTagValue(setting.values[0], setting.values[1], setting.op));
            CustomSettings.Add(setting);
            Tags.Add(tag);
            NameTags.Add(setting.name, tag);
        }
        public static void RemoveCustomTag(string name)
        {
            var tag = NameTags[name];
            NameTags.Remove(name);
            CustomSettings.Remove(tag.CustomSetting);
            Tags.Remove(tag);
        }
        public static readonly string JsonPath = Path.Combine("Mods", "Overlayer", "CustomTags.json");
        public static void Load()
        {
            if (File.Exists(JsonPath))
            {
                var settings = File.ReadAllText(JsonPath).FromJson<List<Setting>>();
                foreach (Setting setting in settings)
                    CreateCustomTag(setting);
            }
        }
        public static void Save()
        {
            if (CustomSettings.Any())
            File.WriteAllText(JsonPath, CustomSettings.ToJson());
            else if (File.Exists(JsonPath))
            File.Delete(JsonPath);
        }
        public static readonly List<Tag> Tags;
        public static readonly List<Tag> NPTags;
        public static List<Setting> CustomSettings;
        public static readonly Dictionary<string, Tag> NameTags;
    }
    public class TagCompiler
    {
        public string source;
        StringBuilder sb;
        List<Func<object>> getters;
        public TagCompiler(string source)
            => Compile(source);
        public void Compile(string source)
        {
            this.source = source;
            sb = new StringBuilder(source.Length);
            StringBuilder tagSb = new StringBuilder();
            getters = new List<Func<object>>();
            bool tagMode = false;
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (tagMode)
                {
                    if (c == '{')
                    {
                        sb.Append(tagSb);
                        tagSb.Clear();
                        tagSb.Append(c);
                    }
                    else if (c == '}')
                    {
                        tagSb.Append(c);
                        if (Tag.NameTags.TryGetValue(tagSb.ToString(), out Tag tag))
                        {
                            string jesus = sb.ToString();
                            getters.Add(() => jesus);
                            sb.Clear();
                            if (!tag.IsString)
                                getters.Add(() => tag.NumberGetter());
                            else getters.Add(() => tag.StrGetter());
                        }
                        else sb.Append(tagSb);
                        tagSb.Clear();
                        tagMode = false;
                    }
                    else tagSb.Append(c);
                }
                else
                {
                    if (c == '{')
                    {
                        tagMode = true;
                        tagSb.Append(c);
                    }
                    else sb.Append(c);
                }
            }
            if (sb.Length > 0)
            {
                string jesus = sb.ToString();
                getters.Add(() => jesus);
            }
        }
        public string Value
        {
            get
            {
                sb.Clear();
                for (int i = 0; i < getters.Count; i++)
                    sb.Append(getters[i]());
                return sb.ToString();
            }
        }
    }
}
