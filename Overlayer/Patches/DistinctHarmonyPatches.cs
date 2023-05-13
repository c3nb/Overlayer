using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Overlayer.Patches
{
    public static class DistinctHarmonyPatches
    {
        public static void Patch(Harmony harmony)
        {
            add ??= typeof(PatchInfo).GetMethod("Add", AccessTools.all);
            if (add != null) harmony.Patch(add, new HarmonyMethod(PrefixPatch));
            else
            {
                addPre ??= typeof(PatchInfo).GetMethod("AddPrefix");
                addPost ??= typeof(PatchInfo).GetMethod("AddPostfix");
                addT ??= typeof(PatchInfo).GetMethod("AddTranspiler");
                addF ??= typeof(PatchInfo).GetMethod("AddFinalizer");
                harmony.Patch(addPre, new HarmonyMethod(AddPrefixPatch));
                harmony.Patch(addPost, new HarmonyMethod(AddPostfixPatch));
                harmony.Patch(addT, new HarmonyMethod(AddTranspilerPatch));
                harmony.Patch(addF, new HarmonyMethod(AddFinalizerPatch));
            }
        }
        public static void Unpatch(Harmony harmony)
        {
            if (add != null) harmony.Unpatch(add, PrefixPatch);
            else
            {
                harmony.Unpatch(addPre, AddPrefixPatch);
                harmony.Unpatch(addPost, AddPostfixPatch);
                harmony.Unpatch(addT, AddTranspilerPatch);
                harmony.Unpatch(addF, AddFinalizerPatch);
            }
        }
        public static MethodInfo add;
        public static MethodInfo addPre;
        public static MethodInfo addPost;
        public static MethodInfo addT;
        public static MethodInfo addF;
        public static readonly MethodInfo PrefixPatch = typeof(DistinctHarmonyPatches).GetMethod("Prefix");
        public static readonly MethodInfo AddPrefixPatch = typeof(DistinctHarmonyPatches_AddPrefix).GetMethod("Prefix");
        public static readonly MethodInfo AddPostfixPatch = typeof(DistinctHarmonyPatches_AddPostfix).GetMethod("Prefix");
        public static readonly MethodInfo AddTranspilerPatch = typeof(DistinctHarmonyPatches_AddTranspiler).GetMethod("Prefix");
        public static readonly MethodInfo AddFinalizerPatch = typeof(DistinctHarmonyPatches_AddFinalizer).GetMethod("Prefix");
        public static bool Prefix(string owner, HarmonyMethod[] add, Patch[] current, ref Patch[] __result)
        {
            if (add.Length == 0)
            {
                __result = current;
                return false;
            }
            int initialIndex = current.Length;
            __result = current.Concat(add.Where((HarmonyMethod method) => method != null)
                .Select((HarmonyMethod method, int i) => new Patch(method, i + initialIndex, owner)))
                .Distinct(Core.PatchInfo.PatchComparer.Instance)
                .ToArray();
            return false;
        }
        public static class DistinctHarmonyPatches_AddPrefix
        {
            public static bool Prefix(PatchInfo __instance, MethodInfo patch, string owner, int priority, string[] before, string[] after, bool debug)
            {
                List<Patch> list = __instance.prefixes.ToList();
                list.Add(new Patch(patch, __instance.prefixes.Count() + 1, owner, priority, before, after, debug));
                __instance.prefixes = list.Distinct(Core.PatchInfo.PatchComparer.Instance).ToArray();
                return false;
            }
        }
        public static class DistinctHarmonyPatches_AddPostfix
        {
            public static bool Prefix(PatchInfo __instance, MethodInfo patch, string owner, int priority, string[] before, string[] after, bool debug)
            {
                List<Patch> list = __instance.postfixes.ToList();
                list.Add(new Patch(patch, __instance.postfixes.Count() + 1, owner, priority, before, after, debug));
                __instance.postfixes = list.Distinct(Core.PatchInfo.PatchComparer.Instance).ToArray();
                return false;
            }
        }
        public static class DistinctHarmonyPatches_AddTranspiler
        {
            public static bool Prefix(PatchInfo __instance, MethodInfo patch, string owner, int priority, string[] before, string[] after, bool debug)
            {
                List<Patch> list = __instance.transpilers.ToList();
                list.Add(new Patch(patch, __instance.transpilers.Count() + 1, owner, priority, before, after, debug));
                __instance.transpilers = list.Distinct(Core.PatchInfo.PatchComparer.Instance).ToArray();
                return false;
            }
        }
        public static class DistinctHarmonyPatches_AddFinalizer
        {
            public static bool Prefix(PatchInfo __instance, MethodInfo patch, string owner, int priority, string[] before, string[] after, bool debug)
            {
                List<Patch> list = __instance.finalizers.ToList();
                list.Add(new Patch(patch, __instance.finalizers.Count() + 1, owner, priority, before, after, debug));
                __instance.finalizers = list.Distinct(Core.PatchInfo.PatchComparer.Instance).ToArray();
                return false;
            }
        }
    }
}
