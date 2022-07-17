using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Overlayer.Tags;
using Overlayer.Tags.Nodes;
using Overlayer.Utils;
using System.Diagnostics;

namespace Overlayer.Test
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var tag = new Tag("nice", "fiewjfiwjijifj", f => f * 234324);
            var tag2 = new Tag("niceee", "fiewfsdfsdfdsjfiwjijifj", f => f * 98734);
            var tags = new Tag[] { tag, tag2 };
            Overlayer.Main.AllTags = new TagCollection(tags);
            var parser = new Parser("(3 + {nice:1}) * fuck", Overlayer.Main.AllTags, new Dictionary<string, float>()
            {
                { "fuck", 34f }
            });
            var node6 = parser.ParseExpression();
            for (int i = 0; i < parser.Errors.Length; i++)
                Console.WriteLine(parser.Errors[i]);
            DynamicMethod dm = new DynamicMethod("testMethod", typeof(float), new[] { typeof(Tag[]) });
            ILGenerator il = dm.GetILGenerator();
            node6.Emit(il);
            il.Emit(OpCodes.Ret);
            Console.WriteLine(dm.Invoke(null, new[] { parser.tags }));
        }
    }
}
