using Overlayer.Scripting.Python;
using Overlayer.Scripting.JS;
using Overlayer.Scripting.CJS;

namespace Overlayer.Scripting
{
    public abstract class Impl
    {
        public abstract ScriptType ScriptType { get; }
        public abstract string Generate();
        public static string Generate(ScriptType scriptType)
        {
            switch (scriptType)
            {
                case ScriptType.Python:
                    return new PythonImpl().Generate();
                case ScriptType.CompilableJS:
                    return new CompilableJavaScriptImpl().Generate();
                case ScriptType.JavaScript:
                    return new JavaScriptImpl().Generate();
                default: return null;
            }
        }
    }
}
