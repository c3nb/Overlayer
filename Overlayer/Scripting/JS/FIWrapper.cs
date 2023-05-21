using System;
using System.Reflection;
using Jint;
using Jint.Native.Function;
using Jint.Native;
using System.Linq;

namespace Overlayer.Scripting.JS
{
    public class FIWrapper
    {
        public readonly Engine engine;
        public readonly FunctionInstance fi;
        public FIWrapper(FunctionInstance fi)
        {
            this.fi = fi;
            engine = fi.Engine;
        }
        public object Call(params object[] args) => fi.Call(null, args != null ? Array.ConvertAll(args, o => JsValue.FromObject(engine, o)) : new JsValue[0]).ToObject();
        public JsValue CallRaw(params object[] args) => fi.Call(null, args != null ? Array.ConvertAll(args, o => JsValue.FromObject(engine, o)) : new JsValue[0]);
        public object CallThis(params object[] args) => fi.Call(JsValue.FromObject(engine, args[0]), (args != null ? Array.ConvertAll(args, o => JsValue.FromObject(engine, o)) : new JsValue[0]).Skip(1).ToArray()).ToObject();
        public JsValue CallThisRaw(params object[] args) => fi.Call(JsValue.FromObject(engine, args[0]), (args != null ? Array.ConvertAll(args, o => JsValue.FromObject(engine, o)) : new JsValue[0]).Skip(1).ToArray());
        public static readonly MethodInfo CallMethod = typeof(FIWrapper).GetMethod("Call");
        public static readonly MethodInfo CallThisMethod = typeof(FIWrapper).GetMethod("Call");
    }
}
