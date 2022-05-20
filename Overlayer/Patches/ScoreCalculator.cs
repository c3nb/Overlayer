using HarmonyLib;

namespace Overlayer.Patches
{
    [HarmonyPatch(typeof(scrMistakesManager), "AddHit")]
    public static class ScoreCalculator
    {
        public static void Postfix(HitMargin hit)
        {
            if (hit == HitMargin.Perfect) Variables.Combo++;
            else Variables.Combo = 0;
            switch (hit)
            {
                case HitMargin.VeryEarly:
                case HitMargin.VeryLate:
                    Variables.CurrentScore += 91;
                    break;
                case HitMargin.EarlyPerfect:
                case HitMargin.LatePerfect:
                    Variables.CurrentScore += 150;
                    break;
                case HitMargin.Perfect:
                    Variables.CurrentScore += 300;
                    break;
            }
            if (GCS.difficulty != Difficulty.Lenient)
                switch (Utils.GetHitMarginForDifficulty(Variables.Angle, Difficulty.Lenient))
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Variables.LenientScore += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Variables.LenientScore += 150;
                        break;
                    case HitMargin.Perfect:
                        Variables.LenientScore += 300;
                        break;
                }
            if (GCS.difficulty != Difficulty.Normal)
                switch (Utils.GetHitMarginForDifficulty(Variables.Angle, Difficulty.Normal))
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Variables.NormalScore += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Variables.NormalScore += 150;
                        break;
                    case HitMargin.Perfect:
                        Variables.NormalScore += 300;
                        break;
                }
            if (GCS.difficulty != Difficulty.Strict)
                switch (Utils.GetHitMarginForDifficulty(Variables.Angle, Difficulty.Strict))
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        Variables.StrictScore += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        Variables.StrictScore += 150;
                        break;
                    case HitMargin.Perfect:
                        Variables.StrictScore += 300;
                        break;
                }
        }
    }
}
