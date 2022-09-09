using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using TagLib.Utils;
using Overlayer.AdofaiggApi;
using Overlayer.Tags.Global;
using Overlayer.AdofaiggApi.Types;

namespace Overlayer.Test
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //BenchmarkRunner.Run<Benchmark>();
            var one = PlayPoint.CalculatePlayPoint(20.6, 100, 97.93, 3166);
            Console.WriteLine(one);
        }
        public static void MoveLast<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length - 1; i++)
                array[i] = array[i + 1];
            array[array.Length - 1] = value;
        }
    }
    public class Benchmark
    {
        public double d = 3000;
        [Benchmark]
        public void Math_Pow()
        {
            Math.Pow(d, 200);
        }
        [Benchmark]
        public void PowEx()
        {
            d.Pow(200);
        }
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
