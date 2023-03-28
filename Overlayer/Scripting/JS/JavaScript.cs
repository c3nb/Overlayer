using JSEngine;
using Overlayer.Core;

namespace Overlayer.Scripting.JS
{
    public class JavaScript : Script
    {
        public ScriptEngine Engine;
        private CompiledEval eval;
        private CompiledScript exec;
        public JavaScript(string path) : base(path)
        {
            Engine = new ScriptEngine();
            Engine.EnableExposedClrTypes = true;
            foreach (var tag in TagManager.All)
                Engine.SetGlobalFunction(tag.Name, tag.GetterDelegate);
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
