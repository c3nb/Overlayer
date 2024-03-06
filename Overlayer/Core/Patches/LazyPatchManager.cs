using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Overlayer.Core.Patches
{
    public static class LazyPatchManager
    {
        private static readonly Harmony Harmony = new Harmony("Overlayer.Core.Patches.LazyPatchManager");
        private static readonly Dictionary<Type, List<LazyPatch>> Patches = new Dictionary<Type, List<LazyPatch>>();
        private static readonly HashSet<string> PatchedTriggers = new HashSet<string>();
        public static int PatchedTriggersCount => PatchedTriggers.Count;
        public static int Count => Patches.Sum(t => t.Value.Count);
        public static bool IsInternalPatched => PatchedTriggers.Contains(LazyPatch.InternalTrigger);
        public static void Load(Assembly ass)
        {
            foreach (var type in ass.GetTypes())
            {
                var lpas = type.GetCustomAttributes<LazyPatchAttribute>();
                if (lpas.Any())
                {
                    if (!Patches.TryGetValue(type, out var list))
                        list = Patches[type] = new List<LazyPatch>();
                    foreach (var patch in lpas)
                        list.Add(new LazyPatch(Harmony, type, patch));
                }
            }
        }
        public static void Unload(Assembly ass)
        {
            foreach (var type in ass.GetTypes())
            {
                if (Patches.TryGetValue(type, out var patches))
                {
                    Unpatch(type);
                    patches.ForEach(lp => LazyPatch.Patches.Remove(lp.attr.Id));
                    Patches.Remove(type);
                }
            }
        }
        public static void UnloadAll()
        {
            Harmony.UnpatchAll(Harmony.Id);
            Patches.Clear();
            PatchedTriggers.Clear();
            LazyPatch.Patches.Clear();
        }
        public static void PatchInternal() => PatchAll(LazyPatch.InternalTrigger);
        public static List<LazyPatch> PatchAll(string trigger = null)
        {
            List<LazyPatch> patches = new List<LazyPatch>();
            if (trigger != null)
            {
                if (PatchedTriggers.Add(trigger))
                    foreach (var patch in patches = Patches.Values.SelectMany(list => list).Where(p => p.attr.Triggers.Contains(trigger)).ToList())
                        patch.Patch();
            }
            else
            {
                foreach (var patchType in Patches.Keys)
                    patches.AddRange(Patch(patchType));
            }
            return patches;
        }
        public static List<LazyPatch> UnpatchAll(string trigger = null)
        {
            List<LazyPatch> patches = new List<LazyPatch>();
            if (trigger != null)
            {
                if (PatchedTriggers.Remove(trigger))
                    foreach (var patch in patches = Patches.Values.SelectMany(list => list).Where(p => p.attr.Triggers.Contains(trigger)).ToList())
                        patch.Unpatch();
            }
            else
            {
                foreach (var patchType in Patches.Keys)
                    patches.AddRange(Unpatch(patchType));
            }
            return patches;
        }
        public static List<LazyPatch> Patch(Type patchType, bool force = false)
        {
            if (Patches.TryGetValue(patchType, out var patches))
            {
                patches.ForEach(lp => lp.Patch(force));
                return patches;
            }
            return null;
        }
        public static List<LazyPatch> Unpatch(Type patchType, bool force = false)
        {
            if (Patches.TryGetValue(patchType, out var patches))
            {
                patches.ForEach(lp => lp.Unpatch(force));
                return patches;
            }
            return null;
        }
        internal static void PatchNested(Type patchType, bool force = false, bool lockPatch = true)
        {
            foreach (var nType in patchType.GetNestedTypes((BindingFlags)15420))
                Patch(nType, force)?.ForEach(p => p.Locked = lockPatch);
        }
        internal static void UnpatchNested(Type patchType, bool force = false, bool lockPatch = true)
        {
            foreach (var nType in patchType.GetNestedTypes((BindingFlags)15420))
                Unpatch(nType, force)?.ForEach(p => p.Locked = lockPatch);
        }
    }
}
