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
        static ScriptEngine PrepareEngine()
        {
            var engine = new ScriptEngine();
            engine.EnableExposedClrTypes = true;
            foreach (Tag tag in TagManager.AllTags)
                engine.SetGlobalFunction(tag.Name,
                    tag.IsDynamic ?
                    tag.Dyn :
                    tag.IsOpt ?
                    tag.IsStringOpt ?
                    tag.IsString ?
                    new Func<string, string>(tag.OptValue) :
                    new Func<string, double>(tag.OptValueFloat) :
                    tag.IsString ?
                    new Func<double, string>(tag.OptValue) :
                    new Func<double, double>(tag.OptValueFloat) :
                    tag.IsString ?
                    new Func<string>(() => tag.Value) :
                    new Func<double>(() => tag.ValueFloat));
            engine.SetGlobalValue("KeyCode", new Kcde(engine));
            engine.SetGlobalValue("Input", new Ipt(engine));
            engine.SetGlobalValue("Overlayer", new Ovlr(engine));
            return engine;
        }
        public static readonly CompilerOptions Option = new CompilerOptions()
        {
            ForceStrictMode = false,
            EnableILAnalysis = true,
            CompatibilityMode = CompatibilityMode.Latest
        };
        public static Func<object> Compile(this string js)
        {
            var engine = PrepareEngine();
            var scr = CompiledEval.Compile(new TextSource(js), Option);
            return () => scr.EvaluateFastInternal(engine);
        }
        public static ScriptSource ToSource(this string s) => new TextSource(s);
        class TextSource : ScriptSource
        {
            public string str;
            public TextSource(string str) => this.str = str;
            public override string Path => null;
            public override TextReader GetReader() => new StringReader(str);
        }
    }
}
