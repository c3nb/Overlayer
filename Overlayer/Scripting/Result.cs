using JSEngine;
using JSEngine.Compiler;
using Microsoft.Scripting.Hosting;
using Overlayer.Core;
using System;
using System.Collections.Generic;
using ScriptEngine = JSEngine.ScriptEngine;

namespace Overlayer.Scripting
{
    public class Result : IDisposable
    {
        bool js;
        CompiledCode code;
        ScriptScope scope;
        ScriptEngine engine;
        CompiledScript exec;
        CompiledEval eval;
        public Result(CompiledCode code, ScriptScope scope)
        {
            js = false;
            this.code = code;
            this.scope = scope;
        }
        public Result(ScriptEngine engine, CompiledScript exec)
        {
            js = true;
            this.engine = engine;
            this.exec = exec;
        }
        public Result(ScriptEngine engine, CompiledEval eval)
        {
            js = true;
            this.engine = engine;
            this.eval = eval;
        }
        public object Eval()
        {
            if (js)
                return eval.EvaluateFastInternal(engine);
            return code.Execute(scope);
        }
        public void Exec()
        {
            if (js)
                exec.ExecuteFastInternal(engine);
            else code.Execute(scope);
        }
        public void Dispose() => Dispose(false);
        void Dispose(bool byFinalizer)
        {
            code = null; 
            scope = null;
            engine = null;
            exec = null;
            eval = null;
            if (!byFinalizer)
                GC.SuppressFinalize(this);
        }
        ~Result() => Dispose(true);
    }
}
