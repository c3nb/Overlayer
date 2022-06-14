using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Overlayer
{
    public class TagCompiler
    {
        public delegate string ValueGetter(TagCompiler compiler);
        public string source;
        public ValueGetter getter;
        public List<Tag> tagsReference;
        /// <summary>
        /// 원래는 AssemblyBuilder, ModuleBuilder, TypeBuilder, MethodBuilder로 하려 했으나 매번 바뀔때마다 컴파일 해야하므로 메모리가 터질것 같아 DynamicMethod사용
        /// </summary>
        public DynamicMethod value;
        /// <summary>
        /// DyanamicMethod에서 접근할 Tag 배열
        /// </summary>
        public Tag[] tags;
        public TagCompiler(List<Tag> tagsReference) => this.tagsReference = tagsReference;
        public TagCompiler(string source, List<Tag> tagsReference) : this(tagsReference)
            => Compile(source);
        public void Compile(string source)
        {
            this.source = source;
            value = new DynamicMethod($"ValueGetter{random.Next()}", typeof(string), new[] { typeof(TagCompiler) }); // 메서드 이름 중복으로 혹시 모를 사고 방지
            ILGenerator il = value.GetILGenerator();
            Dictionary<int, Tag> indexTag = new Dictionary<int, Tag>();
            for (int i = 0; i < tagsReference.Count; i++)
            {
                var tag = tagsReference[i];
                int index;
                for (int c = 0; c < source.Length;)
                {
                    if ((index = source.IndexOf(tag.Name, c)) >= 0) // Tag중에 일치하는 Tag이름이 있다면 그 태그의 index기억
                    {
                        indexTag.Add(index, tag);
                        c = index + tag.Name.Length;
                    }
                    else c++;
                }
            }
            StringBuilder sb = new StringBuilder(); // string 캐시
            List<object> toEmitObjs = new List<object>(); // Emit할 object
            char[] str = source.ToCharArray(); //빠른 string unit접근을 위한 ToCharArray
            for (int i = 0; i < str.Length;)
            {
                if (indexTag.TryGetValue(i, out Tag tag))
                {
                    toEmitObjs.Add(sb.ToString()); // Ldstr을 위한 캐싱
                    sb.Clear(); // 캐싱 했으니 삭제
                    toEmitObjs.Add(tag); // Tags 배열에 추가하기 위한 캐싱
                    i += tag.Name.Length; // Tag이름의 길이만큼 for인덱스 이동
                }
                else sb.Append(str[i++]); // 태그 인덱스가 아닐경우 char캐싱
            }
            toEmitObjs.Add(sb.ToString()); // 마지막으로 남아있을지 모르는 캐시를 toEmitObjs에다 캐싱
            List<Tag> tags = new List<Tag>(); // 캐싱된 태그들을 Tag[]로 쉽게 옮기기 위해 만든 List<Tag>
            int lastIndex = 0;
            Dictionary<string, int> tagIndex = new Dictionary<string, int>();
            Dictionary<int, LocalBuilder> tagLocals = new Dictionary<int, LocalBuilder>();
            int objsLength = toEmitObjs.Count;
            il.Emit(OpCodes.Ldc_I4, objsLength); // 몇개의 string이 들어갈 예정인지 capacity설정
            il.Emit(OpCodes.Newarr, typeof(string)); // string타입의 1차원 배열을 생성
            il.Emit(OpCodes.Dup); // 앞서 생성된 string의 1차원 배열을 string[]의 지역변수에 설정
            for (int i = 0; i < objsLength; i++)
            {
                il.Emit(OpCodes.Ldc_I4, i);
                object obj = toEmitObjs[i];
                if (obj is string s)
                    il.Emit(OpCodes.Ldstr, s);
                if (obj is Tag tag)
                {
                    if (!tagIndex.TryGetValue(tag.Name, out int index))
                    {
                        tags.Add(tag);
                        index = lastIndex++;
                        tagIndex[tag.Name] = index;
                        LocalBuilder loc = tagLocals[index] = il.DeclareLocal(typeof(string));
                        il.Emit(OpCodes.Ldarg_0); // 첫번째 매개변수(TagCompiler) 로드
                        il.Emit(OpCodes.Ldfld, tagsFld); // Tag[] tags필드 로드
                        il.Emit(OpCodes.Ldc_I4, index); // 배열의 인덱스 로드
                        il.Emit(OpCodes.Ldelem_Ref); // 배열의 값을 참조로드
                        il.Emit(OpCodes.Callvirt, tag_get_Value); // Tag의 Value 속성 호출
                        il.Emit(OpCodes.Stloc, loc);
                        il.Emit(OpCodes.Ldloc, loc);
                    }
                    else il.Emit(OpCodes.Ldloc, tagLocals[index]);
                }
                il.Emit(OpCodes.Stelem_Ref); // 배열에 참조 설정
                if (i != objsLength - 1) // 마지막 요소조차 Duplicate할 경우, Ret과정에서 오류가 발생하기 때문에 마지막 index인지 검사
                    il.Emit(OpCodes.Dup);
            }
            il.Emit(OpCodes.Call, string_Concats); // string.Concat 호출
            il.Emit(OpCodes.Ret);
            this.tags = tags.ToArray();
            getter = (ValueGetter)value.CreateDelegate(typeof(ValueGetter));
        }
        public string Result => getter(this);
        private static readonly FieldInfo tagsFld = typeof(TagCompiler).GetField("tags");
        private static readonly Random random = new Random(DateTime.Now.Second);
        public static readonly MethodInfo tag_get_Value = typeof(Tag).GetMethod("get_Value");
        public static readonly MethodInfo string_Concats = typeof(string).GetMethod("Concat", AccessTools.all, null, new[] { typeof(string[]) }, null);
    }
}
