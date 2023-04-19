using HarmonyLib;
using JSEngine.Library;
using System;
using System.Collections.Generic;
using Overlayer.Scripting.JS;
using JSEngine;
using Overlayer.Core.Tags;
using Overlayer.Core;
using IronPython.Runtime;
using System.Reflection;
using IronPython.Runtime.Types;
using System.Linq;

namespace Overlayer.Scripting
{
    public static class Api
    {
        static Dictionary<ScriptType, List<(ApiAttribute, MethodInfo)>> cache = new Dictionary<ScriptType, List<(ApiAttribute, MethodInfo)>>()
        {
            [ScriptType.JavaScript] = new List<(ApiAttribute, MethodInfo)>(),
            [ScriptType.Python] = new List<(ApiAttribute, MethodInfo)>(),
        };
        static Api()
        {
            ScriptType[] types = new[] { ScriptType.JavaScript, ScriptType.Python };
            foreach (MethodInfo method in typeof(Api).GetMethods())
            {
                ApiAttribute attr = method.GetCustomAttribute<ApiAttribute>();
                if (attr == null) continue;
                for (int i = 0; i < types.Length; i++)
                    if ((attr.SupportScript & types[i]) != 0)
                        cache[types[i]].Add((attr, method));
            }
        }
        public static IEnumerable<MethodInfo> GetApiMethods(ScriptType type) => cache[type].Select(t => t.Item2);
        public static List<(ApiAttribute, MethodInfo)> GetApi(ScriptType type) => cache[type];
        public static Harmony harmony = new Harmony("Overlayer.Scripting.Api");
        public static Dictionary<string, object> Variables = new Dictionary<string, object>();
        public static List<string> RegisteredCustomTags = new List<string>();
        public static void Clear()
        {
            Variables.Clear();
            foreach (var tag in RegisteredCustomTags)
                TagManager.RemoveTag(tag);
            RegisteredCustomTags.Clear();
            TextManager.Refresh();
        }
        [Api("Log Object")]
        public static void Log(object obj) => Main.Logger.Log(OverlayerDebug.Log(obj).ToString());
        [Api("Prefix Method", SupportScript = ScriptType.JavaScript)]
        public static bool Prefix(string typeColonMethodName, FunctionInstance func)
        {
            var target = AccessTools.Method(typeColonMethodName);
            var wrap = func.Wrap(target, true);
            if (wrap == null)
                return false;
            harmony.Patch(target, new HarmonyMethod(wrap));
            return true;
        }
        [Api("Postfix Method", SupportScript = ScriptType.JavaScript)]
        public static bool Postfix(string typeColonMethodName, FunctionInstance func)
        {
            var target = AccessTools.Method(typeColonMethodName);
            var wrap = func.Wrap(target, false);
            if (wrap == null)
                return false;
            harmony.Patch(target, postfix: new HarmonyMethod(wrap));
            return true;
        }
        [Api("Get Global Variable")]
        public static object GetGlobalVariable(string name)
        {
            return Variables.TryGetValue(name, out var value) ? value : Undefined.Value;
        }
        [Api("Set Global Variable")]
        public static void SetGlobalVariable(string name, object obj)
        {
            Variables[name] = obj;
        }
        // Source Path Tracing Is Not Supported!
        [Api("Register Tag", SupportScript = ScriptType.JavaScript)]
        public static void RegisterTag(string name, UserDefinedFunction func, bool notplaying)
        {
            OverlayerDebug.Begin($"Registered Tag \"{name}\" (NotPlaying:{notplaying})");
            Tag tag = new Tag(name);
            UDFWrapper wrapper = new UDFWrapper(func);
            wrapper.Tag = tag;
            if (func.ArgumentNames.Count == 1)
                tag.SetGetter((string o) => wrapper.CallGlobal(o).ToString());
            else tag.SetGetter(new Func<string>(() => wrapper.CallGlobal().ToString()));
            tag.Build();
            TagManager.SetTag(tag, notplaying);
            OverlayerDebug.Disable();
            TextManager.Refresh();
            OverlayerDebug.Enable();
            RegisteredCustomTags.Add(name);
            Main.Logger?.Log(OverlayerDebug.End());
        }
        // Source Path Tracing Is Not Supported!
        [Api("Register Tag", SupportScript = ScriptType.Python)]
        public static void RegisterTagOpt(string name, Func<string, object> func, bool notplaying)
        {
            OverlayerDebug.Begin($"Registered Tag \"{name}\" (NotPlaying:{notplaying})");
            Tag tag = new Tag(name);
            tag.SetGetter(s => func(s).ToString());
            tag.Build();
            TagManager.SetTag(tag, notplaying);
            OverlayerDebug.Disable();
            TextManager.Refresh();
            OverlayerDebug.Enable();
            RegisteredCustomTags.Add(name);
            Main.Logger?.Log(OverlayerDebug.End());
        }
        // Source Path Tracing Is Not Supported!
        [Api("Register Tag", SupportScript = ScriptType.Python)]
        public static void RegisterTag(string name, Func<object> func, bool notplaying)
        {
            OverlayerDebug.Begin($"Registered Tag \"{name}\" (NotPlaying:{notplaying})");
            Tag tag = new Tag(name);
            tag.SetGetter(new Func<string>(() => func().ToString()));
            tag.Build();
            TagManager.SetTag(tag, notplaying);
            OverlayerDebug.Disable();
            TextManager.Refresh();
            OverlayerDebug.Enable();
            RegisteredCustomTags.Add(name);
            Main.Logger?.Log(OverlayerDebug.End());
        }
        [Api("Unregister Tag")]
        public static void UnregisterTag(string name)
        {
            TagManager.RemoveTag(name);
            TextManager.Refresh();
        }
        [Api("Resolve CLR Type", SupportScript = ScriptType.JavaScript, Flags = (int)JSFunctionFlags.HasEngineParameter)]
        public static ObjectInstance Resolve(ScriptEngine engine, string clrType)
        {
            return ClrStaticTypeWrapper.FromCache(engine, Utility.TypeByName(clrType));
        }
        [Api("Resolve CLR Type", SupportScript = ScriptType.Python)]
        public static PythonType Resolve(string clrType)
        {
            return DynamicHelpers.GetPythonTypeFromType(Utility.TypeByName(clrType));
        }
        static readonly FieldInfo py_func_codeCtx = typeof(PythonFunction).GetField("_context", AccessTools.all);
    }
}
