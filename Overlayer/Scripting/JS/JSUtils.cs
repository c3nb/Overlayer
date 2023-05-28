using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Overlayer.Core;
using Jint;
using Jint.Native.Function;
using System.IO;
using System.Text;
using Esprima.Ast;
using Overlayer.Core.Utils;
using Jint.Native;

namespace Overlayer.Scripting.JS
{
    public static class JSUtils
    {
        static Dictionary<string, object> apis;
        public static Engine Prepare()
        {
            var engine = new Engine(op => 
                op.AllowClr(MiscUtils.loadedAsss)
                    .AllowReflection()
                    .ClrSlowInvoke(true)
                    .Strict(false)
            );
            foreach (var tag in TagManager.All)
                engine.SetValue(tag.Name, tag.Getter);
            if (apis == null)
            {
                apis = new Dictionary<string, object>();
                foreach (var api in Api.GetApiMethods(ScriptType.JavaScript))
                    apis.Add(api.Name, api);
                foreach (var (attr, t) in Api.GetApiTypesWithAttr(ScriptType.JavaScript))
                    apis.Add(attr.Name ?? t.Name, t);
            }
            foreach (var api in apis)
                engine.SetValue(api.Key, api.Value);
            return engine;
        }
        public static Options AllowReflection(this Options op)
        {
            op.Interop.AllowSystemReflection = true;
            return op;
        }
        public static Result Compile(string path) => new Result(Prepare(), File.ReadAllText(path));
        public static Result CompileSource(string source) => new Result(Prepare(), source);
        static ParameterInfo[] SelectActualParams(MethodBase m, ParameterInfo[] p, string[] n)
        {
            Type dType = m.DeclaringType;
            List<ParameterInfo> pList = new List<ParameterInfo>();
            for (int i = 0; i < n.Length; i++)
            {
                int index = Array.FindIndex(p, pa => pa.Name == n[i]);
                if (index > 0)
                    pList.Add(p[index]);
                else
                {
                    string s = n[i];
                    switch (s)
                    {
                        case "__instance":
                            pList.Add(new CustomParameter(dType, s));
                            break;
                        case "__originalMethod":
                            pList.Add(new CustomParameter(typeof(MethodBase), s));
                            break;
                        case "__args":
                            pList.Add(new CustomParameter(typeof(MethodBase), s));
                            break;
                        case "__result":
                            pList.Add(new CustomParameter(m is MethodInfo mi ? mi.ReturnType : typeof(object), s));
                            break;
                        case "__exception":
                            pList.Add(new CustomParameter(typeof(Exception), s));
                            break;
                        case "__runOriginal":
                            pList.Add(new CustomParameter(typeof(bool), s));
                            break;
                        case "il":
                            pList.Add(new CustomParameter(typeof(ILGenerator), s));
                            break;
                        case "instructions":
                            pList.Add(new CustomParameter(typeof(IEnumerable<CodeInstruction>), s));
                            break;
                        default:
                            if (s.StartsWith("__"))
                            {
                                if (int.TryParse(s.Substring(0, 2), out int num))
                                {
                                    if (num < 0 || num >= p.Length)
                                        return null;
                                    pList.Add(new CustomParameter(p[num].ParameterType, s));
                                }
                                else return null;
                            }
                            else if (s.StartsWith("___"))
                            {
                                string name = s.Substring(0, 3);
                                FieldInfo field = dType.GetField(name, AccessTools.all);
                                if (field == null)
                                    return null;
                            }
                            break;
                    }
                }
            }
            return pList.ToArray();
        }
        public static MethodInfo Wrap(this FunctionInstance func, MethodBase target, bool rtIsBool)
        {
            if (func == null) return null;
            FIWrapper holder = new FIWrapper(func);

            TypeBuilder type = EmitUtils.Mod.DefineType(PatchUtils.TypeCount++.ToString(), TypeAttributes.Public);
            var prmStrs = func.FunctionDeclaration.Params.Select(n => ((Identifier)n).Name);
            ParameterInfo[] parameters = SelectActualParams(target, target.GetParameters(), prmStrs.ToArray());
            if (parameters == null) return null;
            Type[] paramTypes = parameters.Select(p => p.ParameterType).ToArray();
            MethodBuilder methodB = type.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, rtIsBool ? typeof(bool) : typeof(void), paramTypes);
            FieldBuilder holderfld = type.DefineField("holder", typeof(FIWrapper), FieldAttributes.Public | FieldAttributes.Static);

