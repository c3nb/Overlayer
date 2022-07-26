using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TinyJson;

namespace Overlayer.Tags
{
    public class CustomTag
    {
        public static List<CustomTag> Tags = new List<CustomTag>();
        public static readonly string JsonPath = Path.Combine("Mods", "Overlayer", "CustomTags.json");
        public static void Load()
        {
            if (File.Exists(JsonPath))
                Tags = File.ReadAllText(JsonPath).FromJson<List<CustomTag>>();
            else Tags = new List<CustomTag>();
        }
        public static void Save()
            => File.WriteAllText(JsonPath, Tags.ToJson());
        public string name = string.Empty;
        public string description = string.Empty;
        public string expression = string.Empty;

        internal bool canUsedByNotPlaying = false;
        internal string name_;
        internal string description_;
        internal string expression_;
        internal string error;
        internal CustomTagCompiler compiler;
        public string Compile(TagCollection reference, string name, string desc, string expr)
        {
            if (string.IsNullOrEmpty(name))
                return error = "Name Cannot Be Empty!";
            else if (string.IsNullOrEmpty(expr))
                return error = "Expression Cannot Be Empty!";
            reference.RemoveTag(this.name);
            this.name = name;
            description = desc;
            expression = expr;
            compiler = new CustomTagCompiler(reference);
            compiler.Compile(name, description, expression, constants, functions, out string[] err);
            canUsedByNotPlaying = compiler.CanUsedByNotPlaying;
            Recompile();
            if (err.Any())
                return error = err[0];
            else return error = null;
        }
        public static void Recompile()
        {
            foreach (OText text in OText.Texts)
            {
                text.PlayingCompiler.Compile(text.TSetting.PlayingText);
                text.NotPlayingCompiler.Compile(text.TSetting.NotPlayingText);
            }
        }
        public static readonly Dictionary<string, float> constants = new Dictionary<string, float>()
        {
            { "pi", 3.1415926535897931f },
            { "e", 2.7182818284590451f }
        };
        public static readonly Dictionary<string, MethodInfo> functions = new Dictionary<string, MethodInfo>
        {
            { "cos", typeof(CustomTag).GetMethod("Cos", (BindingFlags)15420) },
            { "cosh", typeof(CustomTag).GetMethod("Cosh", (BindingFlags)15420) },
            { "sin", typeof(CustomTag).GetMethod("Sin", (BindingFlags)15420) },
            { "sinh", typeof(CustomTag).GetMethod("Sinh", (BindingFlags)15420) },
            { "tan", typeof(CustomTag).GetMethod("Tan", (BindingFlags)15420) },
            { "tanh", typeof(CustomTag).GetMethod("Tanh", (BindingFlags)15420) },
            { "log", typeof(CustomTag).GetMethod("log", (BindingFlags)15420) },
            { "abs", typeof(CustomTag).GetMethod("Abs", (BindingFlags)15420) },
            { "acos", typeof(CustomTag).GetMethod("Acos", (BindingFlags)15420) },
            { "asin", typeof(CustomTag).GetMethod("Asin", (BindingFlags)15420) },
            { "atan", typeof(CustomTag).GetMethod("Atan", (BindingFlags)15420) },
            { "sqrt", typeof(CustomTag).GetMethod("Sqrt", (BindingFlags)15420) },
            { "max", typeof(CustomTag).GetMethod("Max", (BindingFlags)15420) },
            { "min", typeof(CustomTag).GetMethod("Min", (BindingFlags)15420) },
            { "pow", typeof(CustomTag).GetMethod("Pow", (BindingFlags)15420) },
        };
        static float Cos(float f)
            => (float)Math.Cos(f);
        static float Cosh(float f)
            => (float)Math.Cosh(f);
        static float Sin(float f)
            => (float)Math.Sin(f);
        static float Sinh(float f)
            => (float)Math.Sinh(f);
        static float Tan(float f)
            => (float)Math.Tan(f);
        static float Tanh(float f)
            => (float)Math.Tanh(f);
        static float Log(float f)
            => (float)Math.Log(f);
        static float Abs(float f)
            => (float)Math.Abs(f);
        static float Acos(float f)
            => (float)Math.Acos(f);
        static float Asin(float f)
            => (float)Math.Asin(f);
        static float Atan(float f)
            => (float)Math.Atan(f);
        static float Sqrt(float f)
            => (float)Math.Sqrt(f);
        static float Max(float a, float b)
            => (float)Math.Max(a, b);
        static float Min(float a, float b)
            => (float)Math.Min(a, b);
        static float Pow(float a, float b)
            => (float)Math.Pow(a, b);
    }
}
