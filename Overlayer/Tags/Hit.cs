using Overlayer.Tags.Attributes;
using Overlayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overlayer.Tags
{
    public static class Hit
    {
        [Tag("LHitRaw")]
        public static HitMargin Lenient;
        [Tag("NHitRaw")]
        public static HitMargin Normal;
        [Tag("SHitRaw")]
        public static HitMargin Strict;
        [Tag("CHitRaw")]
        public static HitMargin Current;
        [Tag]
        public static string LHit(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => RDString.Get("HitMargin." + Lenient).Trim(maxLength, afterTrimStr);
        [Tag]
        public static string NHit(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => RDString.Get("HitMargin." + Normal).Trim(maxLength, afterTrimStr);
        [Tag]
        public static string SHit(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => RDString.Get("HitMargin." + Strict).Trim(maxLength, afterTrimStr);
        [Tag]
        public static string CHit(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => RDString.Get("HitMargin." + Current).Trim(maxLength, afterTrimStr);
        [Tag]
        public static double LTE, LVE, LEP, LP, LLP, LVL, LTL;
        [Tag]
        public static double NTE, NVE, NEP, NP, NLP, NVL, NTL;
        [Tag]
        public static double STE, SVE, SEP, SP, SLP, SVL, STL;
        [Tag]
        public static double CTE, CVE, CEP, CP, CLP, CVL, CTL;
        [Tag]
        public static double LFast() => LTE + LVE + LEP;
        [Tag]
        public static double NFast() => NTE + NVE + NEP;
        [Tag]
        public static double SFast() => STE + SVE + SEP;
        [Tag]
        public static double CFast() => CTE + CVE + CEP;
        [Tag]
        public static double LSlow() => LTL + LVL + LLP;
        [Tag]
        public static double NSlow() => NTL + NVL + NLP;
        [Tag]
        public static double SSlow() => STL + SVL + SLP;
        [Tag]
        public static double CSlow() => CTL + CVL + CLP;
        [Tag]
        public static double MissCount() => scrController.instance?.mistakesManager?.GetHits(HitMargin.FailMiss) ?? 0;
        [Tag]
        public static double Overloads() => scrController.instance?.mistakesManager?.GetHits(HitMargin.FailOverload) ?? 0;
        [Tag]
        public static double Multipress;
        [Tag]
        public static string Difficulty(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => RDString.Get("enum.Difficulty." + GCS.difficulty).Trim(maxLength, afterTrimStr);
        [Tag]
        public static string DifficultyStr(int maxLength = -1, string afterTrimStr = Extensions.DefaultTrimStr) => GCS.difficulty.ToString().Trim(maxLength, afterTrimStr);
        #region MarginCombos
        [Tag]
        public static int LMarginCombos(string margins) => MarginCombos_Internal(global::Difficulty.Lenient, margins);
        [Tag]
        public static int NMarginCombos(string margins) => MarginCombos_Internal(global::Difficulty.Normal, margins);
        [Tag]
        public static int SMarginCombos(string margins) => MarginCombos_Internal(global::Difficulty.Strict, margins);
        [Tag]
        public static int MarginCombos(string margins) => MarginCombos_Internal(GCS.difficulty, margins);
        public static int MarginCombos_Internal(Difficulty diff, string margins)
        {
            var hms = margins.SplitParse<HitMargin>('|');
            int hash = ADOUtils.HashMargins(hms);
            if (!MMaxComboCache.TryGetValue(hash, out _))
                MMaxComboCache[hash] = new int[EnumHelper<Difficulty>.GetValues().Length];
            if (!MComboCache.TryGetValue(hash, out int[] combos))
                combos = MComboCache[hash] = new int[EnumHelper<Difficulty>.GetValues().Length];
            return combos[(int)diff];
        }
        #endregion
        #region MarginMaxCombos
        [Tag]
        public static int LMarginMaxCombos(string margins) => MarginMaxCombos_Internal(global::Difficulty.Lenient, margins);
        [Tag]
        public static int NMarginMaxCombos(string margins) => MarginMaxCombos_Internal(global::Difficulty.Normal, margins);
        [Tag]
        public static int SMarginMaxCombos(string margins) => MarginMaxCombos_Internal(global::Difficulty.Strict, margins);
        [Tag]
        public static int MarginMaxCombos(string margins) => MarginMaxCombos_Internal(GCS.difficulty, margins);
        public static int MarginMaxCombos_Internal(Difficulty diff, string margins)
        {
            var hms = margins.SplitParse<HitMargin>('|');
            int hash = ADOUtils.HashMargins(hms);
            if (!MComboCache.TryGetValue(hash, out _))
                MComboCache[hash] = new int[EnumHelper<Difficulty>.GetValues().Length];
            if (!MMaxComboCache.TryGetValue(hash, out int[] combos))
                combos = MMaxComboCache[hash] = new int[EnumHelper<Difficulty>.GetValues().Length];
            return combos[(int)diff];
        }
        #endregion
        public static Dictionary<int, int[]> MComboCache = new Dictionary<int, int[]>();
        public static Dictionary<int, int[]> MMaxComboCache = new Dictionary<int, int[]>();
        public static void Reset()
        {
            Lenient = Normal = Strict = Current = HitMargin.Perfect;
            LTE = LVE = LEP = LP = LLP = LVL = LTL = 0;
            NTE = NVE = NEP = NP = NLP = NVL = NTL = 0;
            STE = SVE = SEP = SP = SLP = SVL = STL = 0;
            CTE = CVE = CEP = CP = CLP = CVL = CTL = 0;
            Multipress = 0;
            MComboCache.Clear();
            MMaxComboCache.Clear();
        }
        public static void SetMarginCombos()
        {
            foreach (int hash in MComboCache.Keys.ToList())
            {
                var hms = ADOUtils.UnboxMarginHash(hash);
                var combos = MComboCache[hash];
                var maxCombos = MMaxComboCache[hash];
                foreach (var diff in EnumHelper<Difficulty>.GetValues())
                {
                    var difference = GetCHit(diff);
                    if (Array.IndexOf(hms, difference) >= 0)
                        maxCombos[(int)diff] = Math.Max(maxCombos[(int)diff], ++combos[(int)diff]);
                    else combos[(int)diff] = 0;
                }
            }
        }
        public static HitMargin GetCHit(Difficulty diff)
        {
            switch (diff)
            {
                case global::Difficulty.Lenient: return Lenient;
                case global::Difficulty.Normal: return Normal;
                case global::Difficulty.Strict: return Strict;
                default: return Strict;
            }
        }
        public static double GetHitCount(Difficulty diff, HitMargin margin)
        {
            switch (diff)
            {
                case global::Difficulty.Lenient:
                    switch (margin)
                    {
                        case HitMargin.TooEarly: return LTE;
                        case HitMargin.VeryEarly: return LVE;
                        case HitMargin.EarlyPerfect: return LEP;
                        case HitMargin.Perfect: return LP;
                        case HitMargin.LatePerfect: return LLP;
                        case HitMargin.VeryLate: return LVL;
                        case HitMargin.TooLate: return LTL;
                        default: return 0;
                    }
                case global::Difficulty.Normal:
                    switch (margin)
                    {
                        case HitMargin.TooEarly: return NTE;
                        case HitMargin.VeryEarly: return NVE;
                        case HitMargin.EarlyPerfect: return NEP;
                        case HitMargin.Perfect: return NP;
                        case HitMargin.LatePerfect: return NLP;
                        case HitMargin.VeryLate: return NVL;
                        case HitMargin.TooLate: return NTL;
                        default: return 0;
                    }
                case global::Difficulty.Strict:
                    switch (margin)
                    {
                        case HitMargin.TooEarly: return STE;
                        case HitMargin.VeryEarly: return SVE;
                        case HitMargin.EarlyPerfect: return SEP;
                        case HitMargin.Perfect: return SP;
                        case HitMargin.LatePerfect: return SLP;
                        case HitMargin.VeryLate: return SVL;
                        case HitMargin.TooLate: return STL;
                        default: return 0;
                    }
                default: return 0;
            }
        }
    }
}
