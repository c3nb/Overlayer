using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Overlayer.Tags.Nodes;

namespace Overlayer.Tags
{
    public class CustomTagCompiler
    {
        public TagCollection tagsReference;
        public CustomTagCompiler(TagCollection tagsReference) => this.tagsReference = tagsReference;
        public Tag Compile(string name, string description, string expression)
        {
            IEnumerable<Token> tokens = Token.Tokenize(expression);
            DynamicMethod valueGetter = new DynamicMethod($"CustomTag_{name}_ValueGetter{TagCompiler.random.Next()}", typeof(float), new[] { typeof(Tag[]) });
            ILGenerator il = valueGetter.GetILGenerator();
            return null;
        }
        private static void EmitTokens(ILGenerator il, IEnumerable<Token> tokens, out string diagnostic)
        {
            diagnostic = null;
            
        }
    }
}
