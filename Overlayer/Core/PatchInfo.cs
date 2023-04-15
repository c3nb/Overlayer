using HarmonyEx;
using System;
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
        public bool Dead { get; private set; }
        PatchClassProcessor proc;
        public bool Patch(Harmony harmony)
        {
            if (Patched) return true;
            if (TypePatch)
            {
                if (harmony != proc?.instance)
                    proc?.Unpatch();
                proc = harmony.CreateClassProcessor(PatchType);
                proc.Patch(out var cannotPatch);
                Dead = cannotPatch.Any();
                OverlayerDebug.Log($"{this} Is Dead. Unavailable Patch List.");
                foreach (var patch in cannotPatch)
                    OverlayerDebug.Log(patch.methodName);
                Patched = true;
                return !Dead;
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
            return Patched = true;
        }
        public void Unpatch(Harmony harmony)
        {
            if (!Patched) return;
            if (TypePatch) (proc ??= harmony.CreateClassProcessor(PatchType)).Unpatch();
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
            return info.GetOriginalMethod();
        }
        public override string ToString() => TypePatch ? PatchType.FullName : $"Target:{Original.DeclaringType.FullName}.{Original.Name} {HarmonyPatchType} {PatchMethod.DeclaringType.FullName}.{PatchMethod.Name}";
        public override int GetHashCode() => TypePatch ? PatchType.GetHashCode() : ((int)HarmonyPatchType | (Original.GetHashCode() + PatchMethod.GetHashCode()));
        public override bool Equals(object obj) => obj is PatchInfo p && this == p;
        public static bool operator ==(PatchInfo a, PatchInfo b) => Comparer.Equals(a, b);
        public static bool operator !=(PatchInfo a, PatchInfo b) => !Comparer.Equals(a, b);
        public static IEqualityComparer<PatchInfo> Comparer = PatchInfoComparer.Instance;
        class PatchInfoComparer : IEqualityComparer<PatchInfo>
        {
            public static readonly IEqualityComparer<PatchInfo> Instance = new PatchInfoComparer();
            PatchInfoComparer() { }
            bool IEqualityComparer<PatchInfo>.Equals(PatchInfo x, PatchInfo y) => x.HarmonyPatchType == y.HarmonyPatchType && x.Original == y.Original && x.PatchMethod == y.PatchMethod;
            int IEqualityComparer<PatchInfo>.GetHashCode(PatchInfo obj) => obj.GetHashCode();
        }
    }
}
