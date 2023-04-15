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
                Type t = Utility.CopyMethods("Overlayer_Internal", Api.GetApi(ScriptType).Concat(TagManager.All.Select(t => t.Getter)));
                PythonUtils.options["Overlayer_Internal"] = DynamicHelpers.GetPythonTypeFromType(t);
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
