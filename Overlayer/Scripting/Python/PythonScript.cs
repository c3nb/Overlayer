using Microsoft.Scripting.Hosting;
using Overlayer.Core;

namespace Overlayer.Scripting.Python
{
    public class PythonScript : Script
    {
        public ScriptEngine Engine;
        private ScriptSource source;
        private ScriptScope scope;
        private CompiledCode code;
        public PythonScript(string path) : base(path)
        {
            Engine = PythonUtils.CreateEngine(path, out source);
            scope = Engine.CreateScope();
            foreach (var tag in TagManager.All)
                scope.SetVariable(tag.Name, tag.HasOption ? tag.FastInvokerOpt : tag.FastInvoker);
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
