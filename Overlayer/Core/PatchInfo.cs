using HarmonyExLib;
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
            HarmonyExPatchType = DeterminePatchType(patchMethod);
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
        public HarmonyExPatchType HarmonyExPatchType { get; }
        public MethodInfo PatchMethod { get; }
        public MethodBase Original { get; }
        public Type PatchType { get; }
        public bool Dead { get; private set; }
        PatchClassProcessor proc;
        public bool Patch(HarmonyEx harmony)
        {
            if (Patched) return true;
            if (TypePatch)
            {
                if (harmony != proc?.instance)
                    proc?.Unpatch();
                proc = harmony.CreateClassProcessor(PatchType);
                proc.Patch(out var cannotPatch);
                Dead = cannotPatch.Any();
                if (Dead)
                {
                    OverlayerDebug.Log($"{this} Is Dead. Unavailable Patch List.");
                    foreach (var patch in cannotPatch)
                        OverlayerDebug.Log(patch.methodName);
                }
                Patched = true;
                return !Dead;
            }
            switch (HarmonyExPatchType)
            {
                case HarmonyExPatchType.Prefix:
                    harmony.Patch(Original, prefix: new HarmonyExMethod(PatchMethod));
                    break;
                case HarmonyExPatchType.Postfix:
                    harmony.Patch(Original, postfix: new HarmonyExMethod(PatchMethod));
                    break;
                case HarmonyExPatchType.Transpiler:
                    harmony.Patch(Original, transpiler: new HarmonyExMethod(PatchMethod));
                    break;
                case HarmonyExPatchType.Finalizer:
                    harmony.Patch(Original, finalizer: new HarmonyExMethod(PatchMethod));
                    break;
                case HarmonyExPatchType.ReversePatch:
                    harmony.CreateReversePatcher(Original, new HarmonyExMethod(PatchMethod)).Patch();
                    break;
                default: break;
            }
            return Patched = true;
        }
        public void Unpatch(HarmonyEx harmony)
        {
            if (!Patched) return;
            if (TypePatch) (proc ??= harmony.CreateClassProcessor(PatchType)).Unpatch();
            else harmony.Unpatch(Original, PatchMethod);
            Patched = false;
        }
        static HarmonyExPatchType DeterminePatchType(MethodInfo m)
        {
            bool isPrefix = m.Name == "Prefix" || m.GetCustomAttribute<HarmonyExPrefix>() != null;
            bool isPostfix = m.Name == "Postfix" || m.GetCustomAttribute<HarmonyExPostfix>() != null;
            bool isTranspiler = m.Name == "Transpiler" || m.GetCustomAttribute<HarmonyExTranspiler>() != null;
            bool isFinalizer = m.Name == "Finalizer" || m.GetCustomAttribute<HarmonyExFinalizer>() != null;
            bool isReversePatch = m.Name == "ReversePatch" || m.GetCustomAttribute<HarmonyExReversePatch>() != null;
            if (isPrefix) return HarmonyExPatchType.Prefix;
            else if (isPostfix) return HarmonyExPatchType.Postfix;
            else if (isTranspiler) return HarmonyExPatchType.Transpiler;
            else if (isFinalizer) return HarmonyExPatchType.Finalizer;
            else if (isReversePatch) return HarmonyExPatchType.ReversePatch;
            else return HarmonyExPatchType.All;
        }
        static MethodBase GetOriginal(MethodInfo replacement)
        {
            Type decType = replacement.DeclaringType;
            var tAttrs = decType.GetCustomAttributes<HarmonyExPatch>(true);
            var mAttrs = replacement.GetCustomAttributes<HarmonyExPatch>(true);
            var attrs = tAttrs.Concat(mAttrs);
            HarmonyExMethod info = HarmonyExMethod.Merge((from attr in attrs
                                                      where attr.GetType().BaseType.FullName == typeof(HarmonyExAttribute).FullName
                                                      select AccessTools.Field(attr.GetType(), "info").GetValue(attr) into harmonyInfo
                                                      select AccessTools.MakeDeepCopy<HarmonyExMethod>(harmonyInfo)).ToList());
            info.methodType ??= MethodType.Normal;
            return info.GetOriginalMethod();
        }
        public override string ToString() => TypePatch ? PatchType.FullName : $"Target:{Original.DeclaringType.FullName}.{Original.Name} {HarmonyExPatchType} {PatchMethod.DeclaringType.FullName}.{PatchMethod.Name}";
        public override int GetHashCode() => TypePatch ? PatchType.GetHashCode() : ((int)HarmonyExPatchType | (Original.GetHashCode() + PatchMethod.GetHashCode()));
        public override bool Equals(object obj) => obj is PatchInfo p && this == p;
        public static bool operator ==(PatchInfo a, PatchInfo b) => Comparer.Equals(a, b);
        public static bool operator !=(PatchInfo a, PatchInfo b) => !Comparer.Equals(a, b);
        public static IEqualityComparer<PatchInfo> Comparer = PatchInfoComparer.Instance;
        class PatchInfoComparer : IEqualityComparer<PatchInfo>
        {
            public static readonly IEqualityComparer<PatchInfo> Instance = new PatchInfoComparer();
            PatchInfoComparer() { }
            bool IEqualityComparer<PatchInfo>.Equals(PatchInfo x, PatchInfo y) => x.HarmonyExPatchType == y.HarmonyExPatchType && x.Original == y.Original && x.PatchMethod == y.PatchMethod;
            int IEqualityComparer<PatchInfo>.GetHashCode(PatchInfo obj) => obj.GetHashCode();
        }
    }
}
