using System;
using System.Text;
using System.IO;
using JSEngine.Compiler;
using JSEngine.Library;
using System.Linq;
using System.Reflection;

namespace JSEngine
{
    /// <summary>
    /// Powered By <see href="https://github.com/paulbartrum/jurassic">Jurassic</see>
    /// </summary>
    public static class JavaScript
    {
        public static readonly CompilerOptions Option = new CompilerOptions()
        {
            ForceStrictMode = false,
            EnableILAnalysis = false,
            CompatibilityMode = CompatibilityMode.Latest
        };
        public static Func<object> CompileEval(this string js, ScriptEngine engine)
        {
            var scr = CompiledEval.Compile(new TextSource(js), Option);
            return () => scr.EvaluateFastInternal(engine);
        }
        public static Action CompileExec(this string js, ScriptEngine engine)
        {
            var scr = CompiledScript.Compile(new TextSource(js), Option);
            return () => scr.ExecuteFastInternal(engine);
        }
        public static Func<object> CompileEvalWithCommentArgs(this string js, ScriptEngine engine, out (string, string)[] args)
        {
            var src = new TextSource(js, true);
            var scr = CompiledEval.Compile(src, Option);
            args = src.args;
            return () => scr.EvaluateFastInternal(engine);
        }
        public static Action CompileExecWithCommentArgs(this string js, ScriptEngine engine, out (string, string)[] args)
        {
            var src = new TextSource(js, true);
            var scr = CompiledScript.Compile(src, Option);
            args = src.args;
            return () => scr.ExecuteFastInternal(engine);
        }
        public static ScriptSource ToSource(this string s) => new TextSource(s);
        class TextSource : ScriptSource
        {
            public string str;
            public bool hasArgs;
            public (string, string)[] args = new (string, string)[0];
            public TextSource(string str, bool hasArgs = false)
            {
                this.str = str;
                this.hasArgs = hasArgs;
            }
            public override string Path => null;
            public override TextReader GetReader()
            {
                using (StringReader sr = new StringReader(str))
                {
                    StringBuilder sb = new StringBuilder();
                    string line = null;
                    bool first = true;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (hasArgs && first)
                        {
                            if (line.StartsWith("//"))
                            {
                                line = line.Remove(0, 2);
                                var args = line.Split(' ');
                                this.args = args.Select(s =>
                                {
                                    var nv = s.Split(':');
                                    return (nv[0], nv[1]);
                                }).ToArray();
                            }
                            first = false;
                            continue;
                        }
                        if (line.StartsWith("import"))
                            continue;
                        sb.AppendLine(line);
                    }
                    return new StringReader(sb.ToString());
                }
            }
        }
    }
    public class UDFWrapper
    {
        public UDFWrapper(UserDefinedFunction udf)
        {
            this.udf = udf;
            fd = (FunctionMethodGenerator.FunctionDelegate)udf.GeneratedMethod.GeneratedDelegate;
        }
        public readonly UserDefinedFunction udf;
        readonly FunctionMethodGenerator.FunctionDelegate fd;
        public object Call(object @this, params object[] arguments)
        {
            var context = ExecutionContext.CreateFunctionContext(
                engine: udf.Engine,
                parentScope: udf.ParentScope,
                thisValue: @this,
                executingFunction: udf);
            return fd(context, arguments);
        }
        public object Call(params object[] arguments)
        {
            var context = ExecutionContext.CreateFunctionContext(
                engine: udf.Engine,
                parentScope: udf.ParentScope,
                thisValue: udf.Prototype ?? (object)Undefined.Value,
                executingFunction: udf);
            return fd(context, arguments);
        }
        public object CallGlobal(params object[] arguments)
        {
            var context = ExecutionContext.CreateFunctionContext(
                engine: udf.Engine,
                parentScope: udf.ParentScope,
                thisValue: udf.Engine.Global,
                executingFunction: udf);
            return fd(context, arguments);
        }
        public static readonly MethodInfo CallMethod = typeof(UDFWrapper).GetMethod("Call", new[] { typeof(object[]) });
        public static readonly MethodInfo CallThisMethod = typeof(UDFWrapper).GetMethod("Call", new[] { typeof(object), typeof(object[]) });
        public static readonly MethodInfo CallGlobalMethod = typeof(UDFWrapper).GetMethod("CallGlobal");
    }
}
