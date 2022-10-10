using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using Overlayer.Core.Tags.Nodes;
using static Overlayer.Core.TextCompiler;

namespace Overlayer.Core
{
    public class TagCompiler
    {
        public delegate float ValueGetterNum(Tag[] tags);
        public delegate string ValueGetterStr(Tag[] tags);
        public Parser parser;
        public bool CanUsedByNotPlaying;
        public bool IsStringTag;
        public TagCollection tagsReference;
        public Delegate raw;
        public Func<Tag[], object> getter;
        public TagCompiler(TagCollection tagsReference)
            => this.tagsReference = tagsReference;
        public TagCompiler Compile(string name, string description, string text, Dictionary<string, float> consts, Dictionary<string, List<MethodInfo>> functions, out string[] errors)
        {
            parser = new Parser(text, tagsReference, null, consts, functions);
            Node node = parser.ParseExpression();
            errors = parser.Errors;
            if (errors.Any())
                return this;
            IsStringTag = parser.IsString;
            DynamicMethod dm;
            if (IsStringTag)
                dm = new DynamicMethod($"CustomTag_{name}_ValueGetter{random.Next()}_String", typeof(string), new[] { typeof(Tag[]) });
            else dm = new DynamicMethod($"CustomTag_{name}_ValueGetter{random.Next()}_Number", typeof(float), new[] { typeof(Tag[]) });
            ILGenerator il = dm.GetILGenerator();
            node.Emit(il);
            il.Emit(OpCodes.Ret);
            Tag tag;
            if (IsStringTag)
            {
                raw = dm.CreateDelegate(typeof(ValueGetterStr));
                var vgs = (ValueGetterStr)raw;
                getter = tags => vgs(tags);
                tag = new Tag(name, description, null, () => vgs(parser.tags));
            }
            else
            {
                raw = dm.CreateDelegate(typeof(ValueGetterNum));
                var vgn = (ValueGetterNum)raw;
                getter = tags => vgn(tags);
                tag = new Tag(name, description, func, null);
            }
            TagManager.AllTags[name] = tag;
            if (CanUsedByNotPlaying = parser.CanUsedByNotPlaying)
                TagManager.NotPlayingTags[name] = tag;
            return this;
        }
        float func(float dec = -1)
        {
            var value = (float)getter(parser.tags);
            if (dec == -1)
                return value;
            return (float)Math.Round(value, (int)dec);
        }
        public object Value => getter(parser.tags);
    }
}
