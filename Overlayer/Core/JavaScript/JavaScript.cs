using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Overlayer.Core.Utils;
using System.IO;
using Overlayer.Core.JavaScript.Compiler;
using Overlayer.Core.JavaScript.CustomLibrary;

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
            engine.SetGlobalValue("Sprite", new Sprite(engine));
            engine.SetGlobalValue("Vector3", new Vector3Constructor(engine));
            engine.SetGlobalValue("Vector2", new Vector2Constructor(engine));
            engine.SetGlobalValue("GameObject", new GameObjectConstructor(engine));
            engine.SetGlobalValue("Color", new ColorConstructor(engine));
            engine.SetGlobalValue("HSV", new HSVConstructor(engine));
            engine.SetGlobalValue("Planet", new PlanetConstructor(engine));
            engine.SetGlobalValue("PlanetType", new PT(engine));
            engine.SetGlobalValue("Time", new Time(engine));
            return engine;
        }
        public static readonly CompilerOptions Option = new CompilerOptions()
        {
            ForceStrictMode = false,
            EnableILAnalysis = false,
            CompatibilityMode = CompatibilityMode.Latest
        };
        public static Func<object> CompileEval(this string js)
        {
            var engine = PrepareEngine();
            var scr = CompiledEval.Compile(new TextSource(js), Option);
            return () => scr.EvaluateFastInternal(engine);
        }
        public static Action CompileExec(this string js)
        {
            var engine = PrepareEngine();
            var scr = CompiledScript.Compile(new TextSource(js), Option);
            return () => scr.ExecuteFastInternal(engine);
        }
        public static ScriptSource ToSource(this string s) => new TextSource(s);
        class TextSource : ScriptSource
        {
            public string str;
            public TextSource(string str) => this.str = str;
            public override string Path => null;
            public override TextReader GetReader()
            {
                using (StringReader sr = new StringReader(str))
                {
                    StringBuilder sb = new StringBuilder();
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("import"))
                            continue;
                        sb.AppendLine(line);
                    }
                    return new StringReader(sb.ToString());
                }
            }
        }
    }
}
