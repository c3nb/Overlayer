using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Overlayer.Core.Utils;
using System.IO;
using Overlayer.Core.JavaScript.Compiler;
using Overlayer.Core.JavaScript.Library;

namespace Overlayer.Core.JavaScript
{
    /// <summary>
    /// Powered By <see href="https://github.com/paulbartrum/jurassic">Jurassic</see>
    /// </summary>
    public static class JavaScript
    {
        private static ScriptEngine PrepareEngine()
        {
            ScriptEngine engine = new ScriptEngine();
            foreach (Tag tag in TagManager.AllTags)
                engine.SetGlobalFunction(tag.Name,
                    tag.IsOpt ?
                    tag.IsStringOpt ?
                    tag.IsString ?
                    new Func<string, string>(tag.OptValue) :
                    new Func<string, float>(tag.OptValueFloat) :
                    tag.IsString ?
                    new Func<float, string>(tag.OptValue) :
                    new Func<float, float>(tag.OptValueFloat) :
                    tag.IsString ?
                    new Func<string>(() => tag.Value) :
                    new Func<float>(() => tag.ValueFloat));
            foreach (var kvp in CustomTag.functions)
                kvp.Value.ForEach(m => engine.SetGlobalFunction(kvp.Key, m.CreateDelegateAuto()));
            foreach (var kvp in CustomTag.constants)
                engine.SetGlobalValue(kvp.Key, kvp.Value);
            return engine;
        }
        public static ScriptEngine Engine;
        public static readonly CompilerOptions Option = new CompilerOptions()
        {
            ForceStrictMode = false,
            EnableILAnalysis = true,
            CompatibilityMode = CompatibilityMode.Latest
        };
        public static Delegate Compile(this string js)
        {
            Engine = PrepareEngine();
            var scr = CompiledEval.Compile(new TextSource(js), Option);
            if (scr.ReturnType == typeof(double) || scr.ReturnType == typeof(uint))
                return () => (double)scr.EvaluateFastInternal(Engine);
            else if (scr.ReturnType == typeof(ConcatenatedString))
                return () => (string)scr.EvaluateFastInternal(Engine);
            else return null;
        }
        class TextSource : ScriptSource
        {
            public string str;
            public TextSource(string str) => this.str = str;
            public override string Path => throw new NotImplementedException();
            public override TextReader GetReader() => new StringReader(str);
        }
    }
}
