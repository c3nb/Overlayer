using Neo.IronLua;
using Overlayer.Core;
using System;
using LuaEngine = Neo.IronLua.Lua;

namespace Overlayer.Scripting.Lua
{
    public class LuaScript : Script
    {
        public LuaEngine Engine;
        private LuaGlobal env;
        private Action exec;
        private Func<object> eval;
        public LuaScript(string path) : base(path)
        {
            Engine = new LuaEngine();
            env = Engine.CreateEnvironment();
            foreach (var tag in TagManager.All)
                env.DefineFunction(tag.Name, tag.GetterDelegate);
        }
        public override ScriptType ScriptType => ScriptType.Lua;
        public override void Compile()
        {
            LuaChunk chunk = Engine.CompileChunk(Path, null);
            exec = () => chunk.Run(env);
            eval = () => chunk.Run(env).Values[0];
        }
        public override void Dispose()
        {
            Engine.Dispose();
            Engine = null;
            exec = null;
            eval = null;
        }
        public override object Evaluate() => eval();
        public override void Execute() => exec();
    }
}
