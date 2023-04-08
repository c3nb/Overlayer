using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlayer.Scripting.Lua
{
    public class LuaImpl : Impl
    {
        public override ScriptType ScriptType => ScriptType.Lua;
        public override string Generate()
        {
            StringBuilder sb = new StringBuilder();
            // TODO
            return sb.ToString();
        }
    }
}
