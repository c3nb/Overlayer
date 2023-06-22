using Jint;
using Microsoft.Scripting.Hosting;
using System;
using Esprima;
using System.IO;
using System.Text;
using JE = JSEngine;
using Overlayer.Scripting.CJS;

namespace Overlayer.Scripting
{
    public class Result : IDisposable
    {
        ScriptType sType;
        CompiledCode code;
        ScriptScope scope;
        internal JE.ScriptEngine cjsEngine;
        TextSource cjsScr;
        JE.CompiledEval cjsEval;
        JE.CompiledScript cjsExec;
        Engine engine;
        Esprima.Ast.Script scr;
        public Result(CompiledCode code, ScriptScope scope)
        {
            sType = ScriptType.Python;
            this.code = code;
            this.scope = scope;
        }
        public Result(Engine engine, string expr)
        {
            sType = ScriptType.JavaScript;
            this.engine = engine;
            scr = new Esprima.Ast.Script(new JavaScriptParser().ParseScript(RemoveImports(expr)).Body, false);
        }
        public Result(JE.ScriptEngine engine, string expr)
        {
            sType = ScriptType.CompilableJS;
            cjsEngine = engine;
            cjsScr = new TextSource(expr, engine);
        }
        public object Eval()
        {
            switch (sType)
            {
                case ScriptType.Python:
                    return code.Execute(scope);
                case ScriptType.JavaScript:
                    return engine.Evaluate(scr).ToObject();
                case ScriptType.CompilableJS:
                    if (cjsEval == null)
                        cjsEval = JE.CompiledEval.Compile(cjsScr, JE.JS.Option);
                    return cjsEval.EvaluateFastInternal(cjsEngine);
                default: return null;
            }
        }
        public void Exec()
        {
            switch (sType)
            {
                case ScriptType.Python:
                    code.Execute(scope);
                    break;
                case ScriptType.JavaScript:
                    engine.Execute(scr);
                    break;
                case ScriptType.CompilableJS:
                    if (cjsExec == null)
                        cjsExec = JE.CompiledScript.Compile(cjsScr, JE.JS.Option);
                    cjsExec.ExecuteFastInternal(cjsEngine);
                    break;
            }
        }
        public object GetValue(string key)
        {
            switch (sType)
            {
                case ScriptType.Python:
                    return scope.GetVariable(key);
                case ScriptType.JavaScript:
                    return engine.GetValue(key).ToObject();
                case ScriptType.CompilableJS:
                    return cjsEngine.GetGlobalValue(key);
                default: return null;
            }
        }
        public void SetValue(string key, object value)
        {
            switch (sType)
            {
                case ScriptType.Python:
                    scope.SetVariable(key, value);  
                    break;
                case ScriptType.JavaScript:
                    engine.SetValue(key, value);    
                    break;
                case ScriptType.CompilableJS:
                    cjsEngine.SetGlobalValue(key, value);
                    break;
            }
        }
        public void Dispose() => Dispose(false);
        void Dispose(bool byFinalizer)
        {
            code = null; 
            scope = null;
            engine = null;
            scr = null;
            cjsEngine = null;
            cjsScr = null;
            cjsEval = null;
            cjsExec = null;
            if (!byFinalizer)
                GC.SuppressFinalize(this);
        }
        ~Result() => Dispose(true);
        static string RemoveImports(string expr)
        {
            using (StringReader sr = new StringReader(expr))
            {
                StringBuilder sb = new StringBuilder();
                string line = null;
                while ((line = sr.ReadLine()) != null)
                    if (!line.StartsWith("import"))
                        sb.AppendLine(line);
                    else sb.AppendLine();
                return sb.ToString();
            }
        }
    }
}
