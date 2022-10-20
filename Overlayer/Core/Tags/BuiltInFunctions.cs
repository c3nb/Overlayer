using System;
using System.Collections.Generic;

namespace Overlayer.Core
{
    public static class BuiltInFunctions
    {
        public static readonly Random random = new Random();
        static readonly Dictionary<string, string> VariablesStr = new Dictionary<string, string>();
        static readonly Dictionary<string, double> VariablesNum = new Dictionary<string, double>();
        static double Abs(double f)
            => (double)Math.Abs(f);
        static double Acos(double f)
            => (double)Math.Acos(f);
        static double Asin(double f)
            => (double)Math.Asin(f);
        static double Atan(double f)
            => (double)Math.Atan(f);
        static double Ceil(double f)
            => (double)Math.Ceiling(f);
        static double Cos(double f)
            => (double)Math.Cos(f);
        static double Cosh(double f)
            => (double)Math.Cosh(f);
        static double Exp(double f)
            => (double)Math.Exp(f);
        static double Floor(double f)
            => (double)Math.Floor(f);
        static double GetNumber(string name) => VariablesNum.TryGetValue(name, out double value) ? value : 0;
        static string GetString(string name) => VariablesStr.TryGetValue(name, out string value) ? value : string.Empty;
        static string If(bool condition, string a, string b)
            => condition ? a : b;
        static double If(bool condition, double a, double b)
            => condition ? a : b;
        static double Log(double f)
            => (double)Math.Log(f);
        static double Max(double a, double b)
            => (double)Math.Max(a, b);
        static double Min(double a, double b)
            => (double)Math.Min(a, b);
        static double Pow(double a, double b)
            => (double)Math.Pow(a, b);
        static double Random() => (double)random.NextDouble();
        static double RandomRange(double min, double max) => random.Next((int)min, (int)max);
        static double Round(double f)
            => (double)Math.Round(f);
        static double Round(double f, double digits)
            => (double)Math.Round(f, (int)digits);
        static double SetNumber(string name, double value) => VariablesNum[name] = value;
        static string SetString(string name, string value) => VariablesStr[name] = value;
        static double Sin(double f)
            => (double)Math.Sin(f);
        static double Sinh(double f)
            => (double)Math.Sinh(f);
        static double Sqrt(double f)
            => (double)Math.Sqrt(f);
        static double Tan(double f)
            => (double)Math.Tan(f);
        static double Tanh(double f)
            => (double)Math.Tanh(f);
        static string ToString(double t)
            => t.ToString();
        static string ToString(bool t)
            => t.ToString();
        static double Truncate(double f)
            => (double)Math.Truncate(f);
        static double Truncate(double f, double digits)
        {
            var number = Math.Pow(10, digits);
            return (double)(Math.Truncate(f * number) / number);
        }
    }
}