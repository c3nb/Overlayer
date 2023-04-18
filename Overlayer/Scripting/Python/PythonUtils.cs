using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Overlayer.Core;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System;
using Py = IronPython.Hosting.Python;
using Overlayer.Core.ExceptionHandling;

namespace Overlayer.Scripting.Python
{
    public static class PythonUtils
    {
        public static readonly Dictionary<string, object> options = new Dictionary<string, object>();
        static bool apiInitialized = false;
        public static void Prepare()
        {
            if (!apiInitialized)
            {
                var delegates = Api.GetApiMethods(ScriptType.Python).Select(m => (m.Name, m.CreateDelegate(m.ReturnType != typeof(void) ? Expression.GetFuncType(m.GetParameters().Select(p => p.ParameterType).Append(m.ReturnType).ToArray()) : Expression.GetActionType(m.GetParameters().Select(p => p.ParameterType).ToArray())))).Concat(TagManager.All.Select(t => (t.Name, t.GetterDelegate)));
                foreach (var (name, del) in delegates)
                    options.Add(name, del);
                apiInitialized = true;
            }
        }
        public static Result CompileExec(string path)
        {
            Prepare();
            var engine = CreateEngine(path, out var source);
            var scope = engine.CreateScope();
            var scr = source.Compile();
            return new Result(scr, scope);
        }
        public static Result CompileEval(string path)
        {
            Prepare();
            var engine = CreateEngine(path, out var source);
            var scope = engine.CreateScope();
            var scr = source.Compile();
            return new Result(scr, scope);
        }
        public static Result CompileExecSource(string source)
        {
            Prepare();
            var engine = CreateEngineFromSource(source, out var scrSource);
            var scope = engine.CreateScope();
            var scr = scrSource.Compile();
            return new Result(scr, scope);
        }
        public static Result CompileEvalSource(string source)
        {
            Prepare();
            var engine = CreateEngineFromSource(source, out var scrSource);
            var scope = engine.CreateScope();
            var scr = scrSource.Compile();
            return new Result(scr, scope);
        }
        public static string[] modulePaths = new string[] { Main.ScriptPath };
        public static ScriptEngine CreateEngine(string path, out ScriptSource source)
        {
            var engine = Py.CreateEngine();
            source = engine.CreateScriptSourceFromFile(path, Encoding.UTF8, SourceCodeKind.AutoDetect);
            ScriptScope scope = Py.GetBuiltinModule(engine);
            scope.SetVariable("__import__", new ImportDelegate(ResolveImport));
            engine.SetSearchPaths(modulePaths);
            return engine;
        }
        public static ScriptEngine CreateEngineFromSource(string src, out ScriptSource source)
        {
            var engine = Py.CreateEngine();
            source = engine.CreateScriptSourceFromString(src, SourceCodeKind.AutoDetect);
            ScriptScope scope = Py.GetBuiltinModule(engine);
            scope.SetVariable("__import__", new ImportDelegate(ResolveImport));
            engine.SetSearchPaths(modulePaths);
            return engine;
        }
        private static object ResolveImport(CodeContext context, string moduleName, PythonDictionary globals, PythonDictionary locals, PythonTuple fromlist, int level)
        {
            var builtin = IronPython.Modules.Builtin.__import__(context, moduleName, globals, locals, fromlist, level);
            var module = builtin as PythonModule;
            foreach (var kvp in options)
                module?.__setattr__(context, kvp.Key, kvp.Value);
            return builtin;
        }
    }
    public delegate object ImportDelegate(CodeContext context, string moduleName, PythonDictionary globals, PythonDictionary locals, PythonTuple fromlist, int level);
}