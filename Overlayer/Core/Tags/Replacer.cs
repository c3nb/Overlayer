using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace Overlayer.Core.Tags
{
    public class Replacer
    {
        bool compiled;
        string str = "";
        HashSet<char> tagOpenChars;
        Func<string> compiledResult;
        IEnumerable<Tag> tags;
        public string Source
        {
            get => str;
            set
            {
                compiled = str == value;
                str = value;
            }
        }
        public HashSet<Tag> References { get; private set; }
        public IEnumerable<Tag> Tags => tags;
        public Replacer(IEnumerable<Tag> tags = null)
        {
            References = new HashSet<Tag>();
            SetReference(tags ?? new List<Tag>());
        }
        public Replacer(string str, IEnumerable<Tag> tags = null) : this(tags) => Source = str;
        public void SetReference(IEnumerable<Tag> tags)
        {
            if (tags == null) return;
            this.tags = tags;
            tagOpenChars = tags.Select(t => t.Config.Open).ToHashSet();
            compiled = false;
        }
        public string Replace()
        {
            Compile();
            return compiledResult();
        }
        public Replacer Compile()
        {
            if (compiled && compiledResult != null) return this;
            foreach (var tag in References)
                tag.ReferencedCount--;
            References = new HashSet<Tag>();
            DynamicMethod result = new DynamicMethod("", typeof(string), Type.EmptyTypes, typeof(Replacer), true);
            ILGenerator il = result.GetILGenerator();
            StringBuilder stack = new StringBuilder();
            List<object> emits = new List<object>();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                stack.Append(c);
                if (tagOpenChars.Contains(c) || char.IsWhiteSpace(c) || c == char.MinValue)
                {
                    var info = ParseTag(c, ref i);
                    if (info != null)
                    {
                        if (References.Add(info.tag))
                            info.tag.ReferencedCount++;
                        stack.Remove(stack.Length - 1, 1);
                        emits.Add(stack.ToString());
                        emits.Add(info);
                        stack.Clear();
                        i++;
                    }
                }
            }
            emits.Add(stack.ToString());
            //int arrIndex = 0;
            //il.Emit(OpCodes.Ldc_I4, emits.Count);
            //il.Emit(OpCodes.Newarr, typeof(string));
            il.Emit(OpCodes.Newobj, StrBuilder_Ctor);
            foreach (object emit in emits)
            {
                //il.Emit(OpCodes.Dup);
                //il.Emit(OpCodes.Ldc_I4, arrIndex++);
                if (emit is string str)
                    il.Emit(OpCodes.Ldstr, str);
                if (emit is TagInfo info)
                {
                    if (info.tag.HasOption)
                    {
                        if (info.option != null)
                        {
                            il.Emit(OpCodes.Ldstr, info.option);
                            if (info.tag.OptionConverter != null)
                                il.Emit(OpCodes.Call, info.tag.OptionConverter);
                        }
                        else
                        {
                            if (info.tag.OptionConverter != null)
                                EmitDefaultValue(il, info.tag.Getter.GetParameters()[0]);
                            else il.Emit(OpCodes.Ldnull);
                        }
                    }
                    il.Emit(OpCodes.Call, info.tag.Getter);
                    if (info.tag.ReturnConverter != null)
                        il.Emit(OpCodes.Call, info.tag.ReturnConverter);
                }
                //il.Emit(OpCodes.Stelem_Ref);
                il.Emit(OpCodes.Call, StrBuilder_Append);
            }
            //il.Emit(OpCodes.Call, Concats);
            il.Emit(OpCodes.Call, StrBuilder_ToString);
            il.Emit(OpCodes.Ret);
            compiledResult = (Func<string>)result.CreateDelegate(typeof(Func<string>));
            compiled = true;
            return this;
        }
        TagInfo ParseTag(char open, ref int index)
        {
            var t = tags.Where(tag => tag.Config.Open == open);
            if (!t.Any()) return null;
            foreach (Tag tag in t)
            {
                int closeIndex = str.IndexOf(tag.Config.Close, index);
                if (closeIndex < 0) continue;
                string subStr = str.Substring(index + 1, closeIndex - index - 1);
                string[] nameOpt = subStr.Split(new char[] { tag.Config.Separator }, 2);
                if (nameOpt[0] != tag.Name) continue;
                index += closeIndex - index - 1;
                return new TagInfo(tag, nameOpt.Length < 2 ? null : nameOpt[1]);
            }
            return null;
        }
        static void EmitDefaultValue(ILGenerator il, ParameterInfo param)
        {
            if (param.DefaultValue != DBNull.Value)
            {
                switch (Type.GetTypeCode(param.ParameterType))
                {
                    case TypeCode.Boolean:
                        il.Emit(OpCodes.Ldc_I4, (bool)param.DefaultValue ? 1 : 0);
                        break;
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        il.Emit(OpCodes.Ldc_I4, (int)param.DefaultValue);
                        break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        il.Emit(OpCodes.Ldc_I8, (long)param.DefaultValue);
                        break;
                    case TypeCode.Single:
                        il.Emit(OpCodes.Ldc_R4, (float)param.DefaultValue);
                        break;
                    case TypeCode.Double:
                        il.Emit(OpCodes.Ldc_R8, (double)param.DefaultValue);
                        break;
                    default:
                        throw new NotSupportedException($"Emitting {param}'s Default Value Is Not Supported");
                }
            }
            else
            {
                switch (Type.GetTypeCode(param.ParameterType))
                {
                    case TypeCode.Boolean:
                        il.Emit(OpCodes.Ldc_I4, 0);
                        break;
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                        il.Emit(OpCodes.Ldc_I4, -1);
                        break;
                    case TypeCode.Byte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                        il.Emit(OpCodes.Ldc_I4, 0);
                        break;
                    case TypeCode.Int64:
                        il.Emit(OpCodes.Ldc_I8, -1);
                        break;
                    case TypeCode.UInt64:
                        il.Emit(OpCodes.Ldc_I8, 0);
                        break;
                    case TypeCode.Single:
                        il.Emit(OpCodes.Ldc_R4, -1);
                        break;
                    case TypeCode.Double:
                        il.Emit(OpCodes.Ldc_R8, -1);
                        break;
                    default:
                        throw new NotSupportedException($"Emitting {param}'s Default Value Is Not Supported");
                }
            }
        }
        class TagInfo
        {
            public Tag tag;
            public string option;
            public TagInfo(Tag tag, string option)
            {
                this.tag = tag;
                this.option = option;
            }
        }
        public static class Wrapper
        {
            public static readonly AssemblyBuilder ass;
            public static readonly ModuleBuilder mod;
            public static int TypeCount { get; internal set; }
            static Wrapper()
            {
                var assName = new AssemblyName("Wrapper");
                ass = AssemblyBuilder.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);
                mod = ass.DefineDynamicModule(assName.Name);
            }
            public static MethodInfo Wrap<T>(T del) where T : Delegate
            {
                Type delType = del.GetType();
                MethodInfo invoke = delType.GetMethod("Invoke");
                MethodInfo method = del.Method;
                TypeBuilder type = mod.DefineType(TypeCount++.ToString(), TypeAttributes.Public);
                ParameterInfo[] parameters = method.GetParameters();
                Type[] paramTypes = parameters.Select(p => p.ParameterType).ToArray();
                MethodBuilder methodB = type.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, invoke.ReturnType, paramTypes);
                FieldBuilder delField = type.DefineField("function", delType, FieldAttributes.Public | FieldAttributes.Static);
                ILGenerator il = methodB.GetILGenerator();
                il.Emit(OpCodes.Ldsfld, delField);
                int paramIndex = 1;
                foreach (ParameterInfo param in parameters)
                {
                    methodB.DefineParameter(paramIndex++, ParameterAttributes.None, param.Name);
                    il.Emit(OpCodes.Ldarg, paramIndex - 2);
                }
                il.Emit(OpCodes.Call, invoke);
                il.Emit(OpCodes.Ret);
                Type t = type.CreateType();
                t.GetField("function").SetValue(null, del);
                return t.GetMethod("Wrapper");
            }
            public static Func<object> WrapFast(MethodInfo m)
            {
                TypeBuilder type = mod.DefineType(TypeCount++.ToString(), TypeAttributes.Public);
                Type rt = m.ReturnType;
                MethodBuilder methodB = type.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, typeof(object), Type.EmptyTypes);
                ILGenerator il = methodB.GetILGenerator();
                il.Emit(OpCodes.Call, m);
                il.Emit(OpCodes.Ret);
                if (rt != typeof(object))
                    il.Emit(OpCodes.Box, rt);
                return (Func<object>)type.CreateType().GetMethod("Wrapper").CreateDelegate(typeof(Func<object>));
            }
            public static Func<object, object> WrapFastOpt(MethodInfo m)
            {
                TypeBuilder type = mod.DefineType(TypeCount++.ToString(), TypeAttributes.Public);
                var p = m.GetParameters().First();
                Type pt = p.ParameterType;
                Type rt = m.ReturnType;
                MethodBuilder methodB = type.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, typeof(object), new[] { typeof(object) });
                ILGenerator il = methodB.GetILGenerator();
                methodB.DefineParameter(1, ParameterAttributes.None, p.Name);
                il.Emit(OpCodes.Ldarg_0);
                if (pt != typeof(object))
                    il.Emit(OpCodes.Call, GetConvert(pt));
                il.Emit(OpCodes.Call, m);
                if (rt != typeof(object))
                    il.Emit(OpCodes.Box, rt);
                il.Emit(OpCodes.Ret);
                return (Func<object, object>)type.CreateType().GetMethod("Wrapper").CreateDelegate(typeof(Func<object, object>));
            }
            public static MethodInfo GetConvert(Type to) => to.IsPrimitive ? typeof(Convert).GetMethod($"To{to.Name}", new[] { typeof(object) }) : to == typeof(string) ? ts : null;
            public static string ToString(object o) => o.ToString();
            static readonly MethodInfo ts = typeof(Wrapper).GetMethod("ToString", new[] { typeof(object) });
        }
        //public static readonly MethodInfo Concats = typeof(string).GetMethod("Concat", new[] { typeof(string[]) });
        public static readonly ConstructorInfo StrBuilder_Ctor = typeof(StringBuilder).GetConstructor(Type.EmptyTypes);
        public static readonly MethodInfo StrBuilder_Append = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });
        public static readonly MethodInfo StrBuilder_ToString = typeof(StringBuilder).GetMethod("ToString", Type.EmptyTypes);
    }
}
