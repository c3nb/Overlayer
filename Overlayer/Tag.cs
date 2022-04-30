using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Overlayer
{
    public class Tag
    {
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

                new Tag("{CurHit}", "HitMargin in Current Difficulty").SetValueGetter(() => RDString.Get("HitMargin." + Utils.GetCurHitMargin(GCS.difficulty))).SetUpdateOnlyPlaying(),
                new Tag("{CurDifficulty}", "Current Difficulty").SetValueGetter(() => RDString.Get("enum.Difficulty." + GCS.difficulty)).SetUpdateOnlyPlaying(),
                new Tag("{Accuracy}", "Accuracy").SetValueGetter(() => $"{Math.Round(scrController.instance.mistakesManager.percentAcc * 100, Settings.Instance.AccuracyDecimals)}").SetUpdateOnlyPlaying(),
                new Tag("{Progress}", "Progress").SetValueGetter(() => $"{Math.Round((!scrController.instance.lm ? 0 : scrController.instance.percentComplete) * 100f, Settings.Instance.ProgressDecimals)}").SetUpdateOnlyPlaying(),
                new Tag("{CheckPoint}", "Check Point Used Count").SetValueGetter(() => scrController.instance.customLevel?.checkpointsUsed).SetUpdateOnlyPlaying(),
                new Tag("{Timing}", "Hit Timing").SetValueGetter(() => Variables.Timing).SetUpdateOnlyPlaying(),
                new Tag("{XAccuracy}", "XAccuracy" ).SetValueGetter(() => $"{Math.Round(scrController.instance.mistakesManager.percentXAcc * 100, Settings.Instance.XAccuracyDecimals)}".Replace("NaN", "100")).SetUpdateOnlyPlaying(),
                new Tag("{FailCount}", "Fail Count" ).SetValueGetter(() => Variables.FailCount).SetUpdateOnlyPlaying(),
                new Tag("{MissCount}", "Miss Count" ).SetValueGetter(() => scrMistakesManager.hitMarginsCount[8]).SetUpdateOnlyPlaying(),
                new Tag("{Overloads}", "Overload Count").SetValueGetter(() => scrMistakesManager.hitMarginsCount[9]).SetUpdateOnlyPlaying(),
                new Tag("{CurBpm}", "Perceived Bpm").SetValueGetter(() => Variables.CurBpm).SetUpdateOnlyPlaying(),
                new Tag("{TileBpm}", "Tile Bpm").SetValueGetter(() => Variables.TileBpm).SetUpdateOnlyPlaying(),
                new Tag("{RecKps}", "Perceived KPS").SetValueGetter(() => Variables.RecKPS).SetUpdateOnlyPlaying(),
                new Tag("{BestProgress}", "Best Progress").SetValueGetter(() => Math.Round(Variables.BestProg, Settings.Instance.BestProgDecimals)).SetUpdateOnlyPlaying(),
                new Tag("{LeastCheckPoint}", "Least Check Point Used Count").SetValueGetter(() => Variables.LeastChkPt).SetUpdateOnlyPlaying(),
                new Tag("{StartProgress}", "Start Progress").SetValueGetter(() => Math.Round(Variables.StartProg, Settings.Instance.StartProgDecimals)).SetUpdateOnlyPlaying(),

            };
            NameTags = new Dictionary<string, Tag>();
            for (int i = 0; i < Tags.Count; i++)
            {
                Tag tag = Tags[i];
                NameTags.Add(tag.Name, tag);
            }
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
            NameTags.Add(tag.Name, tag);
            return tag;
        }
        public readonly string Name;
        public readonly string Description;
        public bool UpdateOnlyPlaying { get; private set; }
        public bool IsThreading { get; private set; }
        private Thread MainThread;
        private List<Thread> SubThreads;
        public object ThreadValue = string.Empty;
        private Func<object> ValueGetter = () => "ValueGetter Is Not Implemented!";
        public Tag SetUpdateOnlyPlaying()
        {
            UpdateOnlyPlaying = true;
            return this;
        }
        public Tag SetValueGetter(Func<object> func)
        {
            if (IsThreading) throw new InvalidOperationException("Threading Tag Cannot Set ValueGetter!");
            ValueGetter = func;
            return this;
        }
        public Tag MakeThreadingTag(Action func)
        {
            if (IsThreading)
            {
                MainThread.Abort();
                MainThread = new Thread(() => func());
                MainThread.Start();
                return this;
            }
            MainThread = new Thread(() => func());
            ValueGetter = () => ThreadValue;
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
        public Tag GetThis(out Tag @this)
        {
            @this = this;
            return this;
        }
        public string Value
        {
            get
            {
                if (UpdateOnlyPlaying)
                {
                    if (Text.IsPlaying)
                        return ValueGetter().ToString();
                    else return string.Empty;
                }
                else return ValueGetter().ToString();
            }
        }
        public void Stop()
        {
            if (!IsThreading) return;
            MainThread.Abort();
            SubThreads?.ForEach(t => t.Abort());
        }
        public override string ToString() => $"{Name}:{Description}";
        public static string Replace(string text)
        {
            Tags.ForEach(t => text = text.Replace(t.Name, t.Value));
            return text;
        }
        public readonly static Dictionary<string, Tag> NameTags;
        public readonly static List<Tag> Tags;
    }
}
