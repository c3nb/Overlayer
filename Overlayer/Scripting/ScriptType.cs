using System;

namespace Overlayer.Scripting
{
    [Flags]
    public enum ScriptType
    {
        None = 0,
        JavaScript = 1 << 0,
        Python = 1 << 1,
        CompilableJS = 1 << 2,
        All = JavaScript | Python | CompilableJS,
    }
}
