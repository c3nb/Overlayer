using Microsoft.Scripting.Hosting;
using Overlayer.Core;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;

namespace Overlayer.Scripting.Python
{
    public class PythonScript : Script
    {
        public ScriptEngine Engine;
        private ScriptSource source;
        private ScriptScope scope;
        private CompiledCode code;
        static Dictionary<string, Delegate> apis = new Dictionary<string, Delegate>();
        public PythonScript(string path) : base(path)
        {
            Engine = PythonUtils.CreateEngine(path, out source);
            scope = Engine.CreateScope();
            foreach (var tag in TagManager.All)
                scope.SetVariable(tag.Name, tag.HasOption ? tag.FastInvokerOpt : tag.FastInvoker);
            if (apis == null)
            {
                apis = new Dictionary<string, Delegate>();
                foreach (var api in Api.GetApi(ScriptType))
                    apis.Add(api.Name, api.CreateDelegate(Expression.GetDelegateType(api.GetParameters().Select(p => p.ParameterType).ToArray())));
            }
            foreach (var api in apis)
                scope.SetVariable(api.Key, api.Value);
        }
        public override ScriptType ScriptType => ScriptType.Python;
        public override void Compile() => code = source.Compile();
        public override void Dispose()
        {
            Engine = null;
            source = null;
            scope = null;
            code = null;
        }
        public override object Evaluate() => code.Execute(scope);
        public override void Execute() => code.Execute(scope);
    }
}
