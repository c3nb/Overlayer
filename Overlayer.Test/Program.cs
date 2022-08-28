using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;

namespace TagLib.Test
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(string.Format("ff{0}", 3));
            //BenchmarkRunner.Run<Benchmark>();
        }
    }
    public class Benchmark
    {
        public readonly int test = 23432434;
    }
    public unsafe static class FastParser
    {
        public static int ParseInt(string s)
        {
            int result = 0;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    result = 10 * result + (*c - 48);
                    c++;
                }
            }
            if (unary)
                return -result;
            return result;
        }
        public static double ParseDouble(string s)
        {
            double result = 0;
            bool isDot = false;
            int dCount = 1;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    if (*c == '.')
                    {
                        isDot = true;
                        goto Continue;
                    }
                    if (!isDot)
                        result = 10 * result + (*c - 48);
                    else result += (*c - 48) / dPow[dCount++];
                Continue:
                    c++;
                }
            }
            if (unary)
                return -result;
            return result;
        }
        public static float ParseFloat(string s)
        {
            float result = 0;
            bool isDot = false;
            int dCount = 1;
            bool unary = s[0] == 45;
            fixed (char* v = s)
            {
                char* c = v;
                if (unary) c++;
                while (*c != '\0')
                {
                    if (*c == '.')
                    {
                        isDot = true;
                        goto Continue;
                    }
                    if (!isDot)
                        result = 10 * result + (*c - 48);
                    else result += (*c - 48) / fPow[dCount++];
                Continue:
                    c++;
                }
            }
            if (unary)
                return -result;
            return result;
        }
        private static readonly double[] dPow = GetDoublePow();
        private static double[] GetDoublePow()
        {
            var max = 309;
            var exps = new double[max];
            for (var i = 0; i < max; i++)
                exps[i] = Math.Pow(10, i);
            return exps;
        }
        private static readonly float[] fPow = GetFloatPow();
        private static float[] GetFloatPow()
        {
            var max = 39;
            var exps = new float[max];
            for (var i = 0; i < max; i++)
                exps[i] = (float)Math.Pow(10, i);
            return exps;
        }
    }
}
