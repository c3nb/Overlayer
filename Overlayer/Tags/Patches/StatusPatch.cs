using Overlayer.Core.Patches;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overlayer.Tags.Patches
{
    public class StatusPatch : PatchBase<StatusPatch>
    {
        [LazyPatch("Tags.Status.TotalCheckPointsPatch_scnGame", "scnGame", "Play", Triggers = new string[] { nameof(Status.TotalCheckPoints) })]
        [LazyPatch("Tags.Status.TotalCheckPointsPatch_scrPressToStart", "scrPressToStart", "ShowText", Triggers = new string[] { nameof(Status.TotalCheckPoints) })]
        public static class TotalCheckPointsPatch
        {
            public static void Postfix()
            {
                Status.TotalCheckPoints = scrLevelMaker.instance.listFloors.Count(f => f.GetComponent<ffxCheckpoint>() != null);
            }
        }
        [LazyPatch("Tags.Status.Combo&ScoreCalculator", "scrMisc", "GetHitMargin", Triggers = new string[]
        {
            nameof(Status.Combo), nameof(Status.MaxCombo), nameof(Status.LScore), nameof(Status.NScore), nameof(Status.SScore), nameof(Status.Score),
            nameof(Status.LMarginCombo), nameof(Status.NMarginCombo), nameof(Status.SMarginCombo), nameof(Status.MarginCombo),
            nameof(Status.LMarginMaxCombo), nameof(Status.NMarginMaxCombo), nameof(Status.SMarginMaxCombo), nameof(Status.MarginMaxCombo),
        })]
        public static class ComboAndScoresPatch
        {
            public static void Postfix(float hitangle, float refangle, bool isCW, float bpmTimesSpeed, float conductorPitch, double marginScale, ref HitMargin __result)
            {
                var controller = scrController.instance;
                if (controller && controller.currFloor.freeroam) return;
                if (!HitPatch.JudgementTagPatch.IsSafe(controller))
                {
                    if (__result == HitMargin.Perfect)
                        Status.MaxCombo = Math.Max(Status.MaxCombo, ++Status.Combo);
                    else Status.Combo = 0;
                    var l = HitPatch.JudgementTagPatch.GetHitMargin(Difficulty.Lenient, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
                    var n = HitPatch.JudgementTagPatch.GetHitMargin(Difficulty.Normal, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
                    var s = HitPatch.JudgementTagPatch.GetHitMargin(Difficulty.Strict, hitangle, refangle, isCW, bpmTimesSpeed, conductorPitch, marginScale);
                    HitPatch.JudgementTagPatch.FixMargin(controller, ref l);
                    HitPatch.JudgementTagPatch.FixMargin(controller, ref n);
                    HitPatch.JudgementTagPatch.FixMargin(controller, ref s);
                    SetScores(l, n, s, __result);
                    SetCombos(Difficulty.Lenient, l);
                    SetCombos(Difficulty.Normal, n);
                    SetCombos(Difficulty.Strict, s);
                }
            }
            private static void SetScores(HitMargin l, HitMargin n, HitMargin s, HitMargin c)
            {
                switch (c)
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Status.Score += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Status.Score += 150;
                        break;
                    case HitMargin.Perfect:
                        Status.Score += 300;
                        break;
                }
                switch (l)
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Status.LScore += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Status.LScore += 150;
                        break;
                    case HitMargin.Perfect:
                        Status.LScore += 300;
                        break;
                }
                switch (n)
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Status.NScore += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Status.NScore += 150;
                        break;
                    case HitMargin.Perfect:
                        Status.NScore += 300;
                        break;
                }
                switch (s)
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Status.SScore += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Status.SScore += 150;
                        break;
                    case HitMargin.Perfect:
                        Status.SScore += 300;
                        break;
                }
            }
            private static void SetCombos(Difficulty diff, HitMargin hit)
            {
                int iHit = (int)hit;
                int[] combos = Status.Combos[(int)diff];
                int[] maxCombos = Status.MaxCombos[(int)diff];
                combos[iHit]++;
                for (int i = 0; i < combos.Length; i++)
                    if (i != iHit) combos[i] = 0;
                for (int i = 0; i < maxCombos.Length; i++)
                    maxCombos[i] = Math.Max(maxCombos[i], combos[i]);
            }
        }
        [LazyPatch("Tags.Status.CurrentCheckPointPreparer_scnGame", "scnGame", "Play", Triggers = new string[]
        {
            nameof(Status.CurCheckPoint)
        })]
        [LazyPatch("Tags.Status.CurrentCheckPointPreparer_scnEditor", "scnEditor", "Play", Triggers = new string[]
        {
            nameof(Status.CurCheckPoint)
        })]
        [LazyPatch("Tags.Status.CurrentCheckPointPreparer_scrPressToStart", "scrPressToStart", "ShowText", Triggers = new string[]
        {
            nameof(Status.CurCheckPoint)
        })]
        public static class CurrentCheckPointPreparer
        {
            public static List<scrFloor> AllCheckPoints;
            public static void Postfix()
            {
                AllCheckPoints = scrLevelMaker.instance.listFloors.FindAll(f => f.GetComponent<ffxCheckpoint>() != null);
            }
        }
        [LazyPatch("Tag.Status.CurrentCheckPointGetter", "scrPlanet", "MoveToNextFloor", Triggers = new string[]
        {
            nameof(Status.CurCheckPoint)
        })]
        public static class CurrentCheckPointGetter
        {
            public static void Postfix(scrFloor floor)
            {
                if (CurrentCheckPointPreparer.AllCheckPoints == null) return;
                Status.CurCheckPoint = GetCheckPointIndex(floor);
            }
            public static int GetCheckPointIndex(scrFloor floor)
            {
                if (floor == null) return 0;
                int i = 0;
                foreach (var chkPt in CurrentCheckPointPreparer.AllCheckPoints)
                {
                    if (floor.seqID + 1 <= chkPt.seqID)
                        return i;
                    i++;
                }
                return i;
            }
        }
        [LazyPatch("Tag.Status.BestProgressResetter_Editor", "scnEditor", "OpenLevelCo", Triggers = new string[]
        {
            nameof(Status.BestProgress)
        })]
        [LazyPatch("Tag.Status.BestProgressResetter_CLS", "scnGame", "LoadLevel", Triggers = new string[]
        {
            nameof(Status.BestProgress)
        })]
        public static class BestProgressResetter
        {
            public static void Postfix()
            {
                Status.BestProgress = 0;
            }
        }
        [LazyPatch("Tag.Status.BestProgressUpdater_OnMove", "scrPlanet", "MoveToNextFloor", Triggers = new string[]
        {
            nameof(Status.BestProgress)
        })]
        [LazyPatch("Tag.Status.BestProgressUpdater_OnFail", "scrController", "FailAction", Triggers = new string[]
        {
            nameof(Status.BestProgress)
        })]
        public static class BestProgressUpdater
        {
            public static void Postfix()
            {
                if (scrLevelMaker.instance == null) return;
                Status.BestProgress = Math.Max(Status.BestProgress, scrController.instance.percentComplete * 100);
            }
        }
        [LazyPatch("Tag.Status.BestProgressFixer", "scrController", "OnLandOnPortal", Triggers = new string[]
        {
            nameof(Status.BestProgress)
        })]
        public static class BestProgressFixer
        {
            public static void Postfix(scrController __instance)
            {
                if (__instance.gameworld)
                    Status.BestProgress = 100;
            }
        }
    }
}
