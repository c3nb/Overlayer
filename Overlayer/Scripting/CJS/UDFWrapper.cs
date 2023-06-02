using JSEngine.Compiler;
using JSEngine.Library;
using System.Linq;
using System.Reflection;

namespace Overlayer.Scripting.CJS
{
    public class UDFWrapper
    {
        public UDFWrapper(UserDefinedFunction udf)
        {
            this.udf = udf;
            args = udf.ArgumentNames.ToArray();
            fd = (FunctionMethodGenerator.FunctionDelegate)udf.GeneratedMethod.GeneratedDelegate;
        }
        public readonly string[] args;
        public readonly UserDefinedFunction udf;
        readonly FunctionMethodGenerator.FunctionDelegate fd;
        public object Call(params object[] arguments)
        {
            var context = ExecutionContext.CreateFunctionContext(
                engine: udf.Engine,
                parentScope: udf.ParentScope,
                thisValue: udf.Engine.Global,
                executingFunction: udf);
            return fd(context, arguments);
        }
        public static readonly MethodInfo CallMethod = typeof(UDFWrapper).GetMethod("Call");
    }
}
