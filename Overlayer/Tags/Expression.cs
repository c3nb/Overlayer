using System.Collections.Generic;
using Overlayer.Core;
using System;
using Overlayer.Core.Tags;
using Overlayer.Scripting;
using Overlayer.Scripting.JS;

namespace Overlayer.Tags
{
    [ClassTag("Expression")]
    public static class Expression
    {
        public static readonly Dictionary<string, Result> expressions = new Dictionary<string, Result>();
        [Tag]
        public static object Expr(string expr)
        {
            if (expressions.TryGetValue(expr, out var res))
                return res.Eval();
            res = Utility.ExecuteSafe(() => JSUtils.CompileSource(expr), out _);
            if (res == null) return null;
            return (expressions[expr] = res).Eval();
        }
    }
}
