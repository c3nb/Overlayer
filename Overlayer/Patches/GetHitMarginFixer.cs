using System;
using System.Collections.Generic;
using AdofaiMapConverter;
using HarmonyLib;
using Overlayer.Core;
using Overlayer.Tags;
using UnityEngine;
using System.Linq;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrMisc), "GetHitMargin")]
    public static class GetHitMarginFixer
    {
        public class PerDiff
        {
            public Difficulty Diff { get; private set; }
            public HitMargin NowMargin { get => nowMargin; private set => nowMargin = value; }
            public Dictionary<HitMargin, int> Counts { get; private set; } = new();
            public int Score { get; private set; } = 0;

            private HitMargin nowMargin;

            public PerDiff(Difficulty diff) { Diff = diff; }

            public void Update(HitMarginGetter getter)
            {
                NowMargin = getter(Diff);
                if (!nowMargin.SafeMargin()) Counts[NowMargin]++;
                Score += ScoreMap.GetValueSafe(NowMargin);
            }
        }

        public static int Combo = 0;
        private static readonly double radian = 57.295780181884766;

        public  static readonly Dictionary<Difficulty, PerDiff> diff = new()
        {
            { Difficulty.Lenient, new(Difficulty.Lenient) },
            { Difficulty.Normal, new(Difficulty.Normal) },
            { Difficulty.Strict, new(Difficulty.Strict) },
        };
        private static readonly Dictionary<HitMargin, int> ScoreMap = new()
        {
            { HitMargin.Perfect, 300 },
            { HitMargin.EarlyPerfect, 150 },
            { HitMargin.LatePerfect, 150 },
            { HitMargin.VeryEarly, 91 },
            { HitMargin.VeryLate, 91 },
        };
        private static readonly Dictionary<Difficulty, float> criteria = new()
        {
            { Difficulty.Lenient, 0.091f },
            { Difficulty.Normal, 0.065f },
            { Difficulty.Strict, 0.04f },
        };
        private static readonly Dictionary<string, string[]> diffTags = new()
        {
            { "Current", new[] { "CurHit", "CTE", "CVE", "CEP", "CP", "CLP", "CVL", "CTL", "Score" } },
            { "Lenient", new[] { "LHit", "LTE", "LVE", "LEP", "LP", "LLP", "LVL", "LTL", "LScore" } },
            { "Normal", new[] { "NHit", "NTE", "NVE", "NEP", "NP", "NLP", "NVL", "NTL", "NScore" } },
            { "Strict", new[] { "SHit", "STE", "SVE", "SEP", "SP", "SLP", "SVL", "STL", "SScore" } },
        };

        public delegate double boundaryGetter(float inMobile, float inPC, double minAngle);
        public delegate HitMargin HitMarginGetter(Difficulty diff);

        public static bool Prefix
            (float hitangle, float refangle, bool isCW, float bpmTimesSpeed, float conductorPitch, double marginScale, ref HitMargin __result)
        {
            var controller = scrController.instance;
            if (controller && controller.currFloor.freeroam) return true;

            float angle = (float)radian * (hitangle - refangle);
            if (isCW) angle = -angle;

            UpdateTags(bpmTimesSpeed, conductorPitch, marginScale, angle);

            __result = diff[GCS.difficulty].NowMargin;
            Combo = (__result == HitMargin.Perfect) ? Combo + 1 : 0;

            return false;
        }

        public static bool SafeMargin(this ref HitMargin hitMargin)
        {
            scrController ctrl = scrController.instance;
            if (ctrl.gameworld)
            {
                if (ctrl.noFailInfiniteMargin)
                    hitMargin = HitMargin.FailMiss;
                if (ctrl.midspinInfiniteMargin || (RDC.auto && !RDC.useOldAuto))
                    hitMargin = HitMargin.Perfect;
            }

            return ctrl.currFloor?.isSafe ?? false;
        }

        private static void UpdateTags(float bpmTimesSpeed, float conductorPitch, double marginScale, float angle)
        {
            HitMarginGetter getter = HitMarginGenerater(angle, bpmTimesSpeed, conductorPitch, marginScale);
            Dictionary<Difficulty, bool> Up = new()
            {
                { Difficulty.Lenient, false },
                { Difficulty.Normal, false },
                { Difficulty.Strict, false },
            };

            if (HasAnyTags(diffTags["Lenient"]))
            {
                diff[Difficulty.Lenient].Update(getter);
                Up[Difficulty.Lenient] = true;
            }
            if (HasAnyTags(diffTags["Normal"]))
            {
                diff[Difficulty.Normal].Update(getter);
                Up[Difficulty.Normal] = true;
            }
            if (HasAnyTags(diffTags["Strict"]))
            {
                diff[Difficulty.Strict].Update(getter);
                Up[Difficulty.Strict] = true;
            }
            if (HasAnyTags(diffTags["Current"]) && !Up[GCS.difficulty])
            {
                diff[GCS.difficulty].Update(getter);
            }
        }

        private static bool HasAnyTags(string[] tags) =>
            (from string tag in tags
             where TagManager.IsReferenced(tag)
             select new { })
            .Count() != 0;

        private static boundaryGetter BoundaryGetter(float bpm, float pitch, double marginScale) =>
            (inMobile, inPC, minAngle) =>
            {
                float a = ADOBase.isMobile ? inMobile : (inPC / GCS.currentSpeedTrial);

                double angleInRead = scrMisc.TimeToAngleInRad((double)Mathf.Max(a, 0.025f), (double)bpm, (double)pitch, false);
                return Math.Max(minAngle * marginScale, radian * angleInRead);
            };

        private static HitMarginGetter HitMarginGenerater
            (float angle, float bpmTimesSpeed, float conductorPitch, double marginScale) => 
            diff =>
            {
                boundaryGetter getBoundary = BoundaryGetter(bpmTimesSpeed, conductorPitch, marginScale);
                double passBound = getBoundary(0.09f, criteria.GetValueSafe(diff, 0.065f), GCS.HITMARGIN_COUNTED);
                double perfectBound = getBoundary(0.07f, 0.03f, 45.0);
                double pureBound = getBoundary(0.05f, 0.02f, 30.0);

                HitMargin result = HitMargin.TooEarly;
                if ((double)angle > -passBound)
                    result = HitMargin.VeryEarly;
                if ((double)angle > -perfectBound)
                    result = HitMargin.EarlyPerfect;
                if ((double)angle > -pureBound)
                    result = HitMargin.Perfect;
                if ((double)angle > pureBound)
                    result = HitMargin.LatePerfect;
                if ((double)angle > perfectBound)
                    result = HitMargin.VeryLate;
                if ((double)angle > passBound)
                    result = HitMargin.TooLate;
                return result;
            };
    }
}
