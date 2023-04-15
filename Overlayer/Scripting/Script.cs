using Overlayer.Scripting.JS;
using Overlayer.Scripting.Lua;
using Overlayer.Scripting.Python;
using System;
using System.IO;
using IOPath = System.IO.Path;

namespace Overlayer.Scripting
{
    public abstract class Script : IDisposable
    {
        public Script(string path)
        {
            path ??= string.Empty;
            Path = path;
            if (File.Exists(path))
                Source = File.ReadAllText(path);
        }
        public string Path { get; }
        public string Source { get; set; }
        public abstract ScriptType ScriptType { get; }
        public abstract void Compile();
        public abstract object Evaluate();
        public abstract void Execute();
        public abstract void Dispose();
        public static Script Create(string path, ScriptType scriptType)
        {
            switch (scriptType)
            {
                case ScriptType.JavaScript:
                    return new JavaScript(path);
                case ScriptType.Python:
                    return new PythonScript(path);
                case ScriptType.Lua:
                    return new LuaScript(path);
                default:
                    return null;
            }
        }
        public static Script CreateFromSource(string source, ScriptType scriptType)
        {
            Script scr;
            switch (scriptType)
            {
                case ScriptType.JavaScript:
                    scr = new JavaScript(null);
                    break;
                case ScriptType.Python:
                    scr = new PythonScript(null);
                    break;
                case ScriptType.Lua:
                    scr = new LuaScript(null);
                    break;
                default:
                    return null;
            }
            scr.Source = source;
            return scr;
        }
        public static ScriptType GetScriptType(string path)
        {
            string ext = IOPath.GetExtension(path);
            switch (ext)
            {
                case ".js": return ScriptType.JavaScript;
                case ".lua": return ScriptType.Lua;
                case ".py": return ScriptType.Python;
                default: return ScriptType.None;
            }
        }
    }
}
