using System;

namespace Overlayer.Scripting
{
    [Flags]
    public enum ScriptType
    {
        None = 0,
        JavaScript = 1,
        Python = 2,
        Lua = 4,
        All = JavaScript | Python | Lua,
    }
}
