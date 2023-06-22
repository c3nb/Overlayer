using HarmonyLib;
using System;
using System.Collections.Generic;
using Overlayer.Scripting.JS;
using Overlayer.Core.Tags;
using Overlayer.Core;
using System.Reflection;
using IronPython.Runtime.Types;
using System.Linq;
using Jint.Native.Function;
using Jint.Native;
using Overlayer.Core.Utils;
using Jint.Runtime.Interop;
using Jint;
using IronPython.Runtime;
using System.Text;
using JE = JSEngine;
using JEL = JSEngine.Library;
using Overlayer.Scripting.CJS;
using UnityEngine;

namespace Overlayer.Scripting
{
    public static class Api
    {
        static Dictionary<ScriptType, List<(ApiAttribute, MethodInfo)>> mcache = new Dictionary<ScriptType, List<(ApiAttribute, MethodInfo)>>()
        {
            [ScriptType.JavaScript] = new List<(ApiAttribute, MethodInfo)>(),
            [ScriptType.CompilableJS] = new List<(ApiAttribute, MethodInfo)>(),
            [ScriptType.Python] = new List<(ApiAttribute, MethodInfo)>(),
        };
        static Dictionary<ScriptType, List<(ApiAttribute, Type)>> tcache = new Dictionary<ScriptType, List<(ApiAttribute, Type)>>()
        {
            [ScriptType.JavaScript] = new List<(ApiAttribute, Type)>(),
            [ScriptType.CompilableJS] = new List<(ApiAttribute, Type)>(),
            [ScriptType.Python] = new List<(ApiAttribute, Type)>(),
        };
        static Dictionary<Type, ApiAttribute> tAttrCache = new Dictionary<Type, ApiAttribute>();
        static Dictionary<MethodInfo, ApiAttribute> mAttrCache = new Dictionary<MethodInfo, ApiAttribute>();
        static Dictionary<string, FIWrapper> jsFuncs = new Dictionary<string, FIWrapper>();
        static Dictionary<string, UDFWrapper> cjsFuncs = new Dictionary<string, UDFWrapper>();
        static Dictionary<string, Func<object[], object>> pyFuncs = new Dictionary<string, Func<object[], object>>();
        static Dictionary<string, PythonType> pyTypes = new Dictionary<string, PythonType>();
        static Dictionary<Engine, Dictionary<string, TypeReference>> jsTypes = new Dictionary<Engine, Dictionary<string, TypeReference>>();
        static Dictionary<string, CJSWrapper> cjsCache = new Dictionary<string, CJSWrapper>();
        static Dictionary<string, MethodInfo> cjsGenericMCache = new Dictionary<string, MethodInfo>();
        static Dictionary<string, Result> cjsResultCache = new Dictionary<string, Result>();
        internal static Dictionary<string, object> editorVariables = new Dictionary<string, object>();
        static Color cacheColor = Color.white;
        static void InitEditorVariables()
        {
            editorVariables = new Dictionary<string, object>()
            {
                { "cos", typeof(Math).GetMethod("Cos", new[]{   typeof(double) }) },
                { "cosh", typeof(Math).GetMethod("Cosh", new[] { typeof(double) }) },
                { "sin", typeof(Math).GetMethod("Sin", new[] { typeof(double) }) },
                { "sinh", typeof(Math).GetMethod("Sinh", new[] { typeof(double) }) },
                { "tan", typeof(Math).GetMethod("Tan", new[] { typeof(double) }) },
                { "tanh", typeof(Math).GetMethod("Tanh", new[] { typeof(double) }) },
                { "log", typeof(Math).GetMethod("Log", new[] { typeof(double) }) },
                { "abs", typeof(Math).GetMethod("Abs", new[] { typeof(double) }) },
                { "acos", typeof(Math).GetMethod("Acos", new[] { typeof(double) }) },
                { "asin", typeof(Math).GetMethod("Asin", new[] { typeof(double) }) },
                { "atan", typeof(Math).GetMethod("Atan", new[] { typeof(double) }) },
                { "sqrt", typeof(Math).GetMethod("Sqrt", new[] { typeof(double) }) },
                { "max", typeof(Math).GetMethod("Max", new[] { typeof(double), typeof(double) }) },
                { "min", typeof(Math).GetMethod("Min", new[] { typeof(double), typeof(double) })  },
                { "pow", typeof(Math).GetMethod("Pow", new[] { typeof(double), typeof(double) }) },
                { "pi", Math.PI },
                { "e", Math.E },
            };
        }
        public static void Init() => RegisterApi(typeof(Api));
        public static void RegisterApi(Type type)
        {
            ScriptType[] types = new[] { ScriptType.JavaScript, ScriptType.Python, ScriptType.CompilableJS };
            foreach (MethodInfo method in type.GetMethods())
            {
                ApiAttribute attr = method.GetCustomAttribute<ApiAttribute>();
                if (attr == null) continue;
                mAttrCache[method] = attr;
                for (int i = 0; i < types.Length; i++)
                    if ((attr.SupportScript & types[i]) != 0)
                    {
                        mcache[types[i]].Add((attr, method));
                        if (attr.RequireTypes != null)
                        {
                            for (int j = 0; j < attr.RequireTypes.Length; j++)
                            {
                                var reqType = attr.RequireTypes[j];
                                if (tcache[types[i]].FindIndex(t => t.Item2 == reqType) < 0)
                                    tcache[types[i]].Add((tAttrCache[reqType] = new ApiAttribute(attr.GetRequireTypeAlias(j)), reqType));
                            }
                        }
                    }
            }
            foreach (Type t in type.GetNestedTypes())
            {
                ApiAttribute attr = t.GetCustomAttribute<ApiAttribute>();
                if (attr == null) continue;
                tAttrCache[t] = attr;
                for (int i = 0; i < types.Length; i++)
                    if ((attr.SupportScript & types[i]) != 0)
                    {
                        tcache[types[i]].Add((attr, t));
                        if (attr.RequireTypes != null)
                        {
                            for (int j = 0; j < attr.RequireTypes.Length; j++)
                            {
                                var reqType = attr.RequireTypes[j];
                                if (tcache[types[i]].FindIndex(t => t.Item2 == reqType) < 0)
                                    tcache[types[i]].Add((tAttrCache[reqType] = new ApiAttribute(attr.GetRequireTypeAlias(j)), reqType));
                            }
                        }
                    }
            }
        }
        public static TileInfo[] TileInfos = new TileInfo[0];
        public static IEnumerable<MethodInfo> GetApiMethods(ScriptType type) => mcache[type].Select(t => t.Item2);
        public static IEnumerable<(ApiAttribute, MethodInfo)> GetApiMethodsWithAttr(ScriptType type) => mcache[type];
        public static IEnumerable<Type> GetApiTypes(ScriptType type)  => tcache[type].Select(t => t.Item2);
        public static IEnumerable<(ApiAttribute, Type)> GetApiTypesWithAttr(ScriptType type) => tcache[type];
        public static IEnumerable<(ApiAttribute, MethodInfo)> GetExternalMethodWithAttr(Type t, ScriptType type)
        {
            foreach (MethodInfo method in t.GetMethods())
            {
                ApiAttribute attr = method.GetCustomAttribute<ApiAttribute>();
                if (attr == null)
                    yield return (attr, method);
                else if ((attr.SupportScript & type) != 0)
                    yield return (attr, method);
            }
        }
        public static ApiAttribute Get(Type t)
        {
            if (tAttrCache.TryGetValue(t, out var attr))
                return attr;
            attr = t.GetCustomAttribute<ApiAttribute>();
            return tAttrCache[t] = attr;
        }
        public static ApiAttribute Get(MethodInfo m)
        {
            if (mAttrCache.TryGetValue(m, out var attr))
                return attr;
            attr = m.GetCustomAttribute<ApiAttribute>();
            return mAttrCache[m] = attr;
        }
        public static Harmony harmony = new Harmony("Overlayer.Scripting.Api");
        public static Dictionary<string, object> Variables = new Dictionary<string, object>();
        public static List<string> RegisteredCustomTags = new List<string>();
        public static void Release()
        {
            tcache.Clear();
            mcache.Clear();
            tAttrCache.Clear();
            mAttrCache.Clear();
            Clear();
        }
        public static void Clear()
        {
            harmony.UnpatchAll(harmony.Id);
            Variables.Clear();
            foreach (var tag in RegisteredCustomTags)
                TagManager.RemoveTag(tag);
            RegisteredCustomTags.Clear();
            TextManager.Refresh();
            pyFuncs.Clear();
            jsFuncs.Clear();
            cjsFuncs.Clear();
            pyTypes.Clear();
            jsTypes.Clear();
            cjsCache.Clear();
            cjsGenericMCache.Clear();
            cjsResultCache.Clear();
            InitEditorVariables();
            ClearTileInfo();
        }
        public static TileInfo CaptureTile(double accuracy, double xAccuracy, int seqID, double timing, double timingAvg, double bpm, int hitMargin)
        {
            var info = new TileInfo(accuracy, xAccuracy, seqID, timing, timingAvg, bpm, hitMargin);
            ArrayUtils.Add(ref TileInfos, info);
            return info;
        }
        public static void ClearTileInfo()
        {
            TileInfos = new TileInfo[0];
        }
        #region Patch API
        #region Javascript API
        [Api(SupportScript = ScriptType.JavaScript)]
        public static bool Prefix(string typeColonMethodName, JsValue patch)
        {
            if (!(patch is FunctionInstance func)) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = func.Wrap(target, true);
            if (wrap == null)
                return false;
            harmony.Patch(target, new HarmonyMethod(wrap));
            return true;
        }
        [Api(SupportScript = ScriptType.JavaScript)]
        public static bool Postfix(string typeColonMethodName, JsValue patch)
        {
            if (!(patch is FunctionInstance func)) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = func.Wrap(target, false);
            if (wrap == null)
                return false;
            harmony.Patch(target, postfix: new HarmonyMethod(wrap));
            return true;
        }
        [Api(SupportScript = ScriptType.JavaScript)]
        public static bool Transpiler(string typeColonMethodName, JsValue patch)
        {
            if (!(patch is FunctionInstance func)) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = func.WrapTranspiler();
            if (wrap == null)
                return false;
            harmony.Patch(target, transpiler: new HarmonyMethod(wrap));
            return true;
        }
        [Api(SupportScript = ScriptType.JavaScript)]
        public static bool PrefixWithArgs(string typeColonMethodName, string[] argumentClrTypes, JsValue patch)
        {
            if (!(patch is FunctionInstance func)) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var argTypes = argumentClrTypes.Select(MiscUtils.TypeByName).ToArray();
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422, null, argTypes, null);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420, null, argTypes, null);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = func.Wrap(target, true);
            if (wrap == null)
                return false;
            harmony.Patch(target, new HarmonyMethod(wrap));
            return true;
        }
        [Api(SupportScript = ScriptType.JavaScript)]
        public static bool PostfixWithArgs(string typeColonMethodName, string[] argumentClrTypes, JsValue patch)
        {
            if (!(patch is FunctionInstance func)) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var argTypes = argumentClrTypes.Select(MiscUtils.TypeByName).ToArray();
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422, null, argTypes, null);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420, null, argTypes, null);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = func.Wrap(target, false);
            if (wrap == null)
                return false;
            harmony.Patch(target, postfix: new HarmonyMethod(wrap));
            return true;
        }
        [Api(SupportScript = ScriptType.JavaScript)]
        public static bool TranspilerWithArgs(string typeColonMethodName, string[] argumentClrTypes, JsValue patch)
        {
            if (!(patch is FunctionInstance func)) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var argTypes = argumentClrTypes.Select(MiscUtils.TypeByName).ToArray();
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422, null, argTypes, null);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420, null, argTypes, null);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = func.WrapTranspiler();
            if (wrap == null)
                return false;
            harmony.Patch(target, transpiler: new HarmonyMethod(wrap));
            return true;
        }
        #endregion
        #region Compilable JS API
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static bool Prefix(string typeColonMethodName, JEL.UserDefinedFunction patch)
        {
            if (patch == null) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = patch.Wrap(target, true);
            if (wrap == null)
                return false;
            harmony.Patch(target, new HarmonyMethod(wrap));
            return true;
        }
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static bool Postfix(string typeColonMethodName, JEL.UserDefinedFunction patch)
        {
            if (patch == null) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = patch.Wrap(target, false);
            if (wrap == null)
                return false;
            harmony.Patch(target, postfix: new HarmonyMethod(wrap));
            return true;
        }
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static bool Transpiler(string typeColonMethodName, JEL.UserDefinedFunction patch)
        {
            if (patch == null) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = patch.WrapTranspiler();
            if (wrap == null)
                return false;
            harmony.Patch(target, transpiler: new HarmonyMethod(wrap));
            return true;
        }
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static bool PrefixWithArgs(string typeColonMethodName, JEL.ArrayInstance argumentClrTypes, JEL.UserDefinedFunction patch)
        {
            if (patch == null) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var argTypes = argumentClrTypes.Select(o => MiscUtils.TypeByName(o.ToString())).ToArray();
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422, null, argTypes, null);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420, null, argTypes, null);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = patch.Wrap(target, true);
            if (wrap == null)
                return false;
            harmony.Patch(target, new HarmonyMethod(wrap));
            return true;
        }
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static bool PostfixWithArgs(string typeColonMethodName, JEL.ArrayInstance argumentClrTypes, JEL.UserDefinedFunction patch)
        {
            if (patch == null) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var argTypes = argumentClrTypes.Select(o => MiscUtils.TypeByName(o.ToString())).ToArray();
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422, null, argTypes, null);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420, null, argTypes, null);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = patch.Wrap(target, false);
            if (wrap == null)
                return false;
            harmony.Patch(target, postfix: new HarmonyMethod(wrap));
            return true;
        }
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static bool TranspilerWithArgs(string typeColonMethodName, JEL.ArrayInstance argumentClrTypes, JEL.UserDefinedFunction patch)
        {
            if (patch == null) return false;
            var typemethod = typeColonMethodName.Split2(':');
            var argTypes = argumentClrTypes.Select(o => MiscUtils.TypeByName(o.ToString())).ToArray();
            var target = MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15422, null, argTypes, null);
            target ??= MiscUtils.TypeByName(typemethod[0]).GetMethod(typemethod[1], (BindingFlags)15420, null, argTypes, null);
            if (target == null)
            {
                Main.Logger.Log(OverlayerDebug.Log($"{typeColonMethodName} Cannot Be Found."));
                return false;
            }
            var wrap = patch.WrapTranspiler();
            if (wrap == null)
                return false;
            harmony.Patch(target, transpiler: new HarmonyMethod(wrap));
            return true;
        }
        #endregion
        #endregion
        #region Tag API
        // Source Path Tracing Is Not Supported!
        [Api(SupportScript = ScriptType.JavaScript)]
        public static void RegisterTag(string name, JsValue func, bool notplaying)
        {
            var executor = $"Registered Tag \"{name}\" (NotPlaying:{notplaying})";
            OverlayerDebug.Begin(executor);
            Tag tag = new Tag(name);
            if (!(func is FunctionInstance fi)) return;
            FIWrapper wrapper = new FIWrapper(fi);
            if (wrapper.args.Length == 1)
                tag.SetGetter((string o) => wrapper.Call(o).ToString());
            else tag.SetGetter(new Func<string>(() => wrapper.Call().ToString()));
            tag.Build();
            TagManager.SetTag(tag, notplaying);
            OverlayerDebug.Disable();
            TextManager.Refresh();
            OverlayerDebug.Enable();
            RegisteredCustomTags.Add(name);
            OverlayerDebug.End();
            Main.Logger?.Log(executor);
        }
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static void RegisterTag(string name, JEL.UserDefinedFunction func, bool notplaying)
        {
            if (func == null) return;
            var executor = $"Registered Tag \"{name}\" (NotPlaying:{notplaying})";
            OverlayerDebug.Begin(executor);
            Tag tag = new Tag(name);
            UDFWrapper wrapper = new UDFWrapper(func);
            if (wrapper.args.Length == 1)
                tag.SetGetter((string o) => wrapper.Call(o).ToString());
            else tag.SetGetter(new Func<string>(() => wrapper.Call().ToString()));
            tag.Build();
            TagManager.SetTag(tag, notplaying);
            OverlayerDebug.Disable();
            TextManager.Refresh();
            OverlayerDebug.Enable();
            RegisteredCustomTags.Add(name);
            OverlayerDebug.End();
            Main.Logger?.Log(executor);
        }
        // Source Path Tracing Is Not Supported!
        [Api(SupportScript = ScriptType.Python)]
        public static void RegisterTagOpt(string name, Func<string, object> func, bool notplaying)
        {
            var executor = $"Registered Tag \"{name}\" (NotPlaying:{notplaying})";
            OverlayerDebug.Begin(executor);
            Tag tag = new Tag(name);
            tag.SetGetter(s => func(s).ToString());
            tag.Build();
            TagManager.SetTag(tag, notplaying);
            OverlayerDebug.Disable();
            TextManager.Refresh();
            OverlayerDebug.Enable();
            RegisteredCustomTags.Add(name);
            OverlayerDebug.End();
            Main.Logger?.Log(executor);
        }
        // Source Path Tracing Is Not Supported!
        [Api(SupportScript = ScriptType.Python)]
        public static void RegisterTag(string name, Func<object> func, bool notplaying)
        {
            var executor = $"Registered Tag \"{name}\" (NotPlaying:{notplaying})";
            OverlayerDebug.Begin(executor);
            Tag tag = new Tag(name);
            tag.SetGetter(new Func<string>(() => func().ToString()));
            tag.Build();
            TagManager.SetTag(tag, notplaying);
            OverlayerDebug.Disable();
            TextManager.Refresh();
            OverlayerDebug.Enable();
            RegisteredCustomTags.Add(name);
            OverlayerDebug.End();
            Main.Logger?.Log(executor);
        }
        [Api]
        public static void UnregisterTag(string name)
        {
            TagManager.RemoveTag(name);
            TextManager.Refresh();
        }
        #endregion
        #region Clr API
        [Api]
        public static Type ResolveType(string clrType)
        {
            return MiscUtils.TypeByName(clrType);
        }
        [Api]
        public static MethodInfo ResolveMethod(string clrType, string name)
        {
            return MiscUtils.TypeByName(clrType)?.GetMethod(name, (BindingFlags)15420);
        }
        [Api(SupportScript = ScriptType.Python)]
        public static PythonType Resolve(string clrType)
        {
            if (pyTypes.TryGetValue(clrType, out var t))
                return t;
            return pyTypes[clrType] = DynamicHelpers.GetPythonTypeFromType(MiscUtils.TypeByName(clrType));
        }
        [Api(SupportScript = ScriptType.JavaScript)]
        public static TypeReference Resolve(Engine engine, string clrType)
        {
            if (jsTypes.TryGetValue(engine, out var dict))
                if (dict.TryGetValue(clrType, out var t))
                    return t;
                else return dict[clrType] = TypeReference.CreateTypeReference(engine, MiscUtils.TypeByName(clrType));
            dict = jsTypes[engine] = new Dictionary<string, TypeReference>();
            return dict[clrType] = TypeReference.CreateTypeReference(engine, MiscUtils.TypeByName(clrType));
        }
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static JEL.ObjectInstance Resolve(JE.ScriptEngine engine, string clrType)
        {
            return JEL.ClrStaticTypeWrapper.FromCache(engine, MiscUtils.TypeByName(clrType));
        }
        #endregion
        #region Misc API
        [Api]
        public static void Log(object obj) => Main.Logger.Log(OverlayerDebug.Log(obj)?.ToString());
        [Api(SupportScript = ScriptType.JavaScript | ScriptType.Python)]
        public static string GetGenericClrTypeString(string genericType, string[] genericArgs)
        {
            string AggregateGenericArgs(Type[] types)
            {
                StringBuilder sb = new StringBuilder();
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    Type type = types[i];
                    sb.Append($"[{type?.FullName}, {type?.Assembly.GetName().Name}]");
                    if (i < length - 1)
                        sb.Append(',');
                }
                return sb.ToString();
            }
            var t = MiscUtils.TypeByName(genericType);
            var args = genericArgs.Select(MiscUtils.TypeByName);
            return $"{t?.FullName}[{AggregateGenericArgs(args.ToArray())}]";
        }
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static string GetGenericClrTypeString(string genericType, JEL.ArrayInstance genericArgs)
        {
            string AggregateGenericArgs(Type[] types)
            {
                StringBuilder sb = new StringBuilder();
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    Type type = types[i];
                    sb.Append($"[{type?.FullName}, {type?.Assembly.GetName().Name}]");
                    if (i < length - 1)
                        sb.Append(',');
                }
                return sb.ToString();
            }
            var t = MiscUtils.TypeByName(genericType);
            var args = genericArgs.Select(o => MiscUtils.TypeByName(o.ToString()));
            return $"{t?.FullName}[{AggregateGenericArgs(args.ToArray())}]";
        }
        [Api]
        public static string ToString(object obj) => obj?.ToString();
        [Api(SupportScript = ScriptType.JavaScript)]
        public static void GenerateProxy(string clrType) => JSUtils.BuildProxy(MiscUtils.TypeByName(clrType), Main.ScriptPath);
        [Api]
        public static object GetGlobalVariable(string name)
        {
            return Variables.TryGetValue(name, out var value) ? value : null;
        }
        public delegate object CallWrapper(params object[] args);
        [Api]
        public static object SetGlobalVariable(string name, object obj)
        {
            if (obj is FunctionInstance fi)
            {
                FIWrapper wrapper = new FIWrapper(fi);
                CallWrapper del = wrapper.Call;
                obj = del;
            }
            else if (obj is JEL.UserDefinedFunction udf)
            {
                UDFWrapper wrapper = new UDFWrapper(udf);
                CallWrapper del = wrapper.Call;
                obj = del;
            }
            return Variables[name] = obj;
        }
        [Api(SupportScript = ScriptType.Python)]
        public static float RoundFloat(double value, int digits = -1) => (float)value.Round(digits);
        [Api(SupportScript = ScriptType.Python)]
        public static string RoundFloatString(double value, int digits = -1) => digits < 0 ? value.ToString() : value.ToString($"F{digits}");
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static void ExecJS(string source)
        {
            if (cjsResultCache.TryGetValue(source, out var result))
                result.Exec();
            else
            {
                result = JSUtils.CompileSource(source);
                cjsResultCache[source] = result;
                result.Exec();
            }
        }
        [Api(SupportScript = ScriptType.CompilableJS)]
        public static object EvalJS(JE.ScriptEngine engine, string source)
        {
            if (cjsResultCache.TryGetValue(source, out var result))
                return CJSUtils.FromObject(result.Eval(), engine);
            else
            {
                result = JSUtils.CompileSource(source);
                cjsResultCache[source] = result;
                return CJSUtils.FromObject(result.Eval(), engine);
            }
        }
        [Api(RequireTypes = new[] { typeof(Color2), typeof(VertexGradient2), typeof(Vector2), typeof(TMPro.TextAlignmentOptions), typeof(TextConfig)  },
            RequireTypesAliases = new[] { "Color", "GradientColor", null, "TextAlign", "OverlayerText" })]
        public static TextConfig GetText(int index) => TextManager.Texts[index].config;
        [Api]
        public static Color2 RainbowColor(double speed) => cacheColor = MiscUtils.ShiftHue(cacheColor, Main.DeltaTime * (float)speed);
        [Api(SupportScript = ScriptType.JavaScript)]
        public static object GetEditorVariable(string name)
        {
            return editorVariables.TryGetValue(name, out var value) ? value : null;
        }
        [Api(SupportScript = ScriptType.JavaScript)]
        public static object SetEditorVariable(Engine engine, string name, object obj)
        {
            var variable = SetGlobalVariable(name, obj);
            if (variable is Func<JsValue, JsValue[], JsValue> func)
                variable = new ClrFunctionInstance(engine, name, func);
            return editorVariables[name] = variable;
        }
        #endregion
        #region Compilable JS API
        [Api(SupportScript = ScriptType.Python | ScriptType.JavaScript,
            Comment = new string[]
            {
                "Compile Javascript From Source",
                "!!Warning!! Javascript Compilation May Leak Memory!"
            },
            JSReturnComment = "Compiled Javascript Invoker")]
        public static CJSWrapper Compile(string source) => cjsCache.TryGetValue(source, out var wrapper) ? wrapper : (cjsCache[source] = new CJSWrapper(CJSUtils.CompileSource(source)));
        [Api(SupportScript = ScriptType.Python | ScriptType.JavaScript,
            Comment = new string[]
            {
                "Compile Javascript From File",
                "!!Warning!! Javascript Compilation May Leak Memory!"
            },
            JSReturnComment = "Compiled Javascript Invoker")]
        public static CJSWrapper CompileFile(string path) => cjsCache.TryGetValue(path, out var wrapper) ? wrapper : (cjsCache[path] = new CJSWrapper(CJSUtils.Compile(path)));
        #endregion
        #region ADOFAI API
        [Api("TileInfo")]
        public class TileInfo
        {
            public double Accuracy;
            public double XAccuracy;
            public int SeqID;
            public double Timing;
            public double TimingAvg;
            public double Bpm;
            public int HitMargin;
            public TileInfo(double accuracy, double xAccuracy, int seqID, double timing, double timingAvg, double bpm, int hitMargin)
            {
                Accuracy = accuracy;
                XAccuracy = xAccuracy;
                SeqID = seqID;
                Timing = timing;
                TimingAvg = timingAvg;
                Bpm = bpm;
                HitMargin = hitMargin;
            }
            public override string ToString() => $"{SeqID} => Acc:{Accuracy:F2}, XAcc:{XAccuracy:F2}, Timing:{Timing:F2}ms, Bpm:{Bpm:F2}, HitMargin:{(HitMargin)HitMargin}";
        }
        [Api]
        public static TileInfo[] GetTileInfos() => TileInfos;
        #endregion
        #region Class API
        [Api("Interop", SupportScript = ScriptType.JavaScript)]
        public class InteropJS
        {
            public static void ExportJSFunction(string name, JsValue func)
            {
                var fi = func as FunctionInstance;
                if (fi == null) return;
                jsFuncs[name] = new FIWrapper(fi);
            }
            public static JsValue InvokePyFunction(Engine engine, string name, object[] args)
            {
                if (pyFuncs.TryGetValue(name, out var handler))
                    return JsValue.FromObject(engine, handler(args));
                return JsValue.Null;
            }
            public static JsValue InvokeCJSFunction(Engine engine, string name, object[] args)
            {
                if (cjsFuncs.TryGetValue(name, out var wrapper))
                    return JsValue.FromObject(engine, wrapper.Call(args));
                return JsValue.Null;
            }
        }
        [Api("Interop", SupportScript = ScriptType.CompilableJS)]
        public class InteropCJS
        {
            public static void ExportCJSFunction(string name, JEL.UserDefinedFunction func)
            {
                if (func == null) return;
                cjsFuncs[name] = new UDFWrapper(func);
            }
            public static object InvokeJSFunction(string name, object[] args)
            {
                if (jsFuncs.TryGetValue(name, out var wrapper))
                    return wrapper.Call(args);
                return null;
            }
            public static object InvokePyFunction(string name, object[] args)
            {
                if (pyFuncs.TryGetValue(name, out var handler))
                    return handler(args);
                return JE.Null.Value;
            }
        }
        [Api("Interop", SupportScript = ScriptType.Python)]
        public class InteropPy
        {
            public static void ExportPyFunction(string name, PythonFunction func)
            {
                CodeContext ctx = pf_ctx.GetValue(func) as CodeContext;
                pyFuncs[name] = args => func.__call__(ctx, args ?? new object[0]);
            }
            public static object InvokeJSFunction(string name, object[] args)
            {
                if (jsFuncs.TryGetValue(name, out var wrapper))
                    return wrapper.Call(args);
                return null;
            }
            public static object InvokeCJSFunction(string name, object[] args)
            {
                if (cjsFuncs.TryGetValue(name, out var wrapper))
                    return wrapper.Call(args);
                return null;
            }
            static readonly FieldInfo pf_ctx = typeof(PythonFunction).GetField("_context", (BindingFlags)15420);
        }
        [Api("CompiledJS", SupportScript = ScriptType.Python | ScriptType.JavaScript)]
        public class CJSWrapper
        {
            private readonly JE.ScriptEngine rootEngine;
            private readonly Result compiledJS;
            public CJSWrapper(Result compiledJS)
            {
                this.compiledJS = compiledJS;
                rootEngine = compiledJS.cjsEngine;
            }
            public void Execute()
            {
                compiledJS.cjsEngine = rootEngine;
                compiledJS.Exec();
            }
            public object Evaluate()
            {
                compiledJS.cjsEngine = rootEngine;
                return CJSUtils.ToObject(compiledJS.Eval());
            }
            public void ExecuteWith(CJSWrapper withExec)
            {
                compiledJS.cjsEngine = withExec.rootEngine;
                compiledJS.Exec();
            }
            public object EvaluateWith(CJSWrapper withEval)
            {
                compiledJS.cjsEngine = withEval.rootEngine;
                return CJSUtils.ToObject(compiledJS.Eval());
            }
        }
        [Api("On", SupportScript = ScriptType.JavaScript,
            RequireTypes = new[] { typeof(KeyCode) })]
        public class OnJS
        {
            [Api(Comment = new[]
            {
                "On ADOFAI Rewind (Level Start, Scene Moved, etc..)"
            })]
            public static void Rewind(JsValue func) => Postfix("scrController:Awake_Rewind", func);
            [Api(Comment = new[]
            {
                "On Tile Hit"
            })]
            public static void Hit(JsValue func) => Postfix("scrController:Hit", func);
            #region KeyEvents
            [Api(Comment = new[]
            {
                "On Any Key Pressed"
            })]
            public static void AnyKey(FunctionInstance func)
            {
                FIWrapper wrapper = new FIWrapper(func);
                harmony.Postfix(MiscUtils.MethodByName("scrController:Update"), new Action(() =>
                {
                    if (Input.anyKey) wrapper.Call();
                }));
            }
            [Api(Comment = new[]
            {
                "On Any Key Down"
            })]
            public static void AnyKeyDown(FunctionInstance func)
            {
                FIWrapper wrapper = new FIWrapper(func);
                harmony.Postfix(MiscUtils.MethodByName("scrController:Update"), new Action(() =>
                {
                    if (Input.anyKeyDown) wrapper.Call();
                }));
            }
            [Api(Comment = new[]
            {
                "On Key Pressed"
            })]
            public static void Key(KeyCode key, FunctionInstance func)
            {
                FIWrapper wrapper = new FIWrapper(func);
                harmony.Postfix(MiscUtils.MethodByName("scrController:Update"), new Action(() =>
                {
                    if (Input.GetKey(key)) wrapper.Call();
                }));
            }
            [Api(Comment = new[]
            {
                "On Key Up"
            })]
            public static void KeyUp(KeyCode key, FunctionInstance func)
            {
                FIWrapper wrapper = new FIWrapper(func);
                harmony.Postfix(MiscUtils.MethodByName("scrController:Update"), new Action(() =>
                {
                    if (Input.GetKeyUp(key)) wrapper.Call();
                }));
            }
            [Api(Comment = new[]
            {
                "On Key Down"
            })]
            public static void KeyDown(KeyCode key, FunctionInstance func)
            {
                FIWrapper wrapper = new FIWrapper(func);
                harmony.Postfix(MiscUtils.MethodByName("scrController:Update"), new Action(() =>
                {
                    if (Input.GetKeyDown(key)) wrapper.Call();
                }));
            }
            #endregion
        }
        [Api("On", SupportScript = ScriptType.CompilableJS,
            RequireTypes = new[] { typeof(KeyCode) })]
        public class OnCJS
        {
            [Api(Comment = new[]
            {
                "On ADOFAI Rewind (Level Start, Scene Moved, etc..)"
            })]
            public static void Rewind(JEL.UserDefinedFunction func) => Postfix("scrController:Awake_Rewind", func);
            [Api(Comment = new[]
            {
                "On Tile Hit"
            })]
            public static void Hit(JEL.UserDefinedFunction func) => Postfix("scrController:Hit", func);
            #region KeyEvents
            [Api(Comment = new[]
            {
                "On Any Key Pressed"
            })]
            public static void AnyKey(JEL.UserDefinedFunction func)
            {
                UDFWrapper wrapper = new UDFWrapper(func);
                harmony.Postfix(MiscUtils.MethodByName("scrController:Update"), new Action(() =>
                {
                    if (Input.anyKey) wrapper.Call();
                }));
            }
            [Api(Comment = new[]
            {
                "On Any Key Down"
            })]
            public static void AnyKeyDown(JEL.UserDefinedFunction func)
            {
                UDFWrapper wrapper = new UDFWrapper(func);
                harmony.Postfix(MiscUtils.MethodByName("scrController:Update"), new Action(() =>
                {
                    if (Input.anyKeyDown) wrapper.Call();
                }));
            }
            [Api(Comment = new[]
            {
                "On Key Pressed"
            })]
            public static void Key(KeyCode key, JEL.UserDefinedFunction func)
            {
                UDFWrapper wrapper = new UDFWrapper(func);
                harmony.Postfix(MiscUtils.MethodByName("scrController:Update"), new Action(() =>
                {
                    if (Input.GetKey(key)) wrapper.Call();
                }));
            }
            [Api(Comment = new[]
            {
                "On Key Up"
            })]
            public static void KeyUp(KeyCode key, JEL.UserDefinedFunction func)
            {
                UDFWrapper wrapper = new UDFWrapper(func);
                harmony.Postfix(MiscUtils.MethodByName("scrController:Update"), new Action(() =>
                {
                    if (Input.GetKeyUp(key)) wrapper.Call();
                }));
            }
            [Api(Comment = new[]
            {
                "On Key Down"
            })]
            public static void KeyDown(KeyCode key, JEL.UserDefinedFunction func)
            {
                UDFWrapper wrapper = new UDFWrapper(func);
                harmony.Postfix(MiscUtils.MethodByName("scrController:Update"), new Action(() =>
                {
                    if (Input.GetKeyDown(key)) wrapper.Call();
                }));
            }
            #endregion
        }
        [Api("TextCompiler")]
        public class TextCompiler
        {
            public static TextCompiler Create() => new TextCompiler();
            private Replacer replacer;
            public TextCompiler()
            {
                replacer = new Replacer(TagManager.All);
            }
            public void Compile(string str)
            {
                replacer.Source = str;
                replacer.Compile();
            }
            public string GetValue() => replacer.Replace();
        }
        #endregion
    }
}
