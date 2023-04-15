using HarmonyExLib;
using JSEngine.Library;
using System;
using System.Collections.Generic;
using Overlayer.Scripting.JS;
using JSEngine;
using Overlayer.Core.Tags;
using Overlayer.Core;
using IronPython.Runtime;
using System.Reflection;
using Neo.IronLua;

namespace Overlayer.Scripting
{
    public static class Api
    {
        public static IEnumerable<MethodInfo> GetApi(ScriptType type)
        {
            foreach (MethodInfo method in typeof(Api).GetMethods())
            {
                ApiAttribute attr = method.GetCustomAttribute<ApiAttribute>();
                if (attr == null) continue;
                if ((attr.SupportScript & type) != 0)
                    yield return method;
            }
        }
        public static HarmonyEx harmony = new HarmonyEx("Overlayer.Scripting.Api");
        public static Dictionary<string, object> Variables = new Dictionary<string, object>();
        [Api("Log Object")]
        public static void Log(object obj) => Main.Logger.Log(OverlayerDebug.Log(obj).ToString());
        [Api("Prefix Method", SupportScript = ScriptType.JavaScript)]
        public static bool Prefix(string typeColonMethodName, FunctionInstance func)
        {
            var target = AccessTools.Method(typeColonMethodName);
            var wrap = func.Wrap(target, true);
            if (wrap == null)
                return false;
            harmony.Patch(target, new HarmonyExMethod(wrap));
            return true;
        }
        [Api("Postfix Method", SupportScript = ScriptType.JavaScript)]
        public static bool Postfix(string typeColonMethodName, FunctionInstance func)
        {
            var target = AccessTools.Method(typeColonMethodName);
            var wrap = func.Wrap(target, false);
            if (wrap == null)
                return false;
            harmony.Patch(target, postfix: new HarmonyExMethod(wrap));
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
            Tag tag = new Tag(name);
            UDFWrapper wrapper = new UDFWrapper(func);
            if (func.ArgumentNames.Count == 1)
                tag.SetGetter((string o) => wrapper.CallGlobal(o).ToString());
            else tag.SetGetter(new Func<string>(() => wrapper.CallGlobal().ToString()));
            tag.Build();
            TagManager.SetTag(tag, notplaying);
            TextManager.Refresh();
        }
        // Source Path Tracing Is Not Supported!
        [Api("Register Tag", SupportScript = ScriptType.Python)]
        public static void RegisterTag(string name, PythonFunction func, bool notplaying)
        {
            Tag tag = new Tag(name);
            CodeContext ctx = (CodeContext)py_func_codeCtx.GetValue(func);
            if (func.__code__.co_argcount == 1)
                tag.SetGetter((string o) => func.__call__(ctx, o).ToString());
            else tag.SetGetter(new Action(() => func.__call__(ctx).ToString()));
            tag.Build();
            TagManager.SetTag(tag, notplaying);
            TextManager.Refresh();
        }
        // Source Path Tracing Is Not Supported!
        [Api("Register Tag", SupportScript = ScriptType.Lua)]
        public static void RegisterTag(string name, LuaMethod func, bool notplaying)
        {
            Tag tag = new Tag(name);
            tag.SetGetter(func.Delegate);
            tag.Build();
            TagManager.SetTag(tag, notplaying);
            TextManager.Refresh();
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
            return ClrStaticTypeWrapper.FromCache(engine, AccessTools.TypeByName(clrType));
        }
        static readonly FieldInfo py_func_codeCtx = typeof(PythonFunction).GetField("_context", AccessTools.all);
    }
}
