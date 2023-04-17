using Microsoft.Scripting.Hosting;
using Overlayer.Core;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;
using IronPython.Runtime.Types;

namespace Overlayer.Scripting.Python
{
    public class PythonScript : Script
    {
        public ScriptEngine Engine;
        private ScriptSource source;
        private ScriptScope scope;
        private CompiledCode code;
        static bool internalTypeGenerated = false;
        public PythonScript(string path) : base(path)
        {
            if (!internalTypeGenerated)
            {
                var delegates = Api.GetApi(ScriptType).Select(m => (m.Name, m.CreateDelegate(m.ReturnType != typeof(void) ? Expression.GetFuncType(m.GetParameters().Select(p => p.ParameterType).Append(m.ReturnType).ToArray()) : Expression.GetActionType(m.GetParameters().Select(p => p.ParameterType).ToArray())))).Concat(TagManager.All.Select(t => (t.Name, t.GetterDelegate)));
                foreach (var (name, del) in delegates)
                    PythonUtils.options.Add(name, del);
                internalTypeGenerated = true;
            }
        }
        public override ScriptType ScriptType => ScriptType.Python;
        public override void Compile()
        {
            if (Engine == null)
            {
                if (Path != null)
                    Engine = PythonUtils.CreateEngine(Path, out source);
                else Engine = PythonUtils.CreateEngineFromSource(Source, out source);
                scope = Engine.CreateScope();
            }
            code = source.Compile();
        }
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
