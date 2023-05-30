using System.Collections.Generic;
using Overlayer.Core.Tags;
using Overlayer.Scripting;
using Overlayer.Scripting.JS;
using Overlayer.Core.Utils;

namespace Overlayer.Tags
{
    [ClassTag("Expression", Category = Category.Misc)]
    public static class Expression
    {
        public static readonly Dictionary<string, Result> expressions = new Dictionary<string, Result>();
        [Tag]
        public static object Expr(string expr)
        {
            if (expressions.TryGetValue(expr, out var res))
                return res.Eval();
            res = MiscUtils.ExecuteSafe(() => JSUtils.CompileSource(expr), out _);
            if (res == null) return null;
            return (expressions[expr] = res).Eval();
        }
    }
}
