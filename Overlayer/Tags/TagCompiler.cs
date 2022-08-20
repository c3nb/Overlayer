using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Overlayer.Utils;

namespace Overlayer.Tags
{
    public class TagCompiler
    {
        public static bool IsReferencing(string name)
        {
            for (int i = 0; i < ReferencingTags.Length; i++)
                if (ReferencingTags[i].Name == name)
                    return true;
            return false;
        }
        public static Tag[] ReferencingTags = new Tag[0];
        public static void UpdateReferences()
        {
            ReferencingTags = OText.Texts
                .SelectMany(t => new[] 
                { 
                    t.PlayingCompiler, 
                    t.NotPlayingCompiler,
                })
                .SelectMany(c => c.tags)
                .Distinct().ToArray();
        }
        public delegate string ValueGetter(TagCompiler compiler);
        public string source;
        public ValueGetter getter;
        public TagCollection tagsReference;
        /// <summary>
        /// 원래는 AssemblyBuilder, ModuleBuilder, TypeBuilder, MethodBuilder로 하려 했으나 매번 바뀔때마다 컴파일 해야하므로 메모리가 터질것 같아 DynamicMethod사용
        /// </summary>
        public DynamicMethod value;
        /// <summary>
        /// DyanamicMethod에서 접근할 Tag 배열
        /// </summary>
        public Tag[] tags;
        public TagCompiler(TagCollection tagsReference) => this.tagsReference = tagsReference;
        public TagCompiler(string source, TagCollection tagsReference) : this(tagsReference) => Compile(source);
        public void Compile(string source)
        {
            this.source = source;
            IEnumerable<Token> tokens = Token.Tokenize(source);
            int objsLength = tokens.Count();
            if (objsLength <= 0)
            {
                getter = t => "";
                return;
            }
            value = new DynamicMethod($"ValueGetter{random.Next()}", typeof(string), new[] { typeof(TagCompiler) }); // 메서드 이름 중복으로 혹시 모를 사고 방지
            ILGenerator il = value.GetILGenerator();
            this.tags = new Tag[0];
            List<Tag> tags = new List<Tag>(); // 캐싱된 태그들을 Tag[]로 쉽게 옮기기 위해 만든 List<Tag>
            int lastIndex = 0;
            Dictionary<string, int> tokRawIndex = new Dictionary<string, int>();
            Dictionary<int, LocalBuilder> tagLocals = new Dictionary<int, LocalBuilder>();
            il.Emit(OpCodes.Ldc_I4, objsLength); // 몇개의 string이 들어갈 예정인지 capacity설정
            il.Emit(OpCodes.Newarr, typeof(string)); // string타입의 1차원 배열을 생성
            il.Emit(OpCodes.Dup); // 앞서 생성된 string의 1차원 배열을 string[]의 지역변수에 설정
            int idx = 0;
            foreach (Token token in tokens)
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
                            il.Emit(OpCodes.Ldarg_0); // 첫번째 매개변수(TagCompiler) 로드
                            il.Emit(OpCodes.Ldfld, tagsFld); // Tag[] tags필드 로드
                            il.Emit(OpCodes.Ldc_I4, index); // 배열의 인덱스 로드
                            il.Emit(OpCodes.Ldelem_Ref); // 배열의 값을 참조로드
                            if (tag.IsOpt)
                            {
                                if (token.HasOption)
                                {
                                    if (tag.IsStringOpt)
                                    {
                                        il.Emit(OpCodes.Ldstr, token.Option);
                                        il.Emit(OpCodes.Callvirt, tag_optValue_String);
                                    }
                                    else
                                    {
                                        float.TryParse(token.Option, out float result);
                                        il.Emit(OpCodes.Ldc_R4, result);
                                        il.Emit(OpCodes.Callvirt, tag_optValue_Float);
                                    }
                                }
                                else
                                {
                                    if (tag.IsStringOpt)
                                    {
                                        if (tag.DefOptStr != null)
                                            il.Emit(OpCodes.Ldstr, tag.DefOptStr);
                                        else il.Emit(OpCodes.Ldnull);
                                        il.Emit(OpCodes.Callvirt, tag_optValue_String);
                                    }
                                    else
                                    {
                                        if (tag.DefOptNum.HasValue)
                                            il.Emit(OpCodes.Ldc_R4, tag.DefOptNum.Value);
                                        else il.Emit(OpCodes.Ldc_R4, 0);
                                        il.Emit(OpCodes.Callvirt, tag_optValue_Float);
                                    }
                                }
                            }
                            else il.Emit(OpCodes.Callvirt, tag_get_Value); // Tag의 Value 속성 호출
                            il.Emit(OpCodes.Stloc, loc);
                            il.Emit(OpCodes.Ldloc, loc);
                        }
                        else il.Emit(OpCodes.Ldloc, tagLocals[index]);
                    }
                    else il.Emit(OpCodes.Ldstr, token.Raw);
                }
                else il.Emit(OpCodes.Ldstr, token.Raw);
                il.Emit(OpCodes.Stelem_Ref); // 배열에 참조 설정
                if (idx != objsLength - 1) // 마지막 요소조차 Duplicate할 경우, Ret과정에서 오류가 발생하기 때문에 마지막 index인지 검사
                    il.Emit(OpCodes.Dup);
                idx++;
            }
            il.Emit(OpCodes.Call, string_Concats); // string.Concat 호출
            il.Emit(OpCodes.Ret);
            this.tags = tags.ToArray();
            UpdateReferences();
            getter = (ValueGetter)value.CreateDelegate(typeof(ValueGetter));
        }
        public string Result => getter(this);
        internal static readonly Random random = new Random(DT.Now.Second);
        private static readonly FieldInfo tagsFld = typeof(TagCompiler).GetField("tags");

        public static readonly MethodInfo tag_get_Value = typeof(Tag).GetMethod("get_Value");
        public static readonly MethodInfo tag_optValue_Float = typeof(Tag).GetMethod("OptValue", new[] { typeof(float) });
        public static readonly MethodInfo tag_optValue_String = typeof(Tag).GetMethod("OptValue", new[] { typeof(string) });
        public static readonly MethodInfo string_Concats = typeof(string).GetMethod("Concat", AccessTools.all, null, new[] { typeof(string[]) }, null);
    }
}
