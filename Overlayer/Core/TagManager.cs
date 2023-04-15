using AdofaiMapConverter;
using BLINDED_AM_ME;
using HarmonyExLib;
using Mono.Cecil;
using Overlayer.Core.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Overlayer.Core
{
    public static class TagManager
    {
        static Dictionary<string, Tag> AllTags = new Dictionary<string, Tag>();
        static Dictionary<string, Tag> NotPlayingTags = new Dictionary<string, Tag>();
        static Dictionary<string, Tag> ReferencedTags = new Dictionary<string, Tag>();
        static Dictionary<PatchInfo, List<Tag>> Patches = new Dictionary<PatchInfo, List<Tag>>(PatchInfo.Comparer);
        public static Tag GetTag(string name) => AllTags.TryGetValue(name, out var tag) ? tag : null;
        public static Tag GetReferencedTag(string name) => ReferencedTags.TryGetValue(name, out var tag) ? tag : null;
        public static bool IsReferenced(string name) => ReferencedTags.ContainsKey(name);
        public static void UpdateReference()
        {
            ReferencedTags = AllTags.Values.Where(t => t.Referenced).ToDictionary(t => t.Name, t => t);
            if (!Main.HasScripts)
            {
                foreach (var (patch, tags) in Patches)
                {
                    if (tags.All(t => !t.Referenced))
                        patch.Unpatch(Main.HarmonyEx);
                    else
                    {
                        var dead = !patch.Patch(Main.HarmonyEx);
                        tags.ForEach(t => t.Dead = dead);
                    }
                }
            }
        }
        public static void SetTag(Tag tag, bool notPlaying)
        {
            AllTags[tag.Name] = tag;
            if (notPlaying)
                NotPlayingTags[tag.Name] = tag;
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
            Patches ??= new Dictionary<PatchInfo, List<Tag>>(PatchInfo.Comparer);
        }
        public static void Load(Assembly assembly, TagConfig config = null)
        {
            foreach (Type type in assembly.GetTypes())
                Load(type, config);
        }
        public static void Load(Type type, TagConfig config = null)
        {
            Prepare();
            ClassTagAttribute cTag = type.GetCustomAttribute<ClassTagAttribute>();
            var methods = type.GetMethods(AccessTools.all);
            var fields = type.GetFields(AccessTools.all);
            if (cTag != null)
            {
                OverlayerDebug.Log($"Loading ClassTag {cTag.Name}..");
                var def = methods.FirstOrDefault(m => m.GetCustomAttribute<TagAttribute>()?.IsDefault ?? false);
                if (def == null) throw new InvalidOperationException("Default Tag Method Not Found.");
                Tag tag = new Tag(cTag.Name, config);
                foreach (var thread in cTag.GetThreads(type))
                    tag.AddThread(thread);
                tag.SetGetter(def).Build();
                AllTags.Add(cTag.Name, tag);
                if (cTag.NotPlaying)
                    NotPlayingTags.Add(cTag.Name, tag);
                if (cTag.RelatedPatches != null)
                    AddPatches(tag, cTag.RelatedPatches);
            }
            foreach (MethodInfo method in methods)
            {
                TagAttribute tagAttr = method.GetCustomAttribute<TagAttribute>();
                if (tagAttr == null) continue;
                tagAttr.Name = tagAttr.Name ?? method.Name;
                OverlayerDebug.Log($"Loading Tag {tagAttr.Name}..");
                Tag tag = new Tag(tagAttr.Name, config);
                tag.SetGetter(method).Build();
                AllTags.Add(tagAttr.Name, tag);
                if (tagAttr.NotPlaying)
                    NotPlayingTags.Add(tagAttr.Name, tag);
                if (tagAttr.RelatedPatches != null)
                    AddPatches(tag, tagAttr.RelatedPatches);
            }
            foreach (FieldInfo field in fields)
            {
                FieldTagAttribute tagAttr = field.GetCustomAttribute<FieldTagAttribute>();
                if (tagAttr == null) continue;
                tagAttr.Name = tagAttr.Name ?? field.Name;
                OverlayerDebug.Log($"Loading FieldTag {tagAttr.Name}..");
                Tag tag = new Tag(tagAttr.Name, config);
                Delegate func;
                if (tagAttr.Processor == null)
                    func = GenerateFieldTagWrapper(tagAttr, field, out _);
                else
                {
                    MethodInfo processor = AccessTools.Method(tagAttr.Processor);
                    func = GenerateFieldTagProcessingWrapper(tagAttr, field, processor, out _);
                }
                tag.SetGetter(func);
                tag.Build();
                AllTags.Add(tagAttr.Name, tag);
                if (tagAttr.NotPlaying)
                    NotPlayingTags.Add(tagAttr.Name, tag);
                if (tagAttr.RelatedPatches != null)
                    AddPatches(tag, tagAttr.RelatedPatches);
            }
        }
        public static void Release()
        {
            AllTags.Values.ForEach(t => t.Dispose());
            AllTags = NotPlayingTags = ReferencedTags = null;
            Patches = null;
        }
        static void AddPatches(Tag tag, string patchNames)
        {
            foreach (var info in ParsePatchNames(patchNames))
            {
                if (Patches.TryGetValue(info, out var tags))
                    tags.Add(tag);
                else Patches.Add(info, new List<Tag> { tag });
                OverlayerDebug.Log($"Added Patch {info}..");
            }
        }
        static List<PatchInfo> ParsePatchNames(string patchNames)
        {
            OverlayerDebug.Log($"Parsing Patch Names..");
            string[] patches = patchNames.Split('|');
            List<PatchInfo> pInfos = new List<PatchInfo>();
            foreach (string patch in patches)
            {
                PatchInfo pInfo;
                var type = AccessTools.TypeByName(patch);
                if (type != null)
                    pInfo = new PatchInfo(type);
                else
                {
                    string[] split = patch.Split2(':');
                    pInfo = new PatchInfo(AccessTools.TypeByName(split[0]).GetMethod(split[1], AccessTools.all));
                }
                pInfos.Add(pInfo);
            }
            return pInfos;
        }
        static Delegate GenerateFieldTagWrapper(FieldTagAttribute fTag, FieldInfo field, out DynamicMethod dm)
        {
            ILGenerator il;
            if (fTag.Round)
            {
                dm = new DynamicMethod($"{fTag.Name}Tag_Wrapper_Opt", field.FieldType, new[] { typeof(int) }, true);
                dm.DefineParameter(1, System.Reflection.ParameterAttributes.None, "digits");
                il = dm.GetILGenerator();
                il.Emit(OpCodes.Ldsfld, field);
                if (field.FieldType != typeof(double))
                    il.Emit(OpCodes.Conv_R8);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, round);
                if (field.FieldType != typeof(double))
                    il.Convert(field.FieldType);
                il.Emit(OpCodes.Ret);
                return dm.CreateDelegate(Expression.GetFuncType(new[] { typeof(int), field.FieldType }));
            }
            dm = new DynamicMethod($"{fTag.Name}Tag_Wrapper", field.FieldType, Type.EmptyTypes, true);
            il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, field);
            il.Emit(OpCodes.Ret);
            return dm.CreateDelegate(Expression.GetFuncType(field.FieldType));
        }
        static Delegate GenerateFieldTagProcessingWrapper(FieldTagAttribute fTag, FieldInfo field, MethodInfo processor, out DynamicMethod dm)
        {
            dm = null;
            ILGenerator il;
            var prms = processor.GetParameters();
            if (prms[0].ParameterType != field.FieldType) return null;
            dm = new DynamicMethod($"{fTag.Name}Tag_Wrapper_Processor", processor.ReturnType, new[] { prms[1].ParameterType }, true);
            il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, field);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, processor);
            il.Emit(OpCodes.Ret);
            return dm.CreateDelegate(Expression.GetFuncType(new[] { prms[1].ParameterType, processor.ReturnType }));
        }
        static readonly MethodInfo round = typeof(Utility).GetMethod("Round", new[] { typeof(double), typeof(int) });
    }
}
