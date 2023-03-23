using AdofaiMapConverter;
using HarmonyLib;
using JetBrains.Annotations;
using Overlayer.Core.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Overlayer.Core
{
    public static class TagManager
    {
        static Dictionary<string, Tag> AllTags = new Dictionary<string, Tag>();
        static Dictionary<string, Tag> NotPlayingTags = new Dictionary<string, Tag>();
        static Dictionary<string, Tag> ReferencedTags = new Dictionary<string, Tag>();
        public static Tag GetTag(string name) => AllTags.TryGetValue(name, out var tag) ? tag : null;
        public static Tag GetReferencedTag(string name) => ReferencedTags.TryGetValue(name, out var tag) ? tag : null;
        public static bool IsReferenced(string name) => ReferencedTags.ContainsKey(name);
        public static void UpdateReference() => ReferencedTags = AllTags.Values.Where(t => t.Referenced).ToDictionary(t => t.Name, t => t);
        public static bool RemoveTag(string name)
        {
            bool result = AllTags.Remove(name);
            result &= NotPlayingTags.Remove(name);
            return result;
        }
        public static IEnumerable<Tag> All => AllTags.Values;
        public static IEnumerable<Tag> NP => NotPlayingTags.Values;
        static void Prepare()
        {
            AllTags ??= new Dictionary<string, Tag>();
            NotPlayingTags??= new Dictionary<string, Tag>();
            ReferencedTags ??= new Dictionary<string, Tag>();
        }
        public static void Load(Assembly assembly, TagConfig config = null)
        {
            foreach (Type type in assembly.GetTypes())
                Load(type, config);
        }
        public static void Load(Type type, TagConfig config = null)
        {
            foreach (MethodInfo method in type.GetMethods(AccessTools.all))
                Load(method, config);
        }
        public static void Load(MethodInfo getter, TagConfig config = null)
        {
            Prepare();
            if (getter == null) return;
            TagAttribute tagAttr = getter.GetCustomAttribute<TagAttribute>();
            if (tagAttr == null) return;
            Tag tag = new Tag(tagAttr.Name, config);
            if (tagAttr.IsDefault)
            {
                Type decType = getter.DeclaringType;
                ClassTagAttribute decTypeTag = decType.GetCustomAttribute<ClassTagAttribute>();
                if (decTypeTag != null)
                {
                    decTypeTag.Combine(tagAttr);
                    foreach (string thread in decTypeTag.Threads)
                        tag.AddThread(decType.GetMethod(thread, AccessTools.all));
                }
            }
            tag.SetGetter(getter);           
            AllTags.Add(tagAttr.Name, tag);
            if (tagAttr.NotPlaying)
                NotPlayingTags.Add(tagAttr.Name, tag);
        }
        public static void Release()
        {
            AllTags.Values.ForEach(t => t.Dispose());
            AllTags = NotPlayingTags = ReferencedTags = null;
        }
    }
}
