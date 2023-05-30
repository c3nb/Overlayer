using System;
using JSEngine.Compiler;
using Overlayer.Scripting.CJS;

namespace JSEngine
{
    /// <summary>
    /// Powered By <see href="https://github.com/paulbartrum/jurassic">Jurassic</see>
    /// </summary>
    public static class JS
    {
        public static readonly CompilerOptions Option = new CompilerOptions()
        {
            ForceStrictMode = false,
            EnableILAnalysis = false,
            CompatibilityMode = CompatibilityMode.Latest
        };
    }
}
