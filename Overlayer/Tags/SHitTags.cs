using Overlayer.Core.Tags;
using System.Runtime.ConstrainedExecution;

namespace Overlayer.Tags
{
    public static class SHitTags
    {
        [Tag("SHit")]
        [ReliabilityContract(Consistency.MayCorruptProcess, Cer.MayFail)]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Strict);
        [Tag("STE", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double TE() => Variables.StrictCounts[HitMargin.TooEarly];
        [Tag("SVE", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double VE() => Variables.StrictCounts[HitMargin.VeryEarly];
        [Tag("SEP", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double EP() => Variables.StrictCounts[HitMargin.EarlyPerfect];
        [Tag("SP", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double P() => Variables.StrictCounts[HitMargin.Perfect];
        [Tag("SLP", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double LP() => Variables.StrictCounts[HitMargin.LatePerfect];
        [Tag("SVL", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double VL() => Variables.StrictCounts[HitMargin.VeryLate];
        [Tag("STL", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double TL() => Variables.StrictCounts[HitMargin.TooLate];
    }
}