            var il = methodB.GetILGenerator();
            LocalBuilder arr = il.MakeArray<object>(parameters.Length);
            int paramIndex = 1;
            foreach (ParameterInfo param in parameters)
            {
                Type pType = param.ParameterType;
                EmitUtils.IgnoreAccessCheck(pType);
                methodB.DefineParameter(paramIndex++, ParameterAttributes.None, param.Name);
                int pIndex = paramIndex - 2;
                il.Emit(OpCodes.Ldloc, arr);
                il.Emit(OpCodes.Ldc_I4, pIndex);
                il.Emit(OpCodes.Ldarg, pIndex);
                il.Emit(OpCodes.Stelem_Ref);
            }
            il.Emit(OpCodes.Ldsfld, holderfld);
            il.Emit(OpCodes.Ldloc, arr);
            il.Emit(OpCodes.Call, FIWrapper.CallMethod);
            if (rtIsBool)
                il.Emit(OpCodes.Call, istrue);
            else il.Emit(OpCodes.Pop);
            il.Emit(OpCodes.Ret);

            Type t = type.CreateType();
            t.GetField("holder").SetValue(null, holder);
            return t.GetMethod("Wrapper");
        }
        /// <param name="func">MUST RETURN CODE INSTRUCTION LIST</param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static MethodInfo WrapTranspiler(this FunctionInstance func, MethodBase target)
        {
            if (func == null) return null;
            FIWrapper holder = new FIWrapper(func);

            TypeBuilder type = EmitUtils.Mod.DefineType(PatchUtils.TypeCount++.ToString(), TypeAttributes.Public);
            var prmStrs = func.FunctionDeclaration.Params.Select(n => ((Identifier)n).Name);
            ParameterInfo[] parameters = SelectActualParams(target, target.GetParameters(), prmStrs.ToArray());
            if (parameters == null) return null;
            Type[] paramTypes = parameters.Select(p => p.ParameterType).ToArray();
            MethodBuilder methodB = type.DefineMethod("Wrapper", MethodAttributes.Public | MethodAttributes.Static, typeof(List<CodeInstruction>), paramTypes);
            FieldBuilder holderfld = type.DefineField("holder", typeof(FIWrapper), FieldAttributes.Public | FieldAttributes.Static);

            var il = methodB.GetILGenerator();
            LocalBuilder arr = il.MakeArray<object>(parameters.Length);
            int paramIndex = 1;
            foreach (ParameterInfo param in parameters)
            {
                Type pType = param.ParameterType;
                EmitUtils.IgnoreAccessCheck(pType);
                methodB.DefineParameter(paramIndex++, ParameterAttributes.None, param.Name);
                int pIndex = paramIndex - 2;
                il.Emit(OpCodes.Ldloc, arr);
                il.Emit(OpCodes.Ldc_I4, pIndex);
                il.Emit(OpCodes.Ldarg, pIndex);
                if (pType == typeof(IEnumerable<CodeInstruction>))
                    il.Emit(OpCodes.Call, castToArr);
                il.Emit(OpCodes.Stelem_Ref);
            }
            il.Emit(OpCodes.Ldsfld, holderfld);
            il.Emit(OpCodes.Ldloc, arr);
            il.Emit(OpCodes.Call, FIWrapper.CallMethod);
            il.Emit(OpCodes.Call, castToEnumerable);
            il.Emit(OpCodes.Ret);

            Type t = type.CreateType();
            t.GetField("holder").SetValue(null, holder);
            return t.GetMethod("Wrapper");
        }
        public static bool IsNull(object obj)
            => obj is JsValue jv && (jv == JsValue.Undefined || jv == JsValue.Null);
        public static bool IsTrue(object obj)
            => IsNull(obj) || obj == null || obj.Equals(true);
        public static CodeInstruction[] CastEnumerableToArray(IEnumerable<CodeInstruction> ci)
            => ci.ToArray();
        public static IEnumerable<CodeInstruction> CastObjectToEnumerable(object ci)
            => ci as CodeInstruction[];
        static readonly MethodInfo isnull = typeof(JSUtils).GetMethod("IsNull", AccessTools.all);
        static readonly MethodInfo istrue = typeof(JSUtils).GetMethod("IsTrue", AccessTools.all);
        static readonly MethodInfo castToArr = typeof(JSUtils).GetMethod("CastEnumerableToArray", AccessTools.all);
        static readonly MethodInfo castToEnumerable = typeof(JSUtils).GetMethod("CastObjectToEnumerable", AccessTools.all);
        class CustomParameter : ParameterInfo
        {
            public CustomParameter(Type type, string name)
            {
                ClassImpl = type;
                NameImpl = name;
            }
        }
        private static string GetPRTypeHintComment(Type returnType, string indent, params (Type, string)[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(indent + "/**");
            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                sb.AppendLine(indent + $" * @param {{{GetTypeHintCode(param.Item1)}}} {param.Item2}");
            }
            sb.AppendLine(indent + $" * @returns {{{GetTypeHintCode(returnType)}}}");
            sb.Append(indent + " */");
            return sb.ToString();
        }
        private static string GetTypeHintCode(Type type)
        {
            if (type == typeof(void))
                return "void";
            else if (type == typeof(Array))
                return "any[]";
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return "null";
                case TypeCode.Boolean:
                    return "boolean";
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return "number";
                case TypeCode.DateTime:
                    return "Date";
                case TypeCode.Char:
                case TypeCode.String:
                    return "string";
                case TypeCode.Object:
                    if (type.Namespace != null)
                        return (type.FullName?.Replace(type.Namespace + ".", "").Replace('+', '.') ?? type.Name).RemoveAfter("`");
                    else return type.Name.RemoveAfter("`");
                default:
                    return "undefined";
            }
        }
        private static string GetPTypeHintComment(Type type, string name) => $"/**@param {{{GetTypeHintCode(type)}}} {name}*/";
        private static string GetTypeHintComment(Type type) => $"/**@type {{{GetTypeHintCode(type)}}}*/";
        public static string RemoveAfter(this string str, string after)
        {
            int index = str.IndexOf(after);
            if (index < 0) return str;
            return str.Remove(index, str.Length - index);
        }
        static string TrimBetween(this string str, string start, string end)
        {
            int sIdx = str.IndexOf(start);
            int eIdx = str.LastIndexOf(end);
            if (sIdx < 0 || eIdx < 0) return str;
            return str.Remove(sIdx, eIdx - sIdx + 1);
        }
        public static string BuildProxy(Type type, string path = "", bool curDir = false, bool buildNestedTypes = false)
            => BuildProxy_(type, path, curDir, buildNestedTypes);
        private static string BuildProxy_(Type type, string path = "", bool curDir = false, bool buildNestedTypes = false, Type parent = null)
        {
            var fileName = (type.FullName ?? (parent != null ? $"{parent.Namespace}.{type.Name}" : type.Name)).Replace('.', '/').Replace('+', '/').TrimBetween("[", "]") + "_Proxy.js";
            if (curDir)
            {
                if (parent == null)
                    fileName = type.Name + "_Proxy.js";
                else fileName = $"{parent.Name}/{type.Name}_Proxy.js";
            }
            fileName = Path.Combine(path, fileName);
            Directory.GetParent(fileName).Create();
            var bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
            using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(file, Encoding.UTF8))
            {
                var needNl = false;
                var lastImportedIndex = 0;
                var sb = new StringBuilder();
                sb.AppendLine($"// {type.FullName} Proxy");
                foreach (var nestedType in type.GetNestedTypes(bindingFlags))
                {
                    if (nestedType.Name.StartsWith("<"))
                        continue;
                    needNl = true;
                    if (buildNestedTypes)
                        BuildProxy_(nestedType, path, curDir, buildNestedTypes, type);
                    sb.Append("import { ");
                    sb.Append(nestedType.Name.RemoveAfter("`"));
                    sb.Append(" } from \"");
                    sb.Append($"./{type.Name}/{nestedType.Name}_Proxy.js");
                    sb.AppendLine("\";");
                }
                lastImportedIndex = sb.Length - 1;

                if (needNl)
                    sb.AppendLine();

                sb.Append("export class ");
                sb.Append(type.Name.RemoveAfter("`"));
                sb.AppendLine(" {");
                #region NestedTypes
                foreach (var nestedType in type.GetNestedTypes(bindingFlags))
                {
                    if (nestedType.Name.StartsWith("<"))
                        continue;
                    sb.Append("  static get ");
                    sb.Append(nestedType.Name.RemoveAfter("`"));
                    sb.Append("() {");
                    sb.Append(" return ");
                    sb.Append(nestedType.Name.RemoveAfter("`"));
                    sb.Append(";");
                    sb.AppendLine(" }");
                }
                #endregion
                #region Fields
                sb.AppendLine("  constructor() {");
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    if (field.IsStatic) continue;
                    sb.AppendLine("    " + GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"    this.{field.Name} = null;");
                }
                foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    if (field.IsStatic) continue;
                    sb.AppendLine("    " + GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"    this.#{field.Name} = null;");
                }
                sb.AppendLine("  }");


                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    sb.AppendLine("  " + GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"  static {field.Name};");
                }
                foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    sb.AppendLine("  " + GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"  static #{field.Name};");
                }
                #endregion
                #region Properties
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (prop.Name.StartsWith("<"))
                        continue;
                    var name = prop.Name.Split('.').Last();
                    var getter = prop.GetGetMethod(true);
                    var setter = prop.GetSetMethod(true);
                    if (getter != null)
                    {
                        sb.AppendLine("  " + GetTypeHintComment(prop.PropertyType));
                        if (getter.IsStatic)
                            sb.AppendLine($"  static get {name}() {{}}");
                        else sb.AppendLine($"  get {name}() {{}}");
                    }
                    if (setter != null)
                    {
                        sb.AppendLine("  " + GetPTypeHintComment(prop.PropertyType, "value"));
                        if (getter.IsStatic)
                            sb.AppendLine($"  static set {name}(value) {{}}");
                        else sb.AppendLine($"  set {name}(value) {{}}");
                    }
                }
                foreach (var prop in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (prop.Name.StartsWith("<"))
                        continue;
                    var name = prop.Name.Split('.').Last();
                    var getter = prop.GetGetMethod(true);
                    var setter = prop.GetSetMethod(true);
                    if (getter != null)
                    {
                        sb.AppendLine("  " + GetTypeHintComment(prop.PropertyType));
                        if (getter.IsStatic)
                            sb.AppendLine($"  static get #{name}() {{}}");
                        else sb.AppendLine($"  get #{name}() {{}}");
                    }
                    if (setter != null)
                    {
                        sb.AppendLine("  " + GetPTypeHintComment(prop.PropertyType, "value"));
                        if (getter.IsStatic)
                            sb.AppendLine($"  static set #{name}(value) {{}}");
                        else sb.AppendLine($"  set #{name}(value) {{}}");
                    }
                }
                #endregion
                #region Methods
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (method.Name.StartsWith("<"))
                        continue;
                    if (method.IsSpecialName && !method.Name.StartsWith("add_") && !method.Name.StartsWith("remove_"))
                        continue;
                    var prms = method.GetParameters();
                    var tuples = prms.Select(p => (p.ParameterType, p.Name));
                    sb.AppendLine(GetPRTypeHintComment(method.ReturnType, "  ", tuples.ToArray()));
                    var prmString = prms.Aggregate("", (c, n) => $"{c}{n.Name}, ");
                    if (prmString.Length > 2)
                        prmString = prmString.Remove(prmString.Length - 2);
                    var name = method.Name.Split('.').Last();
                    if (method.IsStatic)
                        sb.AppendLine($"  static {name}({prmString});");
                    else sb.AppendLine($"  {name}({prmString});");
                }
                foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (method.Name.StartsWith("<"))
                        continue;
                    if (method.IsSpecialName && !method.Name.StartsWith("add_") && !method.Name.StartsWith("remove_"))
                        continue;
                    var prms = method.GetParameters();
                    var tuples = prms.Select(p => (p.ParameterType, p.Name));
                    sb.AppendLine(GetPRTypeHintComment(method.ReturnType, "  ", tuples.ToArray()));
                    var prmString = prms.Aggregate("", (c, n) => $"{c}{n.Name}, ");
                    if (prmString.Length > 2)
                        prmString = prmString.Remove(prmString.Length - 2);
                    var name = method.Name.Split('.').Last();
                    if (method.IsStatic)
                        sb.AppendLine($"  static #{name}({prmString});");
                    else sb.AppendLine($"  #{name}({prmString});");
                }
                #endregion
                sb.Append("}");
                writer.Write(sb.ToString());
                writer.BaseStream.SetLength(writer.BaseStream.Position);
            }
            return fileName;
        }
        public static void WriteType(Type type, StringBuilder sb, string alias = null)
        {
            sb.Append("class ");
            var tName = (alias ?? type.Name);
            sb.Append(tName.RemoveAfter("`"));
            sb.AppendLine(" {");
            #region Fields
            sb.AppendLine("  constructor() {");
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.Name.StartsWith("<"))
                    continue;
                if (field.IsStatic) continue;
                sb.AppendLine("    " + GetTypeHintComment(field.FieldType));
                sb.AppendLine($"    this.{field.Name} = null;");
            }
            sb.AppendLine("  }");

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.Name.StartsWith("<"))
                    continue;
                sb.AppendLine("  " + GetTypeHintComment(field.FieldType));
                sb.AppendLine($"  static {field.Name};");
            }
            #endregion
            #region Methods
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).OrderBy(x => x.Name))
            {
                if (method.IsObjectDeclared()) continue;
                if (method.Name.StartsWith("<"))
                    continue;
                if (method.IsSpecialName && !method.Name.StartsWith("add_") && !method.Name.StartsWith("remove_"))
                    continue;
                var prms = method.GetParameters().Where(p => p.ParameterType != typeof(Engine));
                var tuples = prms.Select(p => (p.ParameterType, p.Name));
                sb.AppendLine(GetPRTypeHintComment(method.ReturnType, "  ", tuples.ToArray()));
                var prmString = prms.Aggregate("", (c, n) => $"{c}{n.Name}, ");
                if (prmString.Length > 2)
                    prmString = prmString.Remove(prmString.Length - 2);
                var name = method.Name.Split('.').Last();
                if (method.IsStatic)
                    sb.AppendLine($"  static {name}({prmString}) {{ {(method.ReturnType != typeof(void) ? "return " : "")}{method.Name}({prmString}) }}");
                else sb.AppendLine($"  {name}({prmString}) {{ {(method.ReturnType != typeof(void) ? "return " : "")}{method.Name}({prmString}) }}");
            }
            #endregion
            sb.AppendLine("}");
        }
        public static bool IsObjectDeclared(this MemberInfo member) => member.DeclaringType == typeof(object);
    }
}
