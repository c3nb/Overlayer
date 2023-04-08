using Neo.IronLua;
using Overlayer.Core;
using System;
using System.Collections.Generic;
using LuaEngine = Neo.IronLua.Lua;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;
using JSEngine.Library;

namespace Overlayer.Scripting.Lua
{
    public class LuaScript : Script
    {
        public LuaEngine Engine;
        private LuaGlobal env;
        private Action exec;
        private Func<object> eval;
        static Dictionary<string, Delegate> apis = new Dictionary<string, Delegate>();
        public LuaScript(string path) : base(path)
        {
            Engine = new LuaEngine();
            env = Engine.CreateEnvironment();
            foreach (var tag in TagManager.All)
                env.DefineFunction(tag.Name, tag.GetterDelegate);
            if (apis == null)
            {
                apis = new Dictionary<string, Delegate>();
                foreach (var api in Api.GetApi(ScriptType))
                {
                    Delegate del;
                    if (api.ReturnType != typeof(void))
                        del = api.CreateDelegate(Expression.GetFuncType(api.GetParameters().Select(p => p.ParameterType).Append(api.ReturnType).ToArray()));
                    else del = api.CreateDelegate(Expression.GetActionType(api.GetParameters().Select(p => p.ParameterType).ToArray()));
                    apis.Add(api.Name, del);
                }
            }
            foreach (var api in apis)
                env.DefineFunction(api.Key, api.Value);
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
