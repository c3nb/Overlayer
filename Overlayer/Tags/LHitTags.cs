using Overlayer.Core.Tags;
using System.Runtime.ConstrainedExecution;

namespace Overlayer.Tags
{
    public static class LHitTags
    {
        [Tag("LHit")]
        [ReliabilityContract(Consistency.MayCorruptProcess, Cer.MayFail)]
        public static string Hit() => RDString.Get("HitMargin." + Variables.Lenient);
        [Tag("LTE", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double TE() => Variables.LenientCounts[HitMargin.TooEarly];
        [Tag("LVE", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double VE() => Variables.LenientCounts[HitMargin.VeryEarly];
        [Tag("LEP", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double EP() => Variables.LenientCounts[HitMargin.EarlyPerfect];
        [Tag("LP", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double P() => Variables.LenientCounts[HitMargin.Perfect];
        [Tag("LLP", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double LP() => Variables.LenientCounts[HitMargin.LatePerfect];
        [Tag("LVL", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double VL() => Variables.LenientCounts[HitMargin.VeryLate];
        [Tag("LTL", RelatedPatches = "Overlayer.Patches.GetHitMarginFixer:Prefix")]
        public static double TL() => Variables.LenientCounts[HitMargin.TooLate];
    }
}
