using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TinyJson;
using UnityEngine;
using Overlayer.Core.Utils;
using Overlayer.AdofaiggApi;

namespace Overlayer.Core
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
        public bool collapsed = true;
        public bool isStringTag = false;

        internal bool canUsedByNotPlaying = false;
        internal string name_;
        internal string description_;
        internal string expression_;
        internal string error;
        internal TagCompiler compiler;
        public string Compile(TagCollection reference, string name, string desc, string expr, Action callbackAfterCompile = null)
        {
            if (string.IsNullOrEmpty(name))
                return error = "Name Cannot Be Empty!";
            else if (string.IsNullOrEmpty(expr))
                return error = "Expression Cannot Be Empty!";
            reference.RemoveTag(this.name);
            this.name = name;
            description = desc;
            expression = expr;
            compiler = new TagCompiler(reference);
            compiler.Compile(name, description, expression, constants, functions, out string[] err);
            isStringTag = compiler.IsStringTag;
            canUsedByNotPlaying = compiler.CanUsedByNotPlaying;
            if (err.Any())
                return error = err[0];
            else
            {
                callbackAfterCompile?.Invoke();
                return error = null;
            }
        }
        static bool funcgui = false;
        static bool constgui = false;
        public static void FunctionGUI()
        {
            GUIUtils.IndentGUI(() =>
            {
                if (funcgui = GUILayout.Toggle(funcgui, "Functions"))
                {
                    GUIUtils.IndentGUI(() =>
                    {
                        foreach (var methodKvp in funcDescs)
                            GUILayout.Label($"{methodKvp.Key}: {methodKvp.Value}");
                    });
                }
            });
        }
        public static void ConstantsGUI()
        {
            GUIUtils.IndentGUI(() =>
            {
                if (constgui = GUILayout.Toggle(constgui, "Constants"))
                {
                    GUIUtils.IndentGUI(() =>
                    {
                        foreach (var kvp in constants)
                            GUILayout.Label($"{kvp.Key}: {kvp.Value} {constDescs[kvp.Key]}");
                    });
                }
            });
        }
        public static readonly Dictionary<string, float> constants = new Dictionary<string, float>()
        {
            { "pi", 3.1415926535897931f },
            { "e", 2.7182818284590451f },
            { "radDeg", 57.29578049f },
            { "degRad",  0.017453292f },
        };
        public static readonly Dictionary<string, string> constDescs = new Dictionary<string, string>()
        {
            { "pi", "PI" },
            { "e", "Euler's Number" },
            { "radDeg", "Radian To Degree" },
            { "degRad",  "Degree To Radian" },
        };
        public static readonly Dictionary<string, string> funcDescs = new Dictionary<string, string>()
        {
            { "cos(Number)", "Cosine" },
            { "cosh(Number)", "Hyperbolic Cosine" },
            { "sin(Number)", "Sine" },
            { "sinh(Number)", "Hyperbolic Sine" },
            { "tan(Number)", "Tangent" },
            { "tanh(Number)", "Hyperbolic Tangent" },
            { "log(Number)", "Logarithm" },
            { "abs(Number)", "Absolute Value" },
            { "acos(Number)", "Arc Cosine" },
            { "asin(Number)", "Arc Sine" },
            { "atan(Number)", "Arc Tangent" },
            { "sqrt(Number)", "Square Root" },
            { "max(Number, Number)", "Max Value" },
            { "min(Number, Number)", "Min Value" },
            { "pow(Number, Number)", "Power" },
            { "exp(Number)", "Exponential" },
            { "round(Number)", "Rounding" },
            { "round(Number, Number)", "Rounding" },
            { "ceil(Number)", "Ceiling" },
            { "floor(Number)", "Floor" },
            { "truncate(Number)", "Truncate" },
            { "truncate(Number, Number)", "Truncate" },
            { "if(Condition, String|Number, String|Number)", "If {condition} a Else b" },
            { "tostring(Number)", "Number To String" },
        };
        public static readonly Dictionary<string, List<MethodInfo>> functions = new Dictionary<string, List<MethodInfo>>
        {
            { "cos", new List<MethodInfo> { typeof(CustomTag).GetMethod("Cos", (BindingFlags)15420) } },
            { "cosh", new List<MethodInfo> { typeof(CustomTag).GetMethod("Cosh", (BindingFlags)15420) } },
            { "sin", new List<MethodInfo> { typeof(CustomTag).GetMethod("Sin", (BindingFlags)15420) } },
            { "sinh", new List<MethodInfo> { typeof(CustomTag).GetMethod("Sinh", (BindingFlags)15420) } },
            { "tan", new List<MethodInfo> { typeof(CustomTag).GetMethod("Tan", (BindingFlags)15420) } },
            { "tanh", new List<MethodInfo> { typeof(CustomTag).GetMethod("Tanh", (BindingFlags)15420) } },
            { "log", new List<MethodInfo> { typeof(CustomTag).GetMethod("Log", (BindingFlags)15420) } },
            { "abs", new List<MethodInfo> { typeof(CustomTag).GetMethod("Abs", (BindingFlags)15420) } },
            { "acos", new List<MethodInfo> { typeof(CustomTag).GetMethod("Acos", (BindingFlags)15420) } },
            { "asin", new List<MethodInfo> { typeof(CustomTag).GetMethod("Asin", (BindingFlags)15420) } },
            { "atan", new List<MethodInfo> { typeof(CustomTag).GetMethod("Atan", (BindingFlags)15420) } },
            { "sqrt", new List<MethodInfo> { typeof(CustomTag).GetMethod("Sqrt", (BindingFlags)15420) } },
            { "max", new List<MethodInfo> { typeof(CustomTag).GetMethod("Max", (BindingFlags)15420) } },
            { "min", new List<MethodInfo> { typeof(CustomTag).GetMethod("Min", (BindingFlags)15420) } },
            { "pow", new List<MethodInfo> { typeof(CustomTag).GetMethod("Pow", (BindingFlags)15420) } },
            { "exp", new List<MethodInfo> { typeof(CustomTag).GetMethod("Exp", (BindingFlags)15420) } },
            { "round", new List<MethodInfo> { typeof(CustomTag).GetMethod("Round", (BindingFlags)15420, null, new[] { typeof(float) }, null), 
                typeof(CustomTag).GetMethod("Round", (BindingFlags)15420, null, new[] { typeof(float), typeof(float) }, null) } },
            { "ceil", new List<MethodInfo> { typeof(CustomTag).GetMethod("Ceil", (BindingFlags)15420) } },
            { "floor", new List<MethodInfo> { typeof(CustomTag).GetMethod("Floor", (BindingFlags)15420) } },
            { "tostring", new List<MethodInfo> { typeof(CustomTag).GetMethod("ToString_", (BindingFlags)15420, null, new[] { typeof(float) }, null),
                typeof(CustomTag).GetMethod("ToString_", (BindingFlags)15420, null, new[] { typeof(bool) }, null)} },
            { "truncate", new List<MethodInfo> { typeof(CustomTag).GetMethod("Truncate", (BindingFlags)15420, null, new[] { typeof(float) }, null),
                typeof(CustomTag).GetMethod("Truncate", (BindingFlags)15420, null, new[] { typeof(float), typeof(float) }, null) } },
            { "if", new List<MethodInfo> { typeof(CustomTag).GetMethod("If", (BindingFlags)15420, null, new[] { typeof(bool), typeof(float), typeof(float) }, null),
                typeof(CustomTag).GetMethod("If", (BindingFlags)15420, null, new[] { typeof(bool), typeof(string), typeof(string) }, null),} },
        };
        static string If(bool condition, string a, string b)
            => condition ? a : b;
        static float If(bool condition, float a, float b)
            => condition ? a : b;
        static string ToString_(float t)
            => t.ToString();
        static string ToString_(bool t)
            => t.ToString();
        static float Exp(float f)
            => (float)Math.Exp(f);
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
        static float Round(float f)
            => (float)Math.Round(f);
        static float Round(float f, float digits)
            => (float)Math.Round(f, (int)digits);
        static float Ceil(float f)
            => (float)Math.Ceiling(f);
        static float Floor(float f)
            => (float)Math.Floor(f);
        static float Truncate(float f)
            => (float)Math.Truncate(f);
        static float Truncate(float f, float digits)
        {
            var number = Math.Pow(10, digits);
            return (float)(Math.Truncate(f * number) / number);
        }
    }
}
