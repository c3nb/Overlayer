using JSEngine;
using JSEngine.Library;
using OggVorbisEncoder.Setup;
using Overlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Overlayer.Scripting.JS
{
    public class JavaScript : Script
    {
        public ScriptEngine Engine;
        private CompiledEval eval;
        private CompiledScript exec;
        static Dictionary<string, (Delegate, JSFunctionFlags)> apis;
        public JavaScript(string path) : base(path)
        {
            Engine = new ScriptEngine();
            Engine.EnableExposedClrTypes = true;
            foreach (var tag in TagManager.All)
                Engine.SetGlobalFunction(tag.Name, tag.GetterDelegate);
            if (apis == null)
            {
                apis = new Dictionary<string, (Delegate, JSFunctionFlags)>();
                foreach (var api in Api.GetApi(ScriptType))
                {
                    var attr = api.GetCustomAttribute<ApiAttribute>();
                    var del = api.CreateDelegate(Expression.GetDelegateType(api.GetParameters().Select(p => p.ParameterType).ToArray()));
                    apis.Add(api.Name, (del, (JSFunctionFlags)attr.Flags));
                }
            }
            foreach (var (name, tuple) in apis)
                Engine.SetGlobalFunction(name, tuple.Item1, tuple.Item2);
        }
        public override ScriptType ScriptType => ScriptType.JavaScript;
        public override void Compile()
        {
            exec = CompiledScript.Compile(new FileSource(Path));
            eval = CompiledEval.Compile(new FileSource(Path));
        }
        public override void Dispose()
        {
            Engine = null;
            exec = null;
            eval = null;
        }
        public override object Evaluate() => eval.EvaluateFastInternal(Engine);
        public override void Execute() => exec.ExecuteFastInternal(Engine);
    }
}
