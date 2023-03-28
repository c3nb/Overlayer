using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using System.Text;
using Py = IronPython.Hosting.Python;

namespace Overlayer.Scripting.Python
{
    public static class PythonUtils
    {
        public static readonly Dictionary<string, object> options;
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