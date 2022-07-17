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
        public delegate float ValueGetter(Tag[] tags);
        public Parser parser;
        public bool CanUsedByNotPlaying;
        public TagCollection tagsReference;
        public ValueGetter getter;
        public CustomTagCompiler(TagCollection tagsReference)
            => this.tagsReference = tagsReference;
        public CustomTagCompiler Compile(string name, string description, string text, Dictionary<string, float> consts, Dictionary<string, MethodInfo> functions, out string[] errors)
        {
            parser = new Parser(text, tagsReference, consts, functions);
            Node node = parser.ParseExpression();
            errors = parser.Errors;
            if (errors.Any())
                return this;
            DynamicMethod dm = new DynamicMethod($"CustomTag_{name}_ValueGetter{TagCompiler.random.Next()}", typeof(float), new[] { typeof(Tag[]) });
            ILGenerator il = dm.GetILGenerator();
            node.Emit(il);
            il.Emit(OpCodes.Ret);
            getter = (ValueGetter)dm.CreateDelegate(typeof(ValueGetter));
            Tag tag = new Tag(name, description, func);
            Main.AllTags[name] = tag;
            if (CanUsedByNotPlaying = parser.CanUsedByNotPlaying)
                Main.NotPlayingTags[name] = tag;
            return this;
        }
        float func(float dec = -1)
        {
            var value = getter(parser.tags);
            if (dec == -1)
                return value;
            return (float)Math.Round(getter(parser.tags), (int)dec);
        }
        public float Value => getter(parser.tags);
    }
}
