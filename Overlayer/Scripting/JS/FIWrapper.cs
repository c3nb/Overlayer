using System;
using System.Reflection;
using Jint;
using Jint.Native.Function;
using Jint.Native;
using System.Runtime.ExceptionServices;
using Overlayer.Core;
using System.Runtime;

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
        public object Call(params object[] args) => fi.Call(null, Array.ConvertAll(args, o => JsValue.FromObject(engine, o))).ToObject();
        public static readonly MethodInfo CallMethod = typeof(FIWrapper).GetMethod("Call");
    }
}
