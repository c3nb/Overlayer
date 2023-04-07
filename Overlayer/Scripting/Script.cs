using Overlayer.Scripting.JS;
using Overlayer.Scripting.Lua;
using Overlayer.Scripting.Python;
using System;

namespace Overlayer.Scripting
{
    public abstract class Script : IDisposable
    {
        public Script(string path) => Path = path;
        public string Path { get; }
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
    }
}
