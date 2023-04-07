using AdofaiMapConverter;
using HarmonyLib;
using JetBrains.Annotations;
using Overlayer.Core.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Overlayer.Core
{
    public static class TagManager
    {
        static Dictionary<string, Tag> AllTags = new Dictionary<string, Tag>();
        static Dictionary<string, Tag> NotPlayingTags = new Dictionary<string, Tag>();
        static Dictionary<string, Tag> ReferencedTags = new Dictionary<string, Tag>();
        static List<ClassTagAttribute> TagPatches = new List<ClassTagAttribute>();
        public static Tag GetTag(string name) => AllTags.TryGetValue(name, out var tag) ? tag : null;
        public static Tag GetReferencedTag(string name) => ReferencedTags.TryGetValue(name, out var tag) ? tag : null;
        public static bool IsReferenced(string name) => ReferencedTags.ContainsKey(name);
        public static void UpdateReference()
        {
            ReferencedTags = AllTags.Values.Where(t => t.Referenced).ToDictionary(t => t.Name, t => t);
            foreach (Tag tag in ReferencedTags.Values)
            {

            }
            AllTags.Values.ForEach(t =>
            {
                if (TagPatches.Where(c => c.tags.Contains()))
                {
                    if (ReferencedTags.ContainsKey(t.Name))
                        PatchType(attr.harmony, attr.target);
                    else attr.harmony.UnpatchAll(attr.harmony.Id);
                }
            });
        }
        public static void AddTag(Tag tag, bool notPlaying)
        {
            AllTags.Add(tag.Name, tag);
            if (notPlaying) 
                NotPlayingTags.Add(tag.Name, tag);
        }
        public static bool RemoveTag(string name)
        {
            bool result = AllTags.Remove(name);
            NotPlayingTags.Remove(name);
            ReferencedTags.Remove(name);
            return result;
        }
        public static IEnumerable<Tag> All => AllTags.Values;
        public static IEnumerable<Tag> NP => NotPlayingTags.Values;
        static void Prepare()
        {
            AllTags ??= new Dictionary<string, Tag>();
            NotPlayingTags??= new Dictionary<string, Tag>();
            ReferencedTags ??= new Dictionary<string, Tag>();
            TagPatches ??= new Dictionary<Tag, ClassTagAttribute>();
        }
        public static void Load(Assembly assembly, TagConfig config = null)
        {
            foreach (Type type in assembly.GetTypes())
                Load(type, config);
        }
        public static void Load(Type type, TagConfig config = null)
        {
            ClassTagAttribute cTag = type.GetCustomAttribute<ClassTagAttribute>();
            if (cTag != null)
            {
                cTag.target = type;
                cTag.harmony = new Harmony($"ClassTag_{type.Name}_Harmony");
                TagPatches.Add()
                if (cTag.Name == null)
                {
                    List<Tag> tags = new List<Tag>();
                    
                }
            }
            foreach (MethodInfo method in type.GetMethods(AccessTools.all))
            {

            }
            foreach (FieldInfo field in type.GetFields(AccessTools.all))
            {

            }
        }
        public static void Release()
        {
            AllTags.Values.ForEach(t => t.Dispose());
            AllTags = NotPlayingTags = ReferencedTags = null;
            TagPatches.Values.ForEach(attr => attr.harmony.UnpatchAll(attr.harmony.Id));
            TagPatches = null;
        }
        static void PatchType(Harmony h, Type t)
        {
            h.CreateClassProcessor(t).Patch();
            foreach (Type nested in t.GetNestedTypes(AccessTools.allDeclared))
                h.CreateClassProcessor(nested).Patch();
        }
        static Func<int, double> GenerateFieldTagWrapper(FieldTagAttribute fTag, FieldInfo field)
        {
            DynamicMethod dm = new DynamicMethod($"{fTag.Name}Tag_Wrapper", typeof(double), new[] { typeof(int) });
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, field);
            if (fTag.Round)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, round);
            }
            il.Emit(OpCodes.Ret);
            return (Func<int, double>)dm.CreateDelegate(typeof(Func<int, double>));
        }
        static bool HasPatches(Tag t)
        {
            foreach (var cTag in TagPatches)
                if (cTag.tags.Contains(t))
                    return true;
            return false;
        }
        static readonly MethodInfo round = typeof(Math).GetMethod("Round", new[] { typeof(double), typeof(int) });
    }
}
