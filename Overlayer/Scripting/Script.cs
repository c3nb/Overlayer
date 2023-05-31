using Overlayer.Scripting.JS;
using Overlayer.Scripting.CJS;
using Overlayer.Scripting.Python;
using IOPath = System.IO.Path;
using System.Collections.Generic;

namespace Overlayer.Scripting
{
    public static class Script
    {
        static Dictionary<string, Result> cache = new Dictionary<string, Result>();
        public static ScriptType GetScriptType(string path)
        {
            string ext = IOPath.GetExtension(path);
            switch (ext)
            {
                case ".js": return ScriptType.JavaScript;
                case ".py": return ScriptType.Python;
                default: return ScriptType.None;
            }
        }
        public static Result Compile(string path, ScriptType scriptType)
        {
            switch (scriptType)
            {
                case ScriptType.JavaScript:
                    return JSUtils.Compile(path);
                case ScriptType.CompilableJS:
                    return CJSUtils.Compile(path);
                case ScriptType.Python:
                    return PythonUtils.Compile(path);
                default: return null;
            }
        }
        public static Result CompileSource(string source, ScriptType scriptType)
        {
            switch (scriptType)
            {
                case ScriptType.JavaScript:
                    return JSUtils.CompileSource(source);
                case ScriptType.CompilableJS:
                    return CJSUtils.CompileSource(source);
                case ScriptType.Python:
                    return PythonUtils.CompileSource(source);
                default: return null;
            }
        }
        public static void ClearCache() => cache.Clear();
        public static void Execute(string path, ScriptType scriptType)
        {
            if (cache.TryGetValue(path, out var result))
                result.Exec();
            else (cache[path] = Compile(path, scriptType)).Exec();
        }
        public static void ExecuteSource(string source, ScriptType scriptType)
        {
            if (cache.TryGetValue(source, out var result))
                result.Exec();
            else (cache[source] = CompileSource(source, scriptType)).Exec();
        }
        public static object Evaluate(string path, ScriptType scriptType)
        {
            if (cache.TryGetValue(path, out var result))
                return result.Eval();
            else return (cache[path] = Compile(path, scriptType)).Eval();
        }
        public static object EvaluateSource(string source, ScriptType scriptType)
        {
            if (cache.TryGetValue(source, out var result))
                return result.Eval();
            else return (cache[source] = CompileSource(source, scriptType)).Eval();
        }
    }
}
