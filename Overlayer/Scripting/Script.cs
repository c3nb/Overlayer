using Overlayer.Scripting.JS;
using Overlayer.Scripting.CJS;
using Overlayer.Scripting.Python;
using IOPath = System.IO.Path;

namespace Overlayer.Scripting
{
    public static class Script
    {
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
    }
}
