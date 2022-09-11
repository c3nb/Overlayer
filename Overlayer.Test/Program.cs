using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using TagLib.Utils;
using Overlayer.AdofaiggApi;
using Overlayer.Tags.Global;
using Overlayer.AdofaiggApi.Types;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections;
using System.Xml.Serialization;
using CommandLine;
using System.Xml.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json.Linq;
using HarmonyLib;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata.Ecma335;

namespace Overlayer.Test
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //BenchmarkRunner.Run<Benchmark>();
        }
        public static readonly MethodInfo prefix = typeof(Program).GetMethod(nameof(Prefix));
        public static bool Prefix(ref bool __result)
            => !(__result = true);
    }
    public static class Serializer
    {
        public static string Serialize(object obj)
            => new SerializerInternal(obj).Serialize();
        class SerializerInternal
        {
            public object obj;
            public List<object> references;
            public SerializerInternal(object obj)
            {
                this.obj = obj;
                references = new List<object>();
            }
            SerializerInternal(object obj, List<object> references)
            {
                this.obj = obj;
                this.references = references;
            }
            public string Serialize()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(obj.GetType().FullName);
                Serialize(sb, 0);
                return sb.ToString();
            }
            private void Serialize(StringBuilder sb, int scope)
            {
                if (obj == null)
                {
                    sb.Append(' ', scope * 4).AppendLine("null");
                    return;
                }
                if (AppendValue(sb, scope, obj))
                    return;
                FieldInfo[] fields = GetAllFields(obj.GetType()).ToArray();
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    string name = field.Name;
                    object fValue = field.GetValue(obj);
                    if (fValue == null)
                    {
                        sb.Append(' ', scope * 4).AppendLine($"{field.FieldType}:{name}=null");
                        continue;
                    }
                    Type type = fValue.GetType();
                    if (type == typeof(bool) ||
                        type == typeof(sbyte) ||
                        type == typeof(byte) ||
                        type == typeof(short) ||
                        type == typeof(ushort) ||
                        type == typeof(int) ||
                        type == typeof(uint) ||
                        type == typeof(long) ||
                        type == typeof(ulong) ||
                        type == typeof(float) ||
                        type == typeof(double) ||
                        type == typeof(decimal) ||
                        type == typeof(IntPtr) ||
                        type == typeof(UIntPtr))
                        sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}={fValue}");
                    else if (type == typeof(string))
                        sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}=\"{fValue}\"");
                    else if (type.IsPointer)
                        sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}={(IntPtr)fValue}");
                    else if (type == typeof(char))
                        sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}='{fValue}'");
                    else if (type == typeof(Type) || type.FullName == "System.RuntimeType")
                        sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}={type.FullName}");
                    else if (fValue is MemberInfo member)
                        sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}={FormatMember(member)}");
                    else if (fValue is Delegate d)
                        sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}={d.Method}");
                    else if (type.IsEnum)
                        sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}={fValue}");
                    else if (type.IsArray)
                    {
                        sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}=[");
                        Array arr = (Array)fValue;
                        foreach (object o in arr)
                        {
                            SerializerInternal flds = new SerializerInternal(o, references);
                            if (!AppendValue(sb, scope + 1, o))
                            {
                                sb.Append(' ', scope * 4 + 4).AppendLine("{");
                                flds.Serialize(sb, scope + 2);
                                sb.Append(' ', scope * 4 + 4).AppendLine("}");
                            }
                        }
                        sb.Append(' ', scope * 4).AppendLine("]");
                    }
                    else
                    {
                        var index = references.FindIndex(o => o.GetType().IsClass ? ReferenceEquals(o, fValue) : Equals(o, fValue));
                        if (index >= 0)
                        {
                            sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}={{{index}}}");
                            continue;
                        }
                        else references.Add(fValue);
                        SerializerInternal flds = new SerializerInternal(fValue, references);
                        sb.Append(' ', scope * 4).AppendLine($"{type.FullName}:{name}={{");
                        flds.Serialize(sb, scope + 1);
                        sb.Append(' ', scope * 4).AppendLine("}");
                    }
                }
            }
            private bool AppendValue(StringBuilder sb, int scope, object obj)
            {
                if (obj == null)
                {
                    sb.Append(' ', scope * 4).AppendLine("null");
                    return true;
                }
                bool appended;
                Type type = obj.GetType();
                if (appended = type == typeof(bool) ||
                        type == typeof(sbyte) ||
                        type == typeof(byte) ||
                        type == typeof(short) ||
                        type == typeof(ushort) ||
                        type == typeof(int) ||
                        type == typeof(uint) ||
                        type == typeof(long) ||
                        type == typeof(ulong) ||
                        type == typeof(float) ||
                        type == typeof(double) ||
                        type == typeof(decimal) ||
                        type == typeof(IntPtr) ||
                        type == typeof(UIntPtr))
                    sb.Append(' ', scope * 4).AppendLine(obj.ToString());
                else if (appended = type == typeof(string))
                    sb.Append(' ', scope * 4).AppendLine($"\"{obj}\"");
                else if (appended = type.IsPointer)
                    sb.Append(' ', scope * 4).AppendLine($"{(IntPtr)obj}");
                else if (appended = type == typeof(char))
                    sb.Append(' ', scope * 4).AppendLine($"'{obj}'");
                else if (appended = type == typeof(Type) || type.FullName == "System.RuntimeType")
                    sb.Append(' ', scope * 4).AppendLine(type.FullName);
                else if (appended = typeof(MemberInfo).IsAssignableFrom(type))
                    sb.Append(' ', scope * 4).AppendLine(FormatMember((MemberInfo)obj));
                else if (obj is Delegate d)
                {
                    sb.Append(' ', scope * 4).AppendLine(d.Method.ToString());
                    appended = true;
                }
                else if (appended = type.IsEnum)
                    sb.Append(' ', scope * 4).AppendLine(obj.ToString());
                else if (appended = type.IsArray)
                {
                    sb.Append(' ', scope * 4).AppendLine("[");
                    Array arr = (Array)obj;
                    foreach (object o in arr)
                    {
                        SerializerInternal flds = new SerializerInternal(o, references);
                        sb.Append(' ', scope * 4).AppendLine("{");
                        flds.Serialize(sb, scope + 1);
                        sb.Append(' ', scope * 4).AppendLine("}");
                    }
                    sb.Append(' ', scope * 4).AppendLine("]");
                }
                return appended;
            }
        }
        public static object Deserialize(string src, params Assembly[] references)
            => new DeserializerInternal(src, references).Deserialize();
        class DeserializerInternal
        {
            public string src;
            public List<object> references;
            public IEnumerable<Assembly> refAsms;
            public DeserializerInternal(string src, IEnumerable<Assembly> refAsms)
            {
                this.src = src;
                references = new List<object>();
                this.refAsms = refAsms;
            }
            DeserializerInternal(string src, List<object> references)
            {
                this.src = src;
                this.references = references;
            }
            public object Deserialize()
            {
                using (StringReader sr = new StringReader(src))
                {
                    var line = sr.ReadLine();
                    var objType = ResolveType(line);
                    if (objType == typeof(bool) ||
                        objType == typeof(sbyte) ||
                        objType == typeof(byte) ||
                        objType == typeof(short) ||
                        objType == typeof(ushort) ||
                        objType == typeof(int) ||
                        objType == typeof(uint) ||
                        objType == typeof(long) ||
                        objType == typeof(ulong) ||
                        objType == typeof(float) ||
                        objType == typeof(double) ||
                        objType == typeof(decimal) ||
                        objType == typeof(IntPtr) ||
                        objType == typeof(UIntPtr))
                        return ParseNumber(objType, sr.ReadLine());
                    else if (objType == typeof(string) || objType == typeof(char))
                        return TrimOne(sr.ReadLine());
                    else if (objType == typeof(Type) || objType.FullName == "System.RuntimeType")
                        return ResolveType(sr.ReadLine());
                    else if (typeof(MemberInfo).IsAssignableFrom(objType))
                    {
                        var str = sr.ReadLine();
                        var retTypeStr = str.Split(' ')[0];
                        var nameStr = str.Remove(str.IndexOf(retTypeStr), retTypeStr.Length + 1);
                        Console.WriteLine(nameStr);
                        if (str.Contains("(") && str.Contains(")"))
                        {
                            var realName = nameStr.Substring(0, str.IndexOf("("));
                            var split = realName.Split('.');
                            var name = split.Last();
                            var decType = ResolveType(realName.Replace('.' + name, ""));
                            var paramString = nameStr.Replace(realName, "").Replace("(", "").Replace(")", "");
                            var method = decType.GetMethod(name, (BindingFlags)15420, null, ParseParameters(paramString, ResolveType), null);
                            return method;
                        }
                        var fSplit = nameStr.Split('.');
                        var fName = fSplit.Last();
                        var fDecType = nameStr.Replace('.' + fName, "");
                        return ResolveType(fDecType).GetField(fName, (BindingFlags)15420);
                    }
                    var instance = CreateInstance(objType);
                    return Deserialize(sr, 0, null, instance);
                }
            }
            private object Deserialize(StringReader sr, int scope, string name, object obj)
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string trim = TrimStart(line, out int count);
                    int tScope = count / 4;
                    string[] split = trim.Split(':');
                    Type type = ResolveType(split[0]);
                    string[] split2 = split[1].Split('=');
                    object instance = CreateInstance(type);
                    string tName = split2[0];
                    
                }
                return null;
            }
            private Type ResolveType(string fullName)
            {
                Console.WriteLine(fullName);
                return Type.GetType(fullName) ?? refAsms.Select(asm => asm.GetType(fullName)).FirstOrDefault(t => t != null);
            }
        }
        static string FormatParameters(ParameterInfo[] parameters)
        {
            if (parameters.Length == 0) return "";
            else if (parameters.Length == 1)
            {
                var param = parameters[0];
                return $"{param.ParameterType.FullName} {param.Name}";
            }
            return parameters.Aggregate("", (s, p) => $"{s}{p.ParameterType} {p.Name}, ");
        }
        static List<FieldInfo> GetAllFields(Type type)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public));
            while (type != null)
            {
                fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
                type = type.BaseType;
            }
            return fields;
        }
        static string FormatMember(MemberInfo member)
            => member is FieldInfo f ? FormatField(f) : 
            member is MethodInfo m ? FormatMethod(m) : "";
        static string FormatField(FieldInfo field)
            => $"{field.FieldType} {field.DeclaringType.FullName}.{field.Name}";
        static string FormatMethod(MethodInfo method)
            => $"{method.ReturnType} {method.DeclaringType.FullName}.{method.Name}({FormatParameters(method.GetParameters())})";
        static Type[] ParseParameters(string s, Func<string, Type> typeResolver)
        {
            var split = s.Split(',');
            if (split.Length > 0)
                return split.Select(s => typeResolver(s)).ToArray();
            return new[] { typeResolver(s) };
        }
        static object ParseNumber(Type type, string s)
            => type.GetMethod("Parse", new[] { typeof(string) })
            .Invoke(null, new object[] { s });
        static string TrimOne(string s)
            => s.Remove(0, 1).Remove(s.Length - 1, 1);
        static string TrimStart(string s, out int count)
        {
            for (int i = 0; i < s.Length; i++)
                if (s[i] != ' ')
                {
                    count = i;
                    return s.TrimStart();
                }
            count = 0;
            return s.TrimStart();
        }
        public static object CreateInstance(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var ctor = type.GetConstructor((BindingFlags)15420, null, Type.EmptyTypes, null);
            if (ctor != null) return ctor.Invoke(null);
            return FormatterServices.GetUninitializedObject(type);
        }
    }
    public class Benchmark
    {
        public double d = 3000;
        [Benchmark]
        public void Math_Pow()
        {
            Math.Pow(d, 200);
        }
        [Benchmark]
        public void PowEx()
        {
            d.Pow(200);
        }
    }
    public unsafe static class FastParser
    {
        public static int ParseInt(string s)
        {
            int result = 0;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    result = 10 * result + (*c - 48);
                    c++;
                }
            }
            if (unary)
                return -result;
            return result;
        }
        public static double ParseDouble(string s)
        {
            double result = 0;
            bool isDot = false;
            int dCount = 1;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    if (*c == '.')
                    {
                        isDot = true;
                        goto Continue;
                    }
                    if (!isDot)
                        result = 10 * result + (*c - 48);
                    else result += (*c - 48) / dPow[dCount++];
                Continue:
                    c++;
                }
            }
            if (unary)
                return -result;
            return result;
        }
        public static float ParseFloat(string s)
        {
            float result = 0;
            bool isDot = false;
            int dCount = 1;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    if (*c == '.')
                    {
                        isDot = true;
                        goto Continue;
                    }
                    if (!isDot)
                        result = 10 * result + (*c - 48);
                    else result += (*c - 48) / fPow[dCount++];
                Continue:
                    c++;
                }
            }
            if (unary)
                return -result;
            return result;
        }
        private static readonly double[] dPow = GetDoublePow();
        private static double[] GetDoublePow()
        {
            var max = 309;
            var exps = new double[max];
            for (var i = 0; i < max; i++)
                exps[i] = Math.Pow(10, i);
            return exps;
        }
        private static readonly float[] fPow = GetFloatPow();
        private static float[] GetFloatPow()
        {
            var max = 39;
            var exps = new float[max];
            for (var i = 0; i < max; i++)
                exps[i] = (float)Math.Pow(10, i);
            return exps;
        }
    }
}
