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
using Overlayer.Core.Utils;
using Jint.Native;
using Jint.Runtime.Interop;
using Jint.Runtime.References;

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
                    .PropertySafeAccess(true)
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
        public static Result Compile(string path)
        {
            string source;
            if (File.Exists(path))
                source = File.ReadAllText(path);
            else if (File.Exists(path = Path.Combine(Main.ScriptPath, path)))
                source = File.ReadAllText(path);
            else return null;
            return new Result(Prepare(), source);
        }
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
            var prmStrs = holder.args;
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
        public static MethodInfo WrapTranspiler(this FunctionInstance func)
        {
            if (func == null) return null;
            FIWrapper holder = new FIWrapper(func);

            TypeBuilder type = EmitUtils.Mod.DefineType(PatchUtils.TypeCount++.ToString(), TypeAttributes.Public);
            MethodBuilder methodB = type.DefineMethod("Wrapper_Transpiler", MethodAttributes.Public | MethodAttributes.Static, typeof(IEnumerable<CodeInstruction>), 
                new[] { typeof(IEnumerable<CodeInstruction>), typeof(MethodBase), typeof(ILGenerator) });
            FieldBuilder holderfld = type.DefineField("holder", typeof(FIWrapper), FieldAttributes.Public | FieldAttributes.Static);

            var il = methodB.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldsfld, holderfld);
            il.Emit(OpCodes.Call, transpilerAdapter);
            il.Emit(OpCodes.Ret);

            Type t = type.CreateType();
            t.GetField("holder").SetValue(null, holder);
            return t.GetMethod("Wrapper_Transpiler");
        }
        public static IEnumerable<CodeInstruction> TranspilerAdapter(IEnumerable<CodeInstruction> instructions, MethodBase original, ILGenerator il, FIWrapper func)
        {
            object[] args = new object[func.args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var argName = func.args[i];
                if (argName.StartsWith("il"))
                    args[i] = il;
                else if (argName.StartsWith("o") ||
                    argName.StartsWith("m"))
                    args[i] = original;
                else if (argName.StartsWith("ins"))
                    args[i] = instructions.ToArray();
                else args[i] = JsValue.Undefined;
            }
            var result = func.CallRaw(args);
            if (IsNull(result)) return Enumerable.Empty<CodeInstruction>();
            else return result.AsArray().Select(v => (CodeInstruction)v.ToObject());
        }
        public static bool IsNull(object obj)
            => obj == null || (obj is JsValue jv && (jv == JsValue.Undefined || jv == JsValue.Null));
        public static bool IsTrue(object obj)
            => IsNull(obj) || obj.Equals(true);
        static readonly MethodInfo isnull = typeof(JSUtils).GetMethod("IsNull", AccessTools.all);
        static readonly MethodInfo istrue = typeof(JSUtils).GetMethod("IsTrue", AccessTools.all);
        static readonly MethodInfo transpilerAdapter = typeof(JSUtils).GetMethod("TranspilerAdapter", AccessTools.all);
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
                    sb.AppendLine("    " + JavaScriptImpl.GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"    this.{field.Name} = null;");
                }
                foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    if (field.IsStatic) continue;
                    sb.AppendLine("    " + JavaScriptImpl.GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"    this.#{field.Name} = null;");
                }
                sb.AppendLine("  }");


                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    sb.AppendLine("  " + JavaScriptImpl.GetTypeHintComment(field.FieldType));
                    sb.AppendLine($"  static {field.Name};");
                }
                foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (field.Name.StartsWith("<"))
                        continue;
                    sb.AppendLine("  " + JavaScriptImpl.GetTypeHintComment(field.FieldType));
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
                        sb.AppendLine("  " + JavaScriptImpl.GetTypeHintComment(prop.PropertyType));
                        if (getter.IsStatic)
                            sb.AppendLine($"  static get {name}() {{}}");
                        else sb.AppendLine($"  get {name}() {{}}");
                    }
                    if (setter != null)
                    {
                        sb.AppendLine("  " + JavaScriptImpl.GetPTypeHintComment(prop.PropertyType, "value"));
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
                        sb.AppendLine("  " + JavaScriptImpl.GetTypeHintComment(prop.PropertyType));
                        if (getter.IsStatic)
                            sb.AppendLine($"  static get #{name}() {{}}");
                        else sb.AppendLine($"  get #{name}() {{}}");
                    }
                    if (setter != null)
                    {
                        sb.AppendLine("  " + JavaScriptImpl.GetPTypeHintComment(prop.PropertyType, "value"));
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
                    sb.AppendLine(JavaScriptImpl.GetPRTypeHintComment(method.ReturnType, "  ", Api.Get(method), tuples.ToArray()));
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
                    sb.AppendLine(JavaScriptImpl.GetPRTypeHintComment(method.ReturnType, "  ", Api.Get(method), tuples.ToArray()));
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
                sb.AppendLine("    " + JavaScriptImpl.GetTypeHintComment(field.FieldType));
                sb.AppendLine($"    this.{field.Name} = null;");
            }
            sb.AppendLine("  }");

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.Name.StartsWith("<"))
                    continue;
                sb.AppendLine("  " + JavaScriptImpl.GetTypeHintComment(field.FieldType));
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
                var prms = method.GetParameters().Where(p => p.ParameterType != typeof(Engine) && p.ParameterType != typeof(JSEngine.ScriptEngine));
                var tuples = prms.Select(p => (p.ParameterType, p.Name));
                sb.AppendLine(JavaScriptImpl.GetPRTypeHintComment(method.ReturnType, "  ", Api.Get(method), tuples.ToArray()));
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
        public static JsValue FromObject(object value, Engine engine) => JsValue.FromObject(engine, value);
        public static object ToObject(object value) => value is JsValue jVal ? jVal.ToObject() : value;
    }
}
