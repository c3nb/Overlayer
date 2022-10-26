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
        public static List<KeyValuePair<string, Delegate>> funcs { get; private set; }
        private static void PrepareEngine()
        {
            Engine = new ScriptEngine();
            Engine.EnableExposedClrTypes = true;
            foreach (Tag tag in TagManager.AllTags)
                Engine.SetGlobalFunction(tag.Name,
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
            if (funcs == null)
            {
                funcs = new List<KeyValuePair<string, Delegate>>();
                foreach (var kvp in CustomTag.functions)
                    foreach (var func in kvp.Value)
                        funcs.Add(new(kvp.Key, func.CreateDelegateAuto()));
            }
            funcs.ForEach(kvp => Engine.SetGlobalFunction(kvp.Key, kvp.Value));
            foreach (var kvp in CustomTag.constants)
                Engine.SetGlobalValue(kvp.Key, kvp.Value);
        }
        public static ScriptEngine Engine;
        public static readonly CompilerOptions Option = new CompilerOptions()
        {
            ForceStrictMode = false,
            EnableILAnalysis = true,
            CompatibilityMode = CompatibilityMode.Latest
        };
        public static Func<object> Compile(this string js)
        {
            PrepareEngine();
            var scr = CompiledEval.Compile(new TextSource(js), Option);
            return () => scr.EvaluateFastInternal(Engine);
        }
        class TextSource : ScriptSource
        {
            public string str;
            public TextSource(string str) => this.str = str;
            public override string Path => "";
            public override TextReader GetReader() => new StringReader(str);
        }
    }
}
