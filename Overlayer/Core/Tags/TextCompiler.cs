using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Overlayer.Core.Utils;

namespace Overlayer.Core
{
    public class TextCompiler
    {
        public static bool IsReferencing(string name)
        {
            for (int i = 0; i < ReferencingTags.Count; i++)
                if (ReferencingTags[i].Name == name)
                    return true;
            return false;
        }
        public static List<Tag> ReferencingTags { get; private set; } = new List<Tag>();
        public static void UpdateReferences()
            => ReferencingTags = ReferencingTags.Distinct().ToList();
        public delegate string ValueGetter(TextCompiler compiler);
        public string source;
        public ValueGetter getter;
        public TagCollection tagsReference;
        public DynamicMethod value;
        public Tag[] tags;
        public TextCompiler(TagCollection tagsReference) => this.tagsReference = tagsReference;
        public TextCompiler(string source, TagCollection tagsReference) : this(tagsReference) => Compile(source);
        public void Compile(string source)
        {
            this.source = source;
            IEnumerable<TToken> tokens = TToken.Tokenize(source);
            int objsLength = tokens.Count();
            if (objsLength <= 0)
            {
                getter = t => "";
                return;
            }
            value = new DynamicMethod($"ValueGetter{random.Next()}", typeof(string), new[] { typeof(TextCompiler) });
            ILGenerator il = value.GetILGenerator();
            this.tags = new Tag[0];
            List<Tag> tags = new List<Tag>();
            int lastIndex = 0;
            Dictionary<string, int> tokRawIndex = new Dictionary<string, int>();
            Dictionary<int, LocalBuilder> tagLocals = new Dictionary<int, LocalBuilder>();
            il.Emit(OpCodes.Ldc_I4, objsLength); 
            il.Emit(OpCodes.Newarr, typeof(string));
            il.Emit(OpCodes.Dup); 
            int idx = 0;
            foreach (TToken token in tokens)
            {
                il.Emit(OpCodes.Ldc_I4, idx);
                var tag = tagsReference[token.Text];
                if (token.Closed)
                {
                    if (tag != null)
                    {
                        if (!tokRawIndex.TryGetValue(token.Raw, out int index))
                        {
                            tags.Add(tag);
                            index = lastIndex++;
                            tokRawIndex[token.Raw] = index;
                            LocalBuilder loc = tagLocals[index] = il.DeclareLocal(typeof(string));
                            il.Emit(OpCodes.Ldarg_0); 
                            il.Emit(OpCodes.Ldfld, tagsFld); 
                            il.Emit(OpCodes.Ldc_I4, index); 
                            il.Emit(OpCodes.Ldelem_Ref);
                            var t = Push(tag, token);
                            Format(tag, token, t);
                            il.Emit(OpCodes.Stloc, loc);
                            il.Emit(OpCodes.Ldloc, loc);
                        }
                        else il.Emit(OpCodes.Ldloc, tagLocals[index]);
                    }
                    else il.Emit(OpCodes.Ldstr, token.Raw);
                }
                else il.Emit(OpCodes.Ldstr, token.Raw);
                il.Emit(OpCodes.Stelem_Ref);
                if (idx != objsLength - 1) 
                    il.Emit(OpCodes.Dup);
                idx++;
            }
            il.Emit(OpCodes.Call, string_Concats); 
            il.Emit(OpCodes.Ret);
            this.tags = tags.ToArray();
            ReferencingTags.AddRange(tags);
            UpdateReferences();
            getter = (ValueGetter)value.CreateDelegate(typeof(ValueGetter));
            LocalBuilder Push(Tag tag, TToken token)
            {
                if (tag.IsOpt)
                {
                    if (tag.IsString)
                    {
                        LocalBuilder str = il.DeclareLocal(typeof(string));
                        if (tag.IsStringOpt)
                        {
                            var result = token.HasOption ? token.Option : tag.DefOptStr == null ? "" : tag.DefOptStr;
                            il.Emit(OpCodes.Ldstr, result);
                            il.Emit(OpCodes.Callvirt, tag_optValue_String);
                            il.Emit(OpCodes.Stloc, str);
                        }
                        else
                        {
                            var result = token.HasOption ? token.Option.ToFloat() : tag.DefOptNum == null ? 0 : tag.DefOptNum.Value;
                            il.Emit(OpCodes.Ldc_R8, result);
                            il.Emit(OpCodes.Callvirt, tag_optValue_Float);
                            il.Emit(OpCodes.Stloc, str);
                        }
                        return str;
                    }
                    else
                    {
                        LocalBuilder num = il.DeclareLocal(typeof(double));
                        if (tag.IsStringOpt)
                        {
                            var result = token.HasOption ? token.Option : tag.DefOptStr == null ? "" : tag.DefOptStr;
                            il.Emit(OpCodes.Ldstr, result);
                            il.Emit(OpCodes.Callvirt, tag_optValueFloat_String);
                            il.Emit(OpCodes.Stloc, num);
                        }
                        else
                        {
                            var result = token.HasOption ? token.Option.ToFloat() : tag.DefOptNum == null ? 0 : tag.DefOptNum.Value;
                            il.Emit(OpCodes.Ldc_R8, result);
                            il.Emit(OpCodes.Callvirt, tag_optValueFloat_Float);
                            il.Emit(OpCodes.Stloc, num);
                        }
                        return num;
                    }
                }
                else
                {
                    if (tag.IsString)
                    {
                        LocalBuilder str = il.DeclareLocal(typeof(string));
                        il.Emit(OpCodes.Callvirt, tag_string_Value);
                        il.Emit(OpCodes.Stloc, str);
                        return str;
                    }
                    else
                    {
                        LocalBuilder num = il.DeclareLocal(typeof(double));
                        il.Emit(OpCodes.Callvirt, tag_double_Value);
                        il.Emit(OpCodes.Stloc, num);
                        return num;
                    }
                }
            }
            void Format(Tag tag, TToken token, LocalBuilder loc)
            {
                if (token.HasFormat)
                {
                    if (tag.IsString)
                    {
                        LocalBuilder arr = il.DeclareLocal(typeof(object[]));
                        il.Emit(OpCodes.Ldc_I4, 1);
                        il.Emit(OpCodes.Newarr, typeof(object));
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Stloc, arr);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ldloc, loc);
                        il.Emit(OpCodes.Stelem_Ref);

                        il.Emit(OpCodes.Ldstr, token.Format);
                        il.Emit(OpCodes.Ldloc, arr);
                        il.Emit(OpCodes.Call, string_Format);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloca, loc);
                        il.Emit(OpCodes.Ldstr, token.Format);
                        il.Emit(OpCodes.Call, double_ToString);
                    }
                    return;
                }
                if (tag.IsString) 
                    il.Emit(OpCodes.Ldloc, loc);
                else
                {
                    il.Emit(OpCodes.Ldloca, loc);
                    il.Emit(OpCodes.Call, double_ToString_Empty);
                    double s = 3;
                    s.ToString();
                }
            }
        }
        public string Result => getter(this);
        internal static readonly Random random = new Random(DateTime.Now.Millisecond);
        private static readonly FieldInfo tagsFld = typeof(TextCompiler).GetField("tags");
        public static readonly MethodInfo tag_string_Value = typeof(Tag).GetMethod("StringValue");
        public static readonly MethodInfo tag_double_Value = typeof(Tag).GetMethod("FloatValue");
        public static readonly MethodInfo string_Format = typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object[]) });
        public static readonly MethodInfo double_ToString = typeof(double).GetMethod("ToString", new[] { typeof(string) });
        public static readonly MethodInfo double_ToString_Empty = typeof(double).GetMethod("ToString", Type.EmptyTypes);
        public static readonly MethodInfo tag_optValue_Float = typeof(Tag).GetMethod("OptValue", new[] { typeof(double) });
        public static readonly MethodInfo tag_optValueFloat_Float = typeof(Tag).GetMethod("OptValueFloat", new[] { typeof(double) });
        public static readonly MethodInfo tag_optValue_String = typeof(Tag).GetMethod("OptValue", new[] { typeof(string) });
        public static readonly MethodInfo tag_optValueFloat_String = typeof(Tag).GetMethod("OptValueFloat", new[] { typeof(string) });
        public static readonly MethodInfo string_Concats = typeof(string).GetMethod("Concat", AccessTools.all, null, new[] { typeof(string[]) }, null);
    }
}
