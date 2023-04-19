using Jint;
using Microsoft.Scripting.Hosting;
using System;
using Esprima.Ast;
using Esprima;
using Microsoft.Scripting.Utils;

namespace Overlayer.Scripting
{
    public class Result : IDisposable
    {
        bool js;
        CompiledCode code;
        ScriptScope scope;
        Engine engine;
        Esprima.Ast.Script scr;
        public Result(CompiledCode code, ScriptScope scope)
        {
            js = false;
            this.code = code;
            this.scope = scope;
        }
        public Result(Engine engine, string expr)
        {
            js = true;
            this.engine = engine;
            scr = new Esprima.Ast.Script(new JavaScriptParser().ParseScript(expr).Body, false);
        }
        public object Eval()
        {
            if (js)
                return engine.Evaluate(scr).ToObject();
            return code.Execute(scope);
        }
        public void Exec()
        {
            if (js)
                engine.Execute(scr);
            else code.Execute(scope);
        }
        public void Dispose() => Dispose(false);
        void Dispose(bool byFinalizer)
        {
            code = null; 
            scope = null;
            engine = null;
            scr = null;
            if (!byFinalizer)
                GC.SuppressFinalize(this);
        }
        ~Result() => Dispose(true);
    }
}
