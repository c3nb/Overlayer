using System;
using System.Collections.Generic;

namespace Overlayer.Core
{
    internal static class BuiltInFunctions
    {
        public static readonly Random random = new Random();
        static readonly Dictionary<string, string> VariablesStr = new Dictionary<string, string>();
        static readonly Dictionary<string, float> VariablesNum = new Dictionary<string, float>();
        static float Abs(float f)
            => (float)Math.Abs(f);
        static float Acos(float f)
            => (float)Math.Acos(f);
        static float Asin(float f)
            => (float)Math.Asin(f);
        static float Atan(float f)
            => (float)Math.Atan(f);
        static float Ceil(float f)
            => (float)Math.Ceiling(f);
        static float Cos(float f)
            => (float)Math.Cos(f);
        static float Cosh(float f)
            => (float)Math.Cosh(f);
        static float Exp(float f)
            => (float)Math.Exp(f);
        static float Floor(float f)
            => (float)Math.Floor(f);
        static float GetNumber(string name) => VariablesNum.TryGetValue(name, out float value) ? value : 0;
        static string GetString(string name) => VariablesStr.TryGetValue(name, out string value) ? value : string.Empty;
        static string If(bool condition, string a, string b)
            => condition ? a : b;
        static float If(bool condition, float a, float b)
            => condition ? a : b;
        static float Log(float f)
            => (float)Math.Log(f);
        static float Max(float a, float b)
            => (float)Math.Max(a, b);
        static float Min(float a, float b)
            => (float)Math.Min(a, b);
        static float Pow(float a, float b)
            => (float)Math.Pow(a, b);
        static float Random() => (float)random.NextDouble();
        static float RandomRange(float min, float max) => random.Next((int)min, (int)max);
        static float Round(float f)
            => (float)Math.Round(f);
        static float Round(float f, float digits)
            => (float)Math.Round(f, (int)digits);
        static float SetNumber(string name, float value) => VariablesNum[name] = value;
        static string SetString(string name, string value) => VariablesStr[name] = value;
        static float Sin(float f)
            => (float)Math.Sin(f);
        static float Sinh(float f)
            => (float)Math.Sinh(f);
        static float Sqrt(float f)
            => (float)Math.Sqrt(f);
        static float Tan(float f)
            => (float)Math.Tan(f);
        static float Tanh(float f)
            => (float)Math.Tanh(f);
        static string ToString(float t)
            => t.ToString();
        static string ToString(bool t)
            => t.ToString();
        static float Truncate(float f)
            => (float)Math.Truncate(f);
        static float Truncate(float f, float digits)
        {
            var number = Math.Pow(10, digits);
            return (float)(Math.Truncate(f * number) / number);
        }
    }
}