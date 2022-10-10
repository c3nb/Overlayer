using Overlayer.Core.Tags.Nodes;
using Overlayer.Core.Translation;
using Overlayer.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TinyJson;

namespace Overlayer.Core.Tags
{
    public class Function
    {
        public class Arg
        {
            public int Index;
            public bool IsString;
            public Arg(int index, bool isString)
            {
                Index = index;
                IsString = isString;
            }
        }
        public static readonly string JsonPath = Path.Combine("Mods", "Overlayer", "Functions.json");
        public static void Load()
        {
            if (File.Exists(JsonPath))
                Functions = File.ReadAllText(JsonPath).FromJson<List<Function>>() ?? new List<Function>();
        }
        public static void Save() => File.WriteAllText(JsonPath, Functions.ToJson());
        public static List<Function> Functions = new List<Function>();
        internal MethodInfo method;

        public List<Arg> args;
        public string name = string.Empty;
        public string expression = string.Empty;
        public bool editing = true;
        public bool isStringTag = false;
        public int indexCache;

        internal bool canUsedByNotPlaying = false;
        internal string name_ = string.Empty;
        internal string expression_ = string.Empty;
        internal string error = string.Empty;
        internal bool isStringFunc;

        internal Tag[] tags;
        internal IntPtr tagsAddr;

        public Function()
        {
            args = new List<Arg>();
            indexCache = 1;
        }
        public Function(string name) : this()
        {
            this.name = name;
            name_ = name;
            args = new List<Arg>();
        }
        public MethodInfo Compile(string name, string expression, Action callbackAfterBuild = null)
        {
            var argList = args.Select(a => new ArgumentNode(a.Index, a.IsString ? typeof(string) : typeof(float))).ToList();
            Parser parser = new Parser(expression, TagManager.AllTags, argList, CustomTag.constants, CustomTag.functions);
            if (string.IsNullOrEmpty(name))
                error = Main.Language[TranslationKeys.NameCannotBeEmpty];
            else if (string.IsNullOrEmpty(expression))
                error = Main.Language[TranslationKeys.ExprCannotBeEmpty];
            CustomTag.functions.Remove(name ?? "");
            this.name = name;
            this.expression = expression;
            Node node = parser.ParseExpression();
            tags = parser.tags;
            tagsAddr = EmitUtils.Address<Tag[]>.Get(ref tags);
            DynamicMethod dm = new DynamicMethod($"{name}_{tagsAddr.ToInt64()}", parser.IsString ? typeof(string) : typeof(float), GetArgs(), true);
            ILGenerator il = dm.GetILGenerator();
            node.Emit(il);
            il.Emit(OpCodes.Ret);
            if (CustomTag.functions.TryGetValue(name, out var list))
                list.Add(dm);
            else CustomTag.functions.Add(name, new List<MethodInfo>() { dm });
            isStringFunc = parser.IsString;
            canUsedByNotPlaying = parser.CanUsedByNotPlaying;
            if (parser.Errors.Any())
                error = parser.Errors[0];
            else
            {
                error = null;
                callbackAfterBuild?.Invoke();
            }
            return method = dm;
        }
        public Type[] GetArgs()
        {
            if (args?.Any() ?? false)
                return args.Select(a => a.IsString ? typeof(string) : typeof(float)).Prepend(typeof(Tag[])).ToArray();
            return new[] { typeof(Tag[]) };
        }
        public void Remove()
        {
            if (string.IsNullOrEmpty(name))
            {
                name = name_;
                return;
            }
            CustomTag.functions.Remove(name);
            name = name_;
        }
        public void AddArgument(bool isString) => args.Add(new Arg(indexCache++, isString));
        public void RemoveArgument() => args.RemoveAt(--indexCache - 1);
    }
}
