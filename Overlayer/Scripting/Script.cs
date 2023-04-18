using Overlayer.Scripting.JS;
using Overlayer.Scripting.Python;
using Steamworks;
using System;
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
        public static Result CompileExec(string path, ScriptType scriptType)
        {
            switch (scriptType)
            {
                case ScriptType.JavaScript:
                    return JSUtils.CompileExec(path);
                case ScriptType.Python:
                    return PythonUtils.CompileExec(path);
                default: return null;
            }
        }
        public static Result CompileEval(string path, ScriptType scriptType)
        {
            switch (scriptType)
            {
                case ScriptType.JavaScript:
                    return JSUtils.CompileEval(path);
                case ScriptType.Python:
                    return PythonUtils.CompileEval(path);
                default: return null;
            }
        }
        public static Result CompileExecSource(string source, ScriptType scriptType)
        {
            switch (scriptType)
            {
                case ScriptType.JavaScript:
                    return JSUtils.CompileExecSource(source);
                case ScriptType.Python:
                    return PythonUtils.CompileExecSource(source);
                default: return null;
            }
        }
        public static Result CompileEvalSource(string source, ScriptType scriptType)
        {
            switch (scriptType)
            {
                case ScriptType.JavaScript:
                    return JSUtils.CompileEvalSource(source);
                case ScriptType.Python:
                    return PythonUtils.CompileEvalSource(source);
                default: return null;
            }
        }
    }
}
