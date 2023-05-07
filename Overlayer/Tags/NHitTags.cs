using Overlayer.Core.Tags;
using System.Runtime.ConstrainedExecution;

namespace Overlayer.Tags
{
    public static class NHitTags
    {
        [Tag("NHit", Category = Category.HitMargin)]
        [ReliabilityContract(Consistency.MayCorruptProcess, Cer.MayFail)]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Normal);
        [Tag("NTE", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static double TE() => Variables.NormalCounts[HitMargin.TooEarly];
        [Tag("NVE", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static double VE() => Variables.NormalCounts[HitMargin.VeryEarly];
        [Tag("NEP", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static double EP() => Variables.NormalCounts[HitMargin.EarlyPerfect];
        [Tag("NP", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static double P() => Variables.NormalCounts[HitMargin.Perfect];
        [Tag("NLP", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static double LP() => Variables.NormalCounts[HitMargin.LatePerfect];
        [Tag("NVL", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static double VL() => Variables.NormalCounts[HitMargin.VeryLate];
        [Tag("NTL", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix", Category = Category.HitMargin)]
        public static double TL() => Variables.NormalCounts[HitMargin.TooLate];
    }
}
