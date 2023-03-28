using HarmonyLib;
using JSEngine.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Overlayer.Scripting.JS;
using JSEngine;
using Overlayer.Core.Tags;
using Overlayer.Core;

namespace Overlayer.Scripting
{
    public static class Api
    {
        public static Harmony harmony = new Harmony("Overlayer.Scripting.Api");
        public static Dictionary<string, object> Variables = new Dictionary<string, object>();
        [Api("Log Object")]
        public static void Log(object obj) => Main.Logger.Log(obj.ToString());
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
        [Api("Register Tag", SupportScript = ScriptType.JavaScript)]
        public static void RegisterTag(string name, UserDefinedFunction func, bool notplaying)
        {
            Tag tag = new Tag(name);
            UDFWrapper wrapper = new UDFWrapper(func);
            if (func.ArgumentNames.Count == 1)
                tag.SetGetter((string o) => wrapper.CallGlobal(o).ToString());
            else tag.SetGetter(new Action(() => wrapper.CallGlobal()));
            tag.Build();
            TagManager.AddTag(tag, notplaying);
            TextManager.Refresh();
        }
        [Api("Unregister Tag")]
        public static void UnregisterTag(string name)
        {
            TagManager.RemoveTag(name);
            TextManager.Refresh();
        }
        [Api("Resolve CLR Type", SupportScript = ScriptType.JavaScript)]
        public static ObjectInstance Resolve(string clrType)
        {
            return ClrStaticTypeWrapper.FromCache(, AccessTools.TypeByName(clrType));
        }
    }
}
