using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Overlayer.Core
{
    public class PatchInfo
    {
        public PatchInfo(MethodInfo patchMethod)
        {
            PatchMethod = patchMethod;
            HarmonyPatchType = DeterminePatchType(patchMethod);
            Original = GetOriginal(patchMethod);
            TypePatch = false;
        }
        public PatchInfo(Type patchType)
        {
            PatchType = patchType;
            TypePatch = true;
        }
        public bool TypePatch { get; private set; }
        public bool Patched { get; private set; }
        public HarmonyPatchType HarmonyPatchType { get; }
        public MethodInfo PatchMethod { get; }
        public MethodBase Original { get; }
        public Type PatchType { get; }
        PatchClassProcessor proc;
        public void Patch(Harmony harmony)
        {
            if (Patched) return;
            if (TypePatch)
            {
                if (proc != null)
                    if (harmony != pcp_instance(proc))
                        UnpatchPCP(proc);
                proc = harmony.CreateClassProcessor(PatchType);
                proc.Patch();
                Patched = true;
                return;
            }
            switch (HarmonyPatchType)
            {
                case HarmonyPatchType.Prefix:
                    harmony.Patch(Original, prefix: new HarmonyMethod(PatchMethod));
                    break;
                case HarmonyPatchType.Postfix:
                    harmony.Patch(Original, postfix: new HarmonyMethod(PatchMethod));
                    break;
                case HarmonyPatchType.Transpiler:
                    harmony.Patch(Original, transpiler: new HarmonyMethod(PatchMethod));
                    break;
                case HarmonyPatchType.Finalizer:
                    harmony.Patch(Original, finalizer: new HarmonyMethod(PatchMethod));
                    break;
                case HarmonyPatchType.ReversePatch:
                    harmony.CreateReversePatcher(Original, new HarmonyMethod(PatchMethod)).Patch();
                    break;
                default: break;
            }
            Patched = true;
            return;
        }
        public void Unpatch(Harmony harmony)
        {
            if (!Patched) return;
            if (TypePatch) UnpatchPCP(proc ??= harmony.CreateClassProcessor(PatchType));
            else harmony.Unpatch(Original, PatchMethod);
            Patched = false;
        }
        static HarmonyPatchType DeterminePatchType(MethodInfo m)
        {
            bool isPrefix = m.Name == "Prefix" || m.GetCustomAttribute<HarmonyPrefix>() != null;
            bool isPostfix = m.Name == "Postfix" || m.GetCustomAttribute<HarmonyPostfix>() != null;
            bool isTranspiler = m.Name == "Transpiler" || m.GetCustomAttribute<HarmonyTranspiler>() != null;
            bool isFinalizer = m.Name == "Finalizer" || m.GetCustomAttribute<HarmonyFinalizer>() != null;
            bool isReversePatch = m.Name == "ReversePatch" || m.GetCustomAttribute<HarmonyReversePatch>() != null;
            if (isPrefix) return HarmonyPatchType.Prefix;
            else if (isPostfix) return HarmonyPatchType.Postfix;
            else if (isTranspiler) return HarmonyPatchType.Transpiler;
            else if (isFinalizer) return HarmonyPatchType.Finalizer;
            else if (isReversePatch) return HarmonyPatchType.ReversePatch;
            else return HarmonyPatchType.All;
        }
        static MethodBase GetOriginal(MethodInfo replacement)
        {
            Type decType = replacement.DeclaringType;
            var tAttrs = decType.GetCustomAttributes<HarmonyPatch>(true);
            var mAttrs = replacement.GetCustomAttributes<HarmonyPatch>(true);
            var attrs = tAttrs.Concat(mAttrs);
            HarmonyMethod info = HarmonyMethod.Merge((from attr in attrs
                                                      where attr.GetType().BaseType.FullName == typeof(HarmonyAttribute).FullName
                                                      select AccessTools.Field(attr.GetType(), "info").GetValue(attr) into harmonyInfo
                                                      select AccessTools.MakeDeepCopy<HarmonyMethod>(harmonyInfo)).ToList());
            info.methodType ??= MethodType.Normal;
            return (MethodBase)getOriginalMethod(null, info);
        }
        public static FastInvokeHandler getOriginalMethod = MethodInvoker.GetHandler(AccessTools.Method("HarmonyLib.PatchTools:GetOriginalMethod"));
        public static FastInvokeHandler getBulkMethods = MethodInvoker.GetHandler(AccessTools.Method("HarmonyLib.PatchClassProcessor:GetBulkMethods"));
        public static FastInvokeHandler getPatchInfo = MethodInvoker.GetHandler(AccessTools.Method("HarmonyLib.HarmonySharedState:GetPatchInfo"));
        public static AccessTools.FieldRef<PatchClassProcessor, Harmony> pcp_instance = AccessTools.FieldRefAccess<PatchClassProcessor, Harmony>(AccessTools.Field(typeof(PatchClassProcessor), "instance"));
        public static AccessTools.FieldRef<PatchClassProcessor, object> pcp_patchMethods = AccessTools.FieldRefAccess<PatchClassProcessor, object>(AccessTools.Field(typeof(PatchClassProcessor), "patchMethods"));
        public static AccessTools.FieldRef<object, HarmonyMethod> attrPatch_info = AccessTools.FieldRefAccess<HarmonyMethod>(Utility.TypeByName("HarmonyLib.AttributePatch"), "info");
        public override string ToString() => TypePatch ? PatchType.FullName : $"Target:{Original.DeclaringType.FullName}.{Original.Name} {HarmonyPatchType} {PatchMethod.DeclaringType.FullName}.{PatchMethod.Name}";
        public override int GetHashCode() => TypePatch ? PatchType.GetHashCode() : ((int)HarmonyPatchType | (Original.GetHashCode() + PatchMethod.GetHashCode()));
        public override bool Equals(object obj) => obj is PatchInfo p && this == p;
        public static bool operator ==(PatchInfo a, PatchInfo b) => Comparer.Equals(a, b);
        public static bool operator !=(PatchInfo a, PatchInfo b) => !Comparer.Equals(a, b);
        public static IEqualityComparer<PatchInfo> Comparer = PatchInfoComparer.Instance;
        static void UnpatchPCP(PatchClassProcessor pcp)
        {
            List<(MethodBase, HarmonyMethod)> toUnpatch = new List<(MethodBase, HarmonyMethod)>();
            var bulks = (List<MethodBase>)getBulkMethods(pcp);
            var patchMethods = pcp_patchMethods(pcp) as IEnumerable;
            if (bulks.Count > 0)
            {
                foreach (var method in bulks)
                    foreach (var attrPatch in patchMethods)
                        toUnpatch.Add((method, attrPatch_info(attrPatch)));
            }
            else
            {
                foreach (var attrPatch in patchMethods)
                {
                    var info = attrPatch_info(attrPatch);
                    toUnpatch.Add(((MethodBase)getOriginalMethod(info), info));
                }
            }
            foreach (var (original, patch) in toUnpatch)
            {
                HarmonyLib.PatchInfo patchInfo = (HarmonyLib.PatchInfo)getPatchInfo(original);
                patchInfo.RemovePatch(patch.method);
            }
        }
        internal class PatchComparer : IEqualityComparer<Patch>
        {
            PatchComparer() { }
            public static readonly IEqualityComparer<Patch> Instance = new PatchComparer();
            public bool Equals(Patch a, Patch b) => a.PatchMethod == b.PatchMethod;
            public int GetHashCode(Patch p) => p.PatchMethod.GetHashCode();
        }
        class PatchInfoComparer : IEqualityComparer<PatchInfo>
        {
            public static readonly IEqualityComparer<PatchInfo> Instance = new PatchInfoComparer();
            PatchInfoComparer() { }
            bool IEqualityComparer<PatchInfo>.Equals(PatchInfo x, PatchInfo y) => x.HarmonyPatchType == y.HarmonyPatchType && x.Original == y.Original && x.PatchMethod == y.PatchMethod;
            int IEqualityComparer<PatchInfo>.GetHashCode(PatchInfo obj) => obj.GetHashCode();
        }
    }
}
