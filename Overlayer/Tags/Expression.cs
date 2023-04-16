using System.Collections.Generic;
using Overlayer.Core;
using Overlayer.Core.Tags;
using Overlayer.Scripting;

namespace Overlayer.Tags
{
    [ClassTag("Expression")]
    public static class Expression
    {
        public static readonly Dictionary<string, Script> expressions = new Dictionary<string, Script>();
        [Tag]
        public static object Expr(string expr)
        {
            if (expressions.TryGetValue(expr, out Script script))
                return script.Evaluate();
            var scr = Script.CreateFromSource(expr, ScriptType.JavaScript);
            Utility.ExecuteSafe(scr.Compile, out var ex);
            if (ex != null) return "";
            return (expressions[expr] = scr).Evaluate();
        }
    }
}
